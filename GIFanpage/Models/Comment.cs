using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace GIFanpage.Models
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentID { get; set; }

        [AllowHtml]
        public string Content { get; set; }
        public DateTime CommentDate { get; set; }
        public int SubCommentCount { get; set; }
        public int CurrentUserVoteType { get; set; }
        public int VotesCount { get; set; }
        public int AskID { get; set; }
        public int UserID { get; set; }
        public Boolean IsTrue { get; set; }
        public Boolean IsHidden { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }
        public virtual List<Vote> Votes { get; set; }
        public virtual List<SubComment> SubComments { get; set; }

    }
}