using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Proiect.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proiect.Models
{
    public class Product
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Key]
        public int IdProduct { get; set; }
        [Required(ErrorMessage="The name is required")]
        public string ProductName { get; set; }
        [Required(ErrorMessage= "The price is required")][Range(0.01, float.MaxValue, ErrorMessage ="The price nu este valid")]
        public float Price { get; set; }
        public string Description { get; set; }

        
        public string FileName { get; set; }
        //public string FilePath { get; set; }
        //public string Extension { get; set; }

        [NotMapped][Required(ErrorMessage ="The image is required")]
        public HttpPostedFileBase Image { get; set; }

        [Required(ErrorMessage = "The category is required")]
        public int IdCategory { get; set; }

        public virtual Category Category { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }

        public virtual ProductRating ProductRating { get; set; }

        //cos de cumparaturi
        public virtual ICollection<CartProduct> CartElements { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}