using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace OnlineShop.Controllers
{
    public class ProduseController : Controller
    {
        private Models.AppContext db = new Models.AppContext();

        [NonAction]
        public IEnumerable<SelectListItem> ListaCategorii()
        {
            var selectList = new List<SelectListItem>();

            var categorii = from cat in db.Categorii
                            select cat;

            foreach (var categorie in categorii)
            {
                selectList.Add(new SelectListItem
                {
                    Value = categorie.IdCategorie.ToString(),
                    Text = categorie.Denumire.ToString()
                });
            }
            return selectList;
        }

        // GET: Produs
        public ActionResult Index()
        {
            ViewBag.Produse = from produs in db.Produse
                              select produs;

            if(TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }
            return View();
        }

        public ActionResult Afiseaza( int id)
        {
            Produs produs = db.Produse.Find(id);
            ViewBag.Produs = produs;
            ViewBag.Reviewuri = from review in produs.Reviewuri
                                select review;
            return View();
        }

        public ActionResult Adauga()
        {
            Produs produs = new Produs
            {
                Categorii = ListaCategorii()
            };
            return View(produs);
        }

        [HttpPost]
        public ActionResult Adauga( Produs produs)
        {
            try
            {
                db.Produse.Add(produs);
                db.SaveChanges();
                TempData["message"] = "Produsul a fost adaugat!";
                return Redirect("/Produse/Index");
            }
            catch( Exception e)
            {
                ViewBag.IdProdus = produs.IdProdus;
                return View();
            }
        }

        public ActionResult Editare ( int id)
        {
            Produs produs = db.Produse.Find(id);
            
            return View(produs);
        }

       
    }
}