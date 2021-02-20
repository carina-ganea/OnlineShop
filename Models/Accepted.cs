using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proiect.Models
{
    public class Accepted
    {
        //public string ProdId { get; set; }
        [Key]
        public int IdP { get; set; }

        public int IdProduct { get; set; }

        public bool Verified { get; set; } = false;

        //public virtual Product Product { get; set; }
    }
}