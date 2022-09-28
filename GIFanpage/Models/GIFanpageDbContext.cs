using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace GIFanpage.Models
{
    public class GIFanpageDbContext : DbContext
    {
        public GIFanpageDbContext() : base("GIFanpageDbContext")
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<SubComment> SubComments { get; set; }
        public DbSet<Playstyle> Playstyles { get; set; }
        public DbSet<Ask> Asks { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<New> News { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<QuizCategory> QuizCategories { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<QuizExam> QuizExams { get; set; }
    }
}