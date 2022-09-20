using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GIFanpage.Models
{
    public class SubComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubCommentID { get; set; }
        public string Content { get; set; }
        public DateTime SubCommentDate { get; set; }
      
        public int CommentID { get; set; }
        public int UserID { get; set; }
        

        [ForeignKey("UserID")]
        public virtual User User { get; set; }
       

    }
}