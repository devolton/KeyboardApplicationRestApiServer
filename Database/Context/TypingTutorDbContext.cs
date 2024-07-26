using KeyboardApplicationRestApiServer.Database.Entities;
using KeyboardApplicationRestApiServer.Shared.Tools;
using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace KeyboardApplicationRestApiServer.Database.Context
{

    public class TypingTutorDbContext : DbContext
    {
        private const string _DB_CONNECTION_STRING = "TypingTutorDb";
        public DbSet<User> Users { get; set; }
        public DbSet<TypingTestResult> TypingTestResults { get; set; }
        public DbSet<EnglishLayoutLesson> EnglishLayoutLessons { get; set; }
        public DbSet<EnglishLayoutLevel> EnglishLayoutLevels { get; set; }
        public DbSet<EducationUsersProgress> EducationUsersProgresses { get; set; }
        public DbSet<EnglishTypingTestText> EnglishTypingTestTexts { get; set; }

        public TypingTutorDbContext(string connectionStr) : base(connectionStr)
        {

        }
        public TypingTutorDbContext() : base($"name={_DB_CONNECTION_STRING}")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();


        }


    }
}
