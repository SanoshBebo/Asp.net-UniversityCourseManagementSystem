using Microsoft.EntityFrameworkCore;
using UCMS.Models.Domain;

namespace UCMS.Data
{
    public class UCMSDbContext : DbContext
    {
        public UCMSDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<SubjectAssign> SubjectAssigns { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define the relationships between entities

            // Student to Course and Semester
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Course)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.CourseId);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Semester)
                .WithMany(sm => sm.Students)
                .HasForeignKey(s => s.SemesterId);

            // Professor to User
            modelBuilder.Entity<Professor>()
                .HasOne(p => p.User)
                .WithOne(u => u.Professor)
                .HasForeignKey<Professor>(p => p.UserId);

            // Student to User
            modelBuilder.Entity<Student>()
                .HasOne(s => s.User)
                .WithOne(u => u.Student)
                .HasForeignKey<Student>(s => s.UserId);

            // SubjectAssign to Semester, Professor, and Subject
            modelBuilder.Entity<SubjectAssign>()
                .HasOne(sa => sa.Semester)
                .WithMany(s => s.SubjectAssigns)
                .HasForeignKey(sa => sa.SemesterId);

            modelBuilder.Entity<SubjectAssign>()
                .HasOne(sa => sa.Professor)
                .WithMany(u => u.SubjectAssigns)
                .HasForeignKey(sa => sa.ProfessorId);

            modelBuilder.Entity<SubjectAssign>()
                .HasOne(sa => sa.Subject)
                .WithMany(s => s.SubjectAssigns)
                .HasForeignKey(sa => sa.SubjectId);

            // Lecture to Venue, Subject, Semester, and Professor
            modelBuilder.Entity<Lecture>()
                .HasOne(l => l.Venue)
                .WithMany(v => v.Lectures)
                .HasForeignKey(l => l.VenueId);

            modelBuilder.Entity<Lecture>()
                .HasOne(l => l.Subject)
                .WithMany(s => s.Lectures)
                .HasForeignKey(l => l.SubjectId);

            modelBuilder.Entity<Lecture>()
                .HasOne(l => l.Semester)
                .WithMany(sm => sm.Lectures)
                .HasForeignKey(l => l.SemesterId);

            modelBuilder.Entity<Lecture>()
                .HasOne(l => l.Professor)
                .WithMany(p => p.Lectures)
                .HasForeignKey(l => l.ProfessorId);

            // Venue to Lecture
            modelBuilder.Entity<Venue>()
                .HasMany(v => v.Lectures)
                .WithOne(l => l.Venue)
                .HasForeignKey(l => l.VenueId);

            // Subject to SubjectAssign and Lecture
            modelBuilder.Entity<Subject>()
                .HasMany(s => s.SubjectAssigns)
                .WithOne(sa => sa.Subject)
                .HasForeignKey(sa => sa.SubjectId);

            modelBuilder.Entity<Subject>()
                .HasMany(s => s.Lectures)
                .WithOne(l => l.Subject)
                .HasForeignKey(l => l.SubjectId);

            // Course to Semester and Student
            modelBuilder.Entity<Course>()
                .HasMany(c => c.Semesters)
                .WithOne(sm => sm.Course)
                .HasForeignKey(sm => sm.CourseId);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Students)
                .WithOne(s => s.Course)
                .HasForeignKey(s => s.CourseId);

            // Semester to SubjectAssign, Lecture, and Student
            modelBuilder.Entity<Semester>()
                .HasMany(sm => sm.SubjectAssigns)
                .WithOne(sa => sa.Semester)
                .HasForeignKey(sa => sa.SemesterId);

            modelBuilder.Entity<Semester>()
                .HasMany(sm => sm.Lectures)
                .WithOne(l => l.Semester)
                .HasForeignKey(l => l.SemesterId);

            modelBuilder.Entity<Semester>()
                .HasMany(sm => sm.Students)
                .WithOne(s => s.Semester)
                .HasForeignKey(s => s.SemesterId);

            modelBuilder.Entity<Student>()
       .HasOne(s => s.Course)
       .WithMany(c => c.Students)
       .HasForeignKey(s => s.CourseId)
       .OnDelete(DeleteBehavior.Cascade); // Set cascade delete for CourseId

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Semester)
                .WithMany(sm => sm.Students)
                .HasForeignKey(s => s.SemesterId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
