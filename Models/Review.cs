using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proiect.Models
{
    public class Review
    {
        [Key]
        public int IdReview { get; set; }
        [Required(ErrorMessage ="This field is required")]
        public string Content { get; set; }
        public DateTime Date { get; set; }
        //[Required]
        //public int IdUser { get; set; }
        public int IdProduct { get; set; }

        public virtual Product Products { get; set; }


        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}