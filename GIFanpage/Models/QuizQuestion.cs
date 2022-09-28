using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GIFanpage.Models
{
    public class QuizQuestion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QuizQuestionID { get; set; }

        [Display(Name = "Quiz question name")]
        public string QuizQuestionContent { get; set; }

        [Display(Name = "Question A")]
        public string QA { get; set; }

        [Display(Name = "Question B")]
        public string QB { get; set; }

        [Display(Name = "Question C")]
        public string QC { get; set; }

        [Display(Name = "Question D")]
        public string QD { get; set; }

        [Display(Name = "Correct Answer")]
        public string CorrectAnswer { get; set; }

        public int QuizCategoryID { get; set; }

        public virtual QuizCategory QuizCategory { get; set; }

    }
}