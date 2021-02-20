using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Proiect.Models
{
    public class ProductRating
    {
        [Key]
        public int IdProduct { get; set; }
        public float Rating { get; set; }


    }
}