using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UCMS.Models.Domain;

namespace UCMS.Data
{
    public class UCMSDbContext : DbContext { 
        public UCMSDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<StudentRegistration> StudentRegistrations { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<VenueBooking> VenueBookings { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<SubjectAssign> SubjectAssigns { get; set; }
        public DbSet<ProfessorAssign> ProfessorAssigns { get; set; }
        public DbSet<StudentLectureEnrollment> StudentLectureEnrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentLectureEnrollment>()
            .HasKey(sle => new { sle.StudentId, sle.LectureId });

            modelBuilder.Entity<StudentLectureEnrollment>()
                .HasOne(sle => sle.Student)
                .WithMany(student => student.LectureEnrollments)
                .HasForeignKey(sle => sle.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StudentLectureEnrollment>()
                .HasOne(sle => sle.Lecture)
                .WithMany(lecture => lecture.StudentEnrollments)
                .HasForeignKey(sle => sle.LectureId)
            .OnDelete(DeleteBehavior.NoAction);
            // Define relationships using data annotations
            modelBuilder.Entity<StudentRegistration>()
        .HasOne(sr => sr.User)
        .WithMany()
        .HasForeignKey(sr => sr.UserId)
        .OnDelete(DeleteBehavior.NoAction); // Specify NO ACTION

            modelBuilder.Entity<StudentRegistration>()
                .HasOne(sr => sr.Course)
                .WithMany()
                .HasForeignKey(sr => sr.CourseId)
                .OnDelete(DeleteBehavior.NoAction); // Specify NO ACTION

            modelBuilder.Entity<StudentRegistration>()
                .HasOne(sr => sr.Semester)
                .WithMany()
                .HasForeignKey(sr => sr.SemesterId)
                .OnDelete(DeleteBehavior.NoAction); // Specify NO ACTION

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
                .HasOne(sa => sa.Subject)
                .WithMany(s => s.SubjectAssigns)
                .HasForeignKey(sa => sa.SubjectId);


            // ProfessorAssign to Semester, Professor, and Subject
            modelBuilder.Entity<ProfessorAssign>()
                .HasOne(sa => sa.Semester)
                .WithMany(s => s.ProfessorAssigns)
                .HasForeignKey(sa => sa.SemesterId);

            modelBuilder.Entity<ProfessorAssign>()
                .HasOne(sa => sa.Professor)
                .WithMany(u => u.ProfessorAssigns)
                .HasForeignKey(sa => sa.ProfessorId);

            modelBuilder.Entity<ProfessorAssign>()
                .HasOne(sa => sa.Subject)
                .WithMany(s => s.ProfessorAssigns)
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

            modelBuilder.Entity<Venue>()
                .HasMany(v => v.VenuesBooking)
                .WithOne(vb => vb.Venue)
                .HasForeignKey(vb => vb.VenueId);

            modelBuilder.Entity<Professor>()
    .HasMany(p => p.VenueBookings)
    .WithOne(vb => vb.Professor)
    .HasForeignKey(vb => vb.ProfessorId);

            modelBuilder.Entity<VenueBooking>()
        .HasOne(vb => vb.Professor)
        .WithMany(p => p.VenueBookings)
        .HasForeignKey(vb => vb.ProfessorId);

            modelBuilder.Entity<VenueBooking>()
                .HasOne(vb => vb.Venue)
                .WithMany(v => v.VenuesBooking)
                .HasForeignKey(vb => vb.VenueId);


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
                .HasMany(sm => sm.StudentRegistration)
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
                .HasMany(sm => sm.StudentRegistration)
                .WithOne(s => s.Semester)
                .HasForeignKey(s => s.SemesterId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
