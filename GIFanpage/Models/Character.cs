using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace GIFanpage.Models
{
    public class Character
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CharacterID { get; set; }
        public string CharacterName { get; set; }
        public string CharacterVision { get; set; }

        [AllowHtml]
        public string CharacterDescription { get; set; }
        public string CharacterRarity { get; set; }
        public string CharacterRegion { get; set; }
        public DateTime CharacterBirthday { get; set; }
        public string CharacterImageCard { get; set; }
        public string CharacterImageOriginal { get; set; }
    }
}