using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Web.Mvc;

namespace GIFanpage.Models
{
    public class QuizCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QuizCategoryID { get; set; }

        [DisplayName("Category Name")]
        public string QuizCategoryName { get; set; }

        public string QuizCategoryPassword { get; set; }

        public int UserID { get; set; }

        public virtual User User { get; set; }


    }
}
