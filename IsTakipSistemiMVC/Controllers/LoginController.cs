using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IsTakipSistemiMVC.Models;

namespace IsTakipSistemiMVC.Controllers
{
    public class LoginController : Controller
    {
        IsTakipSistemiDBEntities entity=new IsTakipSistemiDBEntities();
        // GET: Login
        public ActionResult Index()
        {
            ViewBag.mesaj = null;//hata olmaması için burda oluşturduk.
            return View();
        }
        [HttpPost]
        public ActionResult Index(string kullaniciAd,string parola )
        {
            Personeller personel = (from p in entity.Personeller
                                    where p.PersonelKullanıcıAd == kullaniciAd && p.PersonelParola == parola
                                    select p).FirstOrDefault();  //Entity Framework sorgusu
            if (personel != null )
            {
                Session["PersonelAdSoyad"] = personel.PersonelAdSoyad; //giris yaptıgımız için tum controller ve viewlarda personelin bilgisine erişmek için session .
                Session["PersonelId"] = personel.PersonelID;
                Session["PersonelBirimId"] = personel.PersonelBirimID;
                Session["PersonelYetkiTurId"] = personel.PersonelYetkiTurID;

                //yetki türü id 'ye göre sayfa yönlendirmesi
                switch (personel.PersonelYetkiTurID)
                {
                    case 1:
                        return RedirectToAction("Index","Yonetici");//redirect ile bilgilerini verdigimiz view e yönlendirecek
                    case 2:
                        return RedirectToAction("Index", "Calisan");
                    default:
                        return View();
                }
            }
            else
            {
                ViewBag.mesaj = "Kullanıcı adı ya da parola yanlış ";//controllerdaki action dan view a veri göndermek .veriyi aktarma işlemi
                                                                     ///kullanmak istedigimizde cshtmle git butonun altına p etiketi içerisinde @ViewBag.mesaj yaz.
                return View();
            }
            
        }
    }
}