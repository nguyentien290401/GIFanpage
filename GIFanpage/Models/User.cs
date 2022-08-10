using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace GIFanpage.Models
{
    public class User
    {
        public User()
        {
            UserImg = "~/Content/Image/add.png";
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [DisplayName("Password")]
        public string PasswordHash { get; set; }

        [Required]
        [DisplayName("Image")]
        public string UserImg { get; set; }

        [DisplayName("Play-style Name")]
        public int PlaystyleID { get; set; }

        [DisplayName("Role Name")]
        public int RoleID { get; set; }

        [ForeignKey("PlaystyleID")]
        public virtual Playstyle Playstyle { get; set; }

        [ForeignKey("RoleID")]
        public virtual Role Role { get; set; }

        [NotMapped]
        public HttpPostedFileBase ImageUpload { get; set; }
    }
}