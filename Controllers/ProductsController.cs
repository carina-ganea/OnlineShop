using Microsoft.AspNet.Identity;
using Proiect.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data.Entity;

namespace Proiect.Controllers
{
     public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private int _perPage = 3;

        [NonAction]
        public IEnumerable<SelectListItem> GetCategoriesList()
        {
            var selectList = new List<SelectListItem>();

            var categories = from cat in db.Categories
                            select cat;

            foreach (var category in categories)
            {
                selectList.Add(new SelectListItem
                {
                    Value = category.IdCategory.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }
            return selectList;
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetUsersList()
        {
            var selectList = new List<SelectListItem>();

            var users = from user in db.Users
                             select user;

            foreach (var u in users)
            {
                selectList.Add(new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.Email.ToString()
                });
            }
            return selectList;
        }


        // GET: Product
        //[Authorize(Roles = "User, Colaborator, Admin")]
        public ActionResult Index()
        {
            var products = from product in db.Products
                           join accepted in db.Accepteds on product.IdProduct equals accepted.IdProduct
                           where accepted.Verified == true
                           select product;

            ViewBag.Products = products;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            var Products = products.OrderBy( p => p.ProductName);

            var search = "";

            if (Request.Params.Get("search") != null)
            {
                search = Request.Params.Get("search").Trim();
                
                Products = products.Where(
                    at => at.ProductName.Contains(search)).OrderBy(p => p.ProductName);
            }

            if( Request.Params.Get("orderby") != null)
            {
                var order = Request.Params.Get("orderby");

                if( order == "priceAscending")
                {
                    Products = products.Where(
                    at => at.ProductName.Contains(search)).OrderBy(p => p.Price);
                } 
                else if ( order == "priceDescending")
                {
                    Products = products.Where(
                    at => at.ProductName.Contains(search)).OrderByDescending(p => p.Price);
                } else if ( order == "ratingAscending")
                {
                    Products = products.Where(
                    at => at.ProductName.Contains(search)).Include("ProductRating").OrderBy(p => p.ProductRating.Rating);
                } else
                {
                    Products = products.Where(
                    at => at.ProductName.Contains(search)).Include("ProductRating").OrderByDescending(p => p.ProductRating.Rating);
                }
            }

            var totalItems = products.Count();
            var currentPage = Convert.ToInt32(Request.Params.Get("page"));

            var offset = 0;

            if(!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }

            var paginatedProducts = Products.Skip(offset).Take(_perPage);

            ViewBag.total = totalItems;
            ViewBag.lastPage = Math.Ceiling((float)totalItems / _perPage);
            ViewBag.Products = paginatedProducts;
            ViewBag.SearchString = search;


            ViewBag.isAdmin = false;

            if (User.IsInRole("Admin"))
            {
                ViewBag.isAdmin = true;

                var productsNotAccepted = from product in db.Products
                                          join accepted in db.Accepteds on product.IdProduct equals accepted.IdProduct
                                          where accepted.Verified == false
                                          select product;

                //System.Diagnostics.Debug.WriteLine(productsNotAccepted.FirstOrDefault().ProductName); ////

                ViewBag.ProductsNotAccepted = productsNotAccepted;

            }

            return View();
        }


        //[Authorize(Roles = "User, Colaborator, Admin")]
        public ActionResult Show(int id)
        {
            Product product = db.Products.Find(id);

            ViewBag.Reviews = from review in product.Reviews
                                select review;

            ViewBag.Users = GetUsersList();

            float marks = 0;
            int nr_marks = 0;

            foreach( Rating x in db.Ratings)
            {
                if( x.ProductId == id)
                {
                    marks += x.Mark;
                    nr_marks++;
                }
            }

            if( nr_marks > 0)
            {
                int rez = (int) marks * 100 / nr_marks;
                ViewBag.Rating = (float) rez / 100;
            }
            else
            {
                ViewBag.Rating = 0;
            }

            if ( TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            if (TempData.ContainsKey("errorMessage"))
            {
                ViewBag.errorMessage = TempData["errorMessage"];
            }

            if (TempData.ContainsKey("review"))
            {
                ViewBag.review = TempData["review"];
            }
            else
            {
                ViewBag.review = new Review();
            }

            ViewBag.isAdmin = User.IsInRole("Admin");
            ViewBag.isColaborator = User.IsInRole("Colaborator");
            ViewBag.currentUser = User.Identity.GetUserId();


            Accepted accepted = db.Accepteds.Where(a => a.IdProduct == id).FirstOrDefault();
            ViewBag.IsAccepted = accepted.Verified;

            return View(product);
        }


        [Authorize(Roles = "Colaborator, Admin")]
        public ActionResult New()
        {
            Product product = new Product
            {
                Categories = GetCategoriesList()
            };

            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = "Colaborator, Admin")]
        public ActionResult New( Product product)
        {
            product.Categories = GetCategoriesList();
            product.UserId = User.Identity.GetUserId();
            
            try
            {
                if( ModelState.IsValid)
                {
                    string fileExtension = Path.GetExtension(product.Image.FileName);
                    if (fileExtension == ".png" || fileExtension == ".jpg" || fileExtension == ".jpeg") { 
                    
                        string uploadFolderPath = Server.MapPath("~//Files//");
                        product.Image.SaveAs(uploadFolderPath + product.Image.FileName);
                        product.FileName = product.Image.FileName;
                        ProductRating productRating = new ProductRating();
                        productRating.IdProduct = product.IdProduct;
                        productRating.Rating = 0;
                        product.ProductRating= productRating;
                        db.Products.Add(product);
                        db.ProductRatings.Add(productRating);
                        db.SaveChanges();

                        {   //codul de la adaugarea lui accepted
                            Accepted accepted = new Accepted();
                            accepted.IdP = product.IdProduct;
                            accepted.IdProduct = product.IdProduct;

                            if (User.IsInRole("Admin"))
                            {
                                accepted.Verified = true;
                            }
                            else
                            {
                                accepted.Verified = false;
                            }

                            db.Accepteds.Add(accepted);

                            db.SaveChanges();
                        }


                        TempData["message"] = "Product added!";
                        return Redirect("/Products/Index");
                    }
                    else
                    {
                        TempData["BadFileExtension"] = "File should have .jpg , .jpeg or .png extension.";
                        return View(product);
                    }
                } 
                else
                {
                    return View(product);
                }
            }
            catch( Exception e)
            {
                return View(product);
            }
        }

        [Authorize(Roles = "Colaborator, Admin")]
        public ActionResult Edit ( int id )
        {
            Product product = db.Products.Find(id);
            product.Categories = GetCategoriesList();

            if (User.Identity.GetUserId() == product.UserId || User.IsInRole("Admin"))
            {
                
                return View(product);
            }
            else
            {
                TempData["message"] = "You can't edit the product.";
                return RedirectToAction("Index");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Colaborator, Admin")]
        public ActionResult Edit ( int id, Product product)
        {
            product.Categories = GetCategoriesList();

            /*
            System.Diagnostics.Debug.WriteLine(""); ////
            System.Diagnostics.Debug.WriteLine(""); ////

            System.Diagnostics.Debug.WriteLine("User.Identity.GetUserId():"); ////
            System.Diagnostics.Debug.WriteLine(User.Identity.GetUserId()); ////

            System.Diagnostics.Debug.WriteLine("product.UserId:"); ////
            System.Diagnostics.Debug.WriteLine(product.UserId); ////

            System.Diagnostics.Debug.WriteLine("User.IsInRole()"); ////
            System.Diagnostics.Debug.WriteLine(User.IsInRole("Colaborator")); ////
            */

            try
            {
                if ( ModelState.IsValid)
                { 
                    Product productEdit = db.Products.Find(id);
                    string fileExtension = Path.GetExtension(product.Image.FileName);

                    if (User.Identity.GetUserId() == product.UserId || User.IsInRole("Admin"))
                    {
                        if (fileExtension == ".png" || fileExtension == ".jpg" || fileExtension == ".jpeg")
                        {
                            System.IO.File.Delete(Server.MapPath("~/Files/" + productEdit.FileName));
                            string uploadFolderPath = Server.MapPath("~//Files//");
                            product.Image.SaveAs(uploadFolderPath + product.Image.FileName);
                            

                            if (TryUpdateModel(productEdit))
                            {
                                //System.Diagnostics.Debug.WriteLine("A facut editarea\n"); ////
                                productEdit.FileName = product.Image.FileName;
                                productEdit.ProductName = product.ProductName;
                                productEdit.Price = product.Price;
                                productEdit.IdCategory = product.IdCategory;
                                productEdit.Description = product.Description;
                                db.SaveChanges();
                                TempData["message"] = "Product edited!";
                            }
                            return Redirect("/Products/Show/" + productEdit.IdProduct);
                        }
                        else
                        {
                            TempData["BadFileExtension"] = "File should have .jpg , .jpeg or .png extension.";
                            product.Categories = GetCategoriesList();
                            return View(product);
                        }
                        //System.Diagnostics.Debug.WriteLine("A recunoscut ca cel care a publicat\n"); ////
                    }
                    else
                    {
                        //System.Diagnostics.Debug.WriteLine("Nu a fost recunoscut\n"); ////

                        TempData["message"] = "You can't edit the product.";
                        return RedirectToAction("Index");
                    }  
                }
                else
                {
                    product.Categories = GetCategoriesList();
                    return View(product);
                }
            }
            catch( Exception e)
            {
                product.Categories = GetCategoriesList();
                return View(product);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User, Colaborator, Admin")]
        public ActionResult Rate( Rating rating)
        {
            string currentUser = User.Identity.GetUserId();
            int ratedProduct = rating.ProductId; 
            
            try
            {
                bool alreadyRated = false;
 
                foreach ( Rating x in db.Ratings)
                {
                    if( x.ProductId == ratedProduct && x.UserId == currentUser)
                    {
                        alreadyRated = true;
                        TempData["AlreadyRated"] = "You have already rated this product.";
                        Redirect("/Products/Show/" + ratedProduct);
                    }
                }
                if( !alreadyRated)
                {
                    Rating mark = new Rating();
                    mark.UserId = currentUser;
                    mark.ProductId = ratedProduct;
                    mark.Mark = rating.Mark;

                    db.Ratings.Add(mark);
                    db.SaveChanges();
                    TempData["message"] = "You have rated this product!";

                    float overallRating = 0;
                    int nr = 0;

                    foreach (Rating x in db.Ratings)
                    {
                        if (x.ProductId == ratedProduct)
                        {
                            overallRating += x.Mark;
                            nr++;
                        }
                    }

                    ProductRating productRating = db.ProductRatings.Find(ratedProduct);
                    
                    if (TryUpdateModel(productRating))
                    {
                        productRating.Rating = overallRating / nr;
                        db.SaveChanges();
                    }
                    
                }
            }
            catch ( Exception e)
            {
                return Redirect("/Products/Show/" + ratedProduct);
            }

            return Redirect("/Products/Show/" + ratedProduct);
        }

        [HttpDelete]
        [Authorize(Roles = "Colaborator, Admin")]
        public ActionResult Delete ( int id)
        {
            Product product = db.Products.Find(id);

            if (User.Identity.GetUserId() == product.UserId || User.IsInRole("Admin"))
            {
                System.IO.File.Delete(Server.MapPath("~/Files/" + product.FileName));
                db.Products.Remove(product);
                db.SaveChanges();

                Accepted accepted = db.Accepteds.Where(a => a.IdProduct == id).FirstOrDefault();
                db.Accepteds.Remove(accepted);
                db.SaveChanges();


                TempData["message"] = "Product deleted!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "You can't delete the product.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        //[Authorize(Roles = "User, Colaborator, Admin")]
        public ActionResult AddToCart(int Id, int amount)
        {
            if (User.IsInRole("Admin") || User.IsInRole("Colaborator") || User.IsInRole("User"))
            {
                string CurrentUserId = User.Identity.GetUserId();

                CartProduct cartProduct = new CartProduct();

                cartProduct.CartProductId = CurrentUserId + Id.ToString();
                cartProduct.CartId = CurrentUserId;
                cartProduct.IdProduct = Id;
                cartProduct.Amount = amount;

                //System.Diagnostics.Debug.WriteLine(cartProduct.CartProductId + "\n" + cartProduct.CartId + "\n"
                //     + cartProduct.IdProduct + "\n" + cartProduct.Amount + "\n"); ////

                try
                {
                    if (ModelState.IsValid)
                    {
                        //System.Diagnostics.Debug.WriteLine("Am reusit"); ////

                        db.CartProducts.Add(cartProduct);
                        db.SaveChanges();
                        TempData["message"] = "Product added to cart!";
                        return Redirect("/Products/Index");
                    }
                    else
                    {
                        //System.Diagnostics.Debug.WriteLine("Model invalid"); ////

                        TempData["message"] = "Product not added to cart!";
                        return Redirect("/Products/Show/" + Id);
                    }
                }
                catch (Exception e)
                {
                    //System.Diagnostics.Debug.WriteLine("Exceptie"); ////

                    return Redirect("/Products/Show/" + Id);
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        [HttpPut]
        [Authorize(Roles = "Admin")]
        public ActionResult AcceptProduct(int Id)
        {
            Accepted accepted = db.Accepteds.Where(a => a.IdProduct == Id).FirstOrDefault();

            accepted.Verified = true;
            db.SaveChanges();

            //System.Diagnostics.Debug.WriteLine("Am gasit"); ////

            return Redirect("/Products/Index");
        }
    }

}