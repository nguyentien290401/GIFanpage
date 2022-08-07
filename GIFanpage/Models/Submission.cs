using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace GIFanpage.Models
{
    public class Submission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubmissionID { get; set; }

        [DisplayName("Submisstion Name")]
        public string SubmissionName { get; set; }

        [DisplayName("Submisstion Description")]
        public string SubmissionDescription { get; set; }

        [DisplayName("Submisstion Close Date")]
        public DateTime CloseDate { get; set; }

        [DisplayName("Submisstion Final Date")]
        public DateTime FinalDate { get; set; }
    }
}