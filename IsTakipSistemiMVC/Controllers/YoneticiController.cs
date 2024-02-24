using IsTakipSistemiMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.Mvc;

namespace IsTakipSistemiMVC.Controllers
{
    public class YoneticiController : Controller
    {
        IsTakipSistemiDBEntities entity=new IsTakipSistemiDBEntities();
        // GET: Yonetici
        public ActionResult Index()
        {
            int yetkiTurId=Convert.ToInt32( Session["PersonelYetkiTurId"]);
            if (yetkiTurId==1)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);
                var birim= (from b in entity.Birimler where b.BirimID==birimId select b).FirstOrDefault();
                
                ViewBag.birimAd = birim.BirimAd;

                return View();
            }
            else
            {
                return RedirectToAction("Index","Login");
            }
            
        }

        public ActionResult Ata()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);
            if (yetkiTurId == 1)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);

                var calisanlar=(from p in entity.Personeller where p.PersonelBirimID==birimId 
                                && p.PersonelYetkiTurID==2   select p).ToList();

                ViewBag.personeller = calisanlar;
               
                var birim = (from b in entity.Birimler where b.BirimID == birimId select b).FirstOrDefault();

                ViewBag.birimAd = birim.BirimAd;

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }
        [HttpPost]
        public ActionResult Ata(FormCollection formCollection)
        {
            string IsBaslik = formCollection["IsBaslik"];
            string IsAciklama = formCollection["IsAciklama"];
            int secilenPersonelId =Convert.ToInt32( formCollection["selectPer"]);

            Isler yeniIs = new Isler();
            yeniIs.IsBaslik = IsBaslik;
            yeniIs.IsAciklama=IsAciklama;
            yeniIs.IsPersonelID=secilenPersonelId;
            yeniIs.IletilenTarih=DateTime.Now;
            yeniIs.IsDurumID = 1;
            yeniIs.IsOkunma = false;


            entity.Isler.Add(yeniIs);//ekleme işlemi mvc projesinde kalır veritabanını tetiklemek için :
            entity.SaveChanges();


            return RedirectToAction("Takip", "Yonetici");

        }
        public ActionResult Takip()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);
            if (yetkiTurId == 1)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);

                var calisanlar = (from p in entity.Personeller
                                  where p.PersonelBirimID == birimId
                                && p.PersonelYetkiTurID == 2
                                  select p).ToList();

                ViewBag.personeller = calisanlar;

                var birim = (from b in entity.Birimler where b.BirimID == birimId select b).FirstOrDefault();

                ViewBag.birimAd = birim.BirimAd;

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult Takip(int selectPer)
        {
            var secilenPersonel=(from p in entity.Personeller where p.PersonelID==selectPer select p ).FirstOrDefault();

            TempData["secilen"]=secilenPersonel;


            return RedirectToAction("Listele", "Yonetici");

        }

        [HttpGet]
        public ActionResult Listele()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);
            if (yetkiTurId == 1)
            {
                Personeller secilenPersonel = (Personeller)TempData["secilen"];

                try
                {
                    var isler = (from i in entity.Isler where i.IsPersonelID == secilenPersonel.PersonelID 
                                 select i).ToList().OrderByDescending(i => i.IletilenTarih);

                    ViewBag.isler = isler;
                    ViewBag.personel = secilenPersonel;
                    ViewBag.isSayisi = isler.Count();
                    
                    return View();
                }
                catch (Exception)
                {

                    return RedirectToAction("Takip", "Yonetici");
                }

                
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
    }
}
