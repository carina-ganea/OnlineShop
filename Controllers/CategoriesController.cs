using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Proiect.Models;

namespace Proiect.Controllers
{
    
    public class CategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Category
        public ActionResult Index()
        {
            var categories = from category in db.Categories
                            orderby category.CategoryName
                            select category;
            ViewBag.Categories = categories;
            ViewBag.isAdmin = User.IsInRole("Admin");

            if (TempData.ContainsKey("message")) 
            { 
                ViewBag.message = TempData["message"].ToString(); 
            }
            return View();
        }

        public ActionResult Show (int id)
        {
            Category category = db.Categories.Find(id);
            ViewBag.Products = category.Products;
            ViewBag.isAdmin = User.IsInRole("Admin");
            return View(category);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult New()
        {
            //System.Diagnostics.Debug.WriteLine("Primul pas");
            ViewBag.isAdmin = User.IsInRole("Admin");
            return View();

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult New( Category cat)
        {
            //System.Diagnostics.Debug.WriteLine("Al doilea pas");

            try
            {
                if ( ModelState.IsValid )
                {

                   // System.Diagnostics.Debug.WriteLine("Este valid\n");

                    db.Categories.Add(cat);
                    db.SaveChanges();

                    TempData["message"] = "The category has been added!";

                    return Redirect("/Categories/Index");
                } 
                else
                {
                    //System.Diagnostics.Debug.WriteLine("Nu este valid\n");

                    return View(cat);
                }
            }
            catch ( Exception e)
            {
                return View(cat);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit ( int id)
        {
            Category cat = db.Categories.Find(id);

            ViewBag.isAdmin = User.IsInRole("Admin");
            return View(cat);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public ActionResult Edit ( int id, Category cat)
        {
            try
            {
                if ( ModelState.IsValid )
                {
                    Category category = db.Categories.Find(id);
                    if (TryUpdateModel(category))
                    {
                        category.CategoryName = cat.CategoryName;
                        db.SaveChanges();
                    }
                    TempData["message"] = "The category has been changed!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(cat);
                }
            }
            catch ( Exception e)
            {
                return View(cat);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public ActionResult Delete ( int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            TempData["message"] = "The category has been deleted!";
            return RedirectToAction("Index");
        }
    }
}