using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proiect.Models
{
    public class CartProduct
    {
        //[Key]
        public string CartProductId { get; set; }

        //[Required]
        public string CartId { get; set; }
        //[Required]
        public int IdProduct { get; set; }

        [Required]
        public int Amount { get; set; }

        public virtual Product Product { get; set; }
        public virtual Cart Cart { get; set; }
    }
}