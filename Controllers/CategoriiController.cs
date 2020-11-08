using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;

namespace OnlineShop.Controllers
{
    public class CategoriiController : Controller
    {
        private Models.AppContext db = new Models.AppContext();

        // GET: Categorie
        public ActionResult Index()
        {
            var categorii = from categorie in db.Categorii
                            orderby categorie.Denumire
                            select categorie;
            ViewBag.Categorii = categorii;

            if (TempData.ContainsKey("message")) 
            { 
                ViewBag.message = TempData["message"].ToString(); 
            }
            return View();
        }

        public ActionResult Afiseaza (int id)
        {
            Categorie categorie = db.Categorii.Find(id);
            ViewBag.Categorie = categorie;
            ViewBag.Produse = categorie.Produse;
            return View();
        }

        public ActionResult Adauga()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Adauga( Categorie cat)
        {
            try
            {
                db.Categorii.Add(cat);
                db.SaveChanges();

                TempData["message"] = "Categoria a fost adaugata!";

                return Redirect("/Categorii/Index");
            }
            catch ( Exception e)
            {
                ViewBag.IdCategorie = cat.IdCategorie;
                return View();
            }
        }

        public ActionResult Editare ( int id)
        {
            Categorie cat = db.Categorii.Find(id);
            ViewBag.Categorie = cat;
            return View();
        }

        [HttpPut]
        public ActionResult Editare ( int id, Categorie cat)
        {
            try
            {
                Categorie categorie = db.Categorii.Find(id);
                if(TryUpdateModel(categorie))
                {
                    categorie.Denumire = cat.Denumire;
                    db.SaveChanges();
                }
                TempData["message"] = "Categoria a fost editata!";
                return RedirectToAction("Index");
            }
            catch ( Exception e)
            {
                return View();
            }
        }

        [HttpDelete]
        public ActionResult Stergere ( int id)
        {
            Categorie categorie = db.Categorii.Find(id);
            db.Categorii.Remove(categorie);
            db.SaveChanges();
            TempData["message"] = "Categoria a fost stearsa!";
            return RedirectToAction("Index");
        }
    }
}