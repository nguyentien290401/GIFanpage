using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace GIFanpage.Models
{
    public class Playstyle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlaystyleID { get; set; }

        [DisplayName("Playstyle Name")]
        public string PlaystyleName { get; set; }
    }
}