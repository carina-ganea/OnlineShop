using Microsoft.AspNet.Identity;
using Proiect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proiect.Controllers
{
    public class CartsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Carts
        public ActionResult Index()
        {
            string id = User.Identity.GetUserId();
            Cart cart = db.Carts.Find(id);


            float sum = 0;
            foreach (var elem in cart.CartProducts)
            {
                sum = sum + (elem.Product.Price * elem.Amount);
            }
            ViewBag.TotalPrice = sum;


            var cartProducts = db.CartProducts.Where(c => c.CartId == id).ToList();
            ViewBag.Elements = cartProducts;

            return View(cart);
        }


        [HttpDelete]
        public ActionResult Delete(int Id)
        {
            string iduser = User.Identity.GetUserId();

            CartProduct element = db.CartProducts.Where(c => c.CartId == iduser && c.IdProduct == Id).FirstOrDefault();
            db.CartProducts.Remove(element);

            db.SaveChanges();
            return RedirectToAction("Index");
        }



    }
}