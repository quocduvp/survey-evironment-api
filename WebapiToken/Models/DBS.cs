namespace WebapiToken.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DBS : DbContext
    {
        public DBS()
            : base("name=DBS")
        {
        }

        public virtual DbSet<account> accounts { get; set; }
        public virtual DbSet<admin> admins { get; set; }
        public virtual DbSet<classroom> classrooms { get; set; }
        public virtual DbSet<detail> details { get; set; }
        public virtual DbSet<details_admin> details_admin { get; set; }
        public virtual DbSet<faculty> faculties { get; set; }
        public virtual DbSet<faq> faqs { get; set; }
        public virtual DbSet<feedback> feedbacks { get; set; }
        public virtual DbSet<question> questions { get; set; }
        public virtual DbSet<question_choice> question_choice { get; set; }
        public virtual DbSet<question_choice_response> question_choice_response { get; set; }
        public virtual DbSet<question_text> question_text { get; set; }
        public virtual DbSet<question_text_response> question_text_response { get; set; }
        public virtual DbSet<report_account> report_account { get; set; }
        public virtual DbSet<role> roles { get; set; }
        public virtual DbSet<survey> surveys { get; set; }
        public virtual DbSet<surveys_response> surveys_response { get; set; }
        public virtual DbSet<surveys_type> surveys_type { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<account>()
                .HasMany(e => e.details)
                .WithOptional(e => e.account)
                .HasForeignKey(e => e.account_id);

            modelBuilder.Entity<account>()
                .HasMany(e => e.report_account)
                .WithOptional(e => e.account)
                .HasForeignKey(e => e.account_id);

            modelBuilder.Entity<admin>()
                .HasMany(e => e.details_admin)
                .WithOptional(e => e.admin)
                .HasForeignKey(e => e.admin_id);

            modelBuilder.Entity<classroom>()
                .HasMany(e => e.details)
                .WithOptional(e => e.classroom)
                .HasForeignKey(e => e.classroom_id);

            modelBuilder.Entity<faculty>()
                .HasMany(e => e.classrooms)
                .WithOptional(e => e.faculty)
                .HasForeignKey(e => e.faculty_id);

            modelBuilder.Entity<faq>()
                .Property(e => e.body)
                .IsUnicode(false);

            modelBuilder.Entity<feedback>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<question>()
                .HasMany(e => e.question_choice)
                .WithOptional(e => e.question)
                .HasForeignKey(e => e.question_id);

            modelBuilder.Entity<question>()
                .HasMany(e => e.question_text)
                .WithOptional(e => e.question)
                .HasForeignKey(e => e.question_id);

            modelBuilder.Entity<role>()
                .HasMany(e => e.accounts)
                .WithOptional(e => e.role)
                .HasForeignKey(e => e.role_id);

            modelBuilder.Entity<role>()
                .HasMany(e => e.admins)
                .WithOptional(e => e.role)
                .HasForeignKey(e => e.role_id);

            modelBuilder.Entity<survey>()
                .HasMany(e => e.questions)
                .WithOptional(e => e.survey)
                .HasForeignKey(e => e.surveys_id);

            modelBuilder.Entity<survey>()
                .HasMany(e => e.surveys_response)
                .WithOptional(e => e.survey)
                .HasForeignKey(e => e.surveys_id);

            modelBuilder.Entity<surveys_response>()
                .HasMany(e => e.question_choice_response)
                .WithOptional(e => e.surveys_response)
                .HasForeignKey(e => e.surveys_response_id);

            modelBuilder.Entity<surveys_response>()
                .HasMany(e => e.question_text_response)
                .WithOptional(e => e.surveys_response)
                .HasForeignKey(e => e.surveys_response_id);

            modelBuilder.Entity<surveys_type>()
                .HasMany(e => e.surveys)
                .WithOptional(e => e.surveys_type)
                .HasForeignKey(e => e.surveys_type_id);
        }
    }
}
