using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proiect.Models
{
    public class Category
    {
        [Key]
        public int IdCategory { get; set; }
        [Required(ErrorMessage ="The name is required")]
        public string CategoryName { get; set; }

        public virtual ICollection<Product> Products { get; set; }

    }
}