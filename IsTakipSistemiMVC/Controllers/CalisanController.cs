using IsTakipSistemiMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace IsTakipSistemiMVC.Controllers
{
    public class IsDurum
    {
        public string IsBaslik { get; set; }
        public string IsAciklama { get; set; }
        public DateTime? IletilenTarih { get; set; }
        public DateTime? TeslimTarihi { get; set; }
        public string DurumAd { get; set; }
        public string IsYorum { get; set; }
    }
    public class CalisanController : Controller
    {
        IsTakipSistemiDBEntities entity=new IsTakipSistemiDBEntities();
        // GET: Calisan
        public ActionResult Index()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 2)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);
                var birim=(from b in entity.Birimler where b.BirimID== birimId select b).FirstOrDefault();

                ViewBag.birimAd = birim.BirimAd;
                int personelId = Convert.ToInt32(Session["PersonelId"]);
                var isler=(from i in entity.Isler where i.IsPersonelID== personelId && i.IsOkunma==false orderby i.IletilenTarih descending select i).ToList();

                ViewBag.isler = isler;
                return View();
            }
            else
            {
                return RedirectToAction("Index","Login");
            }
        }
        [HttpPost]
        public ActionResult Index(int IsID)
        {
            var tekIs=(from i in entity.Isler where i.IsID==IsID select i).FirstOrDefault();
            tekIs.IsOkunma=true;
            entity.SaveChanges();
            return RedirectToAction("Yap", "Calisan");

        }

        public ActionResult Yap()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 2)
            {
                int personelId = Convert.ToInt32(Session["PersonelId"]);

                var isler=(from i in entity.Isler where i.IsPersonelID== personelId && i.IsDurumID==1 
                           select i).ToList().OrderByDescending(i=>i.IletilenTarih);

                ViewBag.isler=isler;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult Yap(int IsID, string IsYorum )
        {
            var tekIs = (from i in entity.Isler where i.IsID == IsID select i).FirstOrDefault();
            if (IsYorum == "") IsYorum = "Kullanıcı Yorum yapmadı";

            tekIs.TeslimTarihi = DateTime.Now;
            tekIs.IsDurumID = 2;
            tekIs.IsYorum= IsYorum;

            entity.SaveChanges();

            return RedirectToAction("Index", "Calisan");
        }

        public ActionResult Takip()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 2)
            {
                int personelId = Convert.ToInt32(Session["PersonelId"]);

                var isler = (from i in entity.Isler join d in entity.IsDurumlar on i.IsDurumID equals d.DurumID
                             where i.IsPersonelID==personelId select i).ToList().OrderByDescending(i=>i.IletilenTarih);

                IsDurumModel model= new IsDurumModel();

                List<IsDurum> list =new List<IsDurum>();

                foreach (var i in isler)
                {
                    IsDurum isDurum = new IsDurum();
                    isDurum.IsBaslik = i.IsBaslik;
                    isDurum.IsAciklama= i.IsAciklama;
                    isDurum.IletilenTarih= i.IletilenTarih;
                    isDurum.TeslimTarihi= i.TeslimTarihi;
                    isDurum.DurumAd = i.IsDurumlar.DurumAd;
                    isDurum.IsYorum=i.IsYorum;

                    list.Add(isDurum);
                }
                model.IsDurumlar = list;

               

                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

    }
}

