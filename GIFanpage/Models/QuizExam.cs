using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GIFanpage.Models
{
    public class QuizExam
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QuizExamID { get; set; }

        [Display(Name = "Quiz Question Content")]
        public string QuizExamContent { get; set; }

        [Display(Name = "Create Date")]
        public DateTime QuizExamDate { get; set; }

        [Display(Name = "Score")]
        public int QuizExamScore { get; set; }

        public int UserID { get; set; }

        public virtual User User { get; set; }
    }
}