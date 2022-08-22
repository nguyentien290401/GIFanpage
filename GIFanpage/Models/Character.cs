using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GIFanpage.Models
{
    public class Character
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CharacterID { get; set; }
        public string CharacterName { get; set; }
        public string CharacterVision { get; set; }
        public string CharacterDescription { get; set; }
        public string CharacterImage { get; set; }
    }
}