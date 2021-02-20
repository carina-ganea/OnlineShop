using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proiect.Models
{
    public class Cart
    {
        [Key]
        public string CartId { get; set; }


        public string UserId { get; set; }


        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<CartProduct> CartProducts { get; set; }
    }
}