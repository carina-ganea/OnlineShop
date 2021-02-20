using Microsoft.AspNet.Identity;
using Proiect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proiect.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Review
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult New(Review review)
        {
            review.Date = DateTime.Now;
            review.UserId = User.Identity.GetUserId();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Reviews.Add(review);
                    db.SaveChanges();
                    TempData["message"] = "Review added!";
                    return Redirect("/Products/Show/" + review.IdProduct);
                }
                else
                {
                    TempData["errorMessage"] = "Can't add the Review!";
                    return Redirect("/Products/Show/" + review.IdProduct);
                }
            }
            catch(Exception e)
            {
                return Redirect("/Products/Show/" + review.IdProduct);
            }
        }

        public ActionResult Edit(int id)
        {
            Review review = db.Reviews.Find(id);

            if (User.Identity.GetUserId() == review.UserId)
            {
                return View(review);
            }
            else
            {
                TempData["message"] = "You can't edit the product.";
                return Redirect("/Products/Show/" + review.IdProduct);
            }
        }

        [HttpPut]
        public ActionResult Edit(int id, Review nouReview)
        {
            try
            {   
                Review review = db.Reviews.Find(id);

                if(ModelState.IsValid)
                {

                    if (User.Identity.GetUserId() == review.UserId)
                    {
                        if (TryUpdateModel(review))
                        {
                            review.Content = nouReview.Content;
                            db.SaveChanges();
                            TempData["message"] = "Review edited";
                        }
                        return Redirect("/Products/Show/" + review.IdProduct);

                    }
                    else
                    {
                        TempData["message"] = "You can't edit the review.";
                        return View(review);
                    }

                }
                else
                {
                    return View(review);
                }
            }
            catch(Exception e)
            {
                ViewBag.Review = nouReview;
                return Redirect("/Product/Show/" + nouReview.IdProduct);
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Review review = db.Reviews.Find(id);

            if (User.Identity.GetUserId() == review.UserId || User.IsInRole("Admin"))
            {
                db.Reviews.Remove(review);
                db.SaveChanges();
                TempData["message"] = "Review deleted!";
            }
            else
            {
                TempData["message"] = "You can't delete the review.";
            }

            return Redirect("/Products/Show/" + review.IdProduct);

        }

    }
}