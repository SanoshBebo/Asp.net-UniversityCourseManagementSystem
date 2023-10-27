﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UCMS.Data;

#nullable disable

namespace UCMS.Migrations
{
    [DbContext(typeof(UCMSDbContext))]
    partial class UCMSDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("UCMS.Models.Domain.Course", b =>
                {
                    b.Property<Guid>("CourseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("CourseDurationInYears")
                        .HasColumnType("int");

                    b.Property<string>("CourseName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CourseId");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("UCMS.Models.Domain.Lecture", b =>
                {
                    b.Property<Guid>("LectureId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("LectureName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ProfessorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SemesterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Series")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("SubjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("VenueId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("LectureId");

                    b.HasIndex("ProfessorId");

                    b.HasIndex("SemesterId");

                    b.HasIndex("SubjectId");

                    b.HasIndex("UserId");

                    b.HasIndex("VenueId");

                    b.ToTable("Lectures");
                });

            modelBuilder.Entity("UCMS.Models.Domain.Professor", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("ExperienceInYears")
                        .HasColumnType("int");

                    b.Property<string>("ProfessorName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Professors");
                });

            modelBuilder.Entity("UCMS.Models.Domain.ProfessorAssign", b =>
                {
                    b.Property<Guid>("ProfessorAssignId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ProfessorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SemesterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SubjectId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ProfessorAssignId");

                    b.HasIndex("ProfessorId");

                    b.HasIndex("SemesterId");

                    b.HasIndex("SubjectId");

                    b.ToTable("ProfessorAssigns");
                });

            modelBuilder.Entity("UCMS.Models.Domain.Semester", b =>
                {
                    b.Property<Guid>("SemesterId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CourseId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SemesterName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SemesterId");

                    b.HasIndex("CourseId");

                    b.ToTable("Semesters");
                });

            modelBuilder.Entity("UCMS.Models.Domain.Student", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Batch")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("CourseId")
                        .IsRequired()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("SemesterId")
                        .IsRequired()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("StudentName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.HasIndex("CourseId");

                    b.HasIndex("SemesterId");

                    b.ToTable("Students");
                });

            modelBuilder.Entity("UCMS.Models.Domain.Subject", b =>
                {
                    b.Property<Guid>("SubjectId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SubjectName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TeachingHours")
                        .HasColumnType("int");

                    b.HasKey("SubjectId");

                    b.ToTable("Subjects");
                });

            modelBuilder.Entity("UCMS.Models.Domain.SubjectAssign", b =>
                {
                    b.Property<Guid>("SubjectAssignId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ProfessorUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SemesterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("StudentUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SubjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("SubjectAssignId");

                    b.HasIndex("ProfessorUserId");

                    b.HasIndex("SemesterId");

                    b.HasIndex("StudentUserId");

                    b.HasIndex("SubjectId");

                    b.HasIndex("UserId");

                    b.ToTable("SubjectAssigns");
                });

            modelBuilder.Entity("UCMS.Models.Domain.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("UCMS.Models.Domain.Venue", b =>
                {
                    b.Property<Guid>("VenueId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("VenueLocation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VenueName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("VenueId");

                    b.ToTable("Venues");
                });

            modelBuilder.Entity("UCMS.Models.Domain.Lecture", b =>
                {
                    b.HasOne("UCMS.Models.Domain.Professor", "Professor")
                        .WithMany("Lectures")
                        .HasForeignKey("ProfessorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UCMS.Models.Domain.Semester", "Semester")
                        .WithMany("Lectures")
                        .HasForeignKey("SemesterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UCMS.Models.Domain.Subject", "Subject")
                        .WithMany("Lectures")
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UCMS.Models.Domain.User", null)
                        .WithMany("Lectures")
                        .HasForeignKey("UserId");

                    b.HasOne("UCMS.Models.Domain.Venue", "Venue")
                        .WithMany("Lectures")
                        .HasForeignKey("VenueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Professor");

                    b.Navigation("Semester");

                    b.Navigation("Subject");

                    b.Navigation("Venue");
                });

            modelBuilder.Entity("UCMS.Models.Domain.Professor", b =>
                {
                    b.HasOne("UCMS.Models.Domain.User", "User")
                        .WithOne("Professor")
                        .HasForeignKey("UCMS.Models.Domain.Professor", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("UCMS.Models.Domain.ProfessorAssign", b =>
                {
                    b.HasOne("UCMS.Models.Domain.Professor", "Professor")
                        .WithMany("ProfessorAssigns")
                        .HasForeignKey("ProfessorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UCMS.Models.Domain.Semester", "Semester")
                        .WithMany("ProfessorAssigns")
                        .HasForeignKey("SemesterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UCMS.Models.Domain.Subject", "Subject")
                        .WithMany("ProfessorAssigns")
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Professor");

                    b.Navigation("Semester");

                    b.Navigation("Subject");
                });

            modelBuilder.Entity("UCMS.Models.Domain.Semester", b =>
                {
                    b.HasOne("UCMS.Models.Domain.Course", "Course")
                        .WithMany("Semesters")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");
                });

            modelBuilder.Entity("UCMS.Models.Domain.Student", b =>
                {
                    b.HasOne("UCMS.Models.Domain.Course", "Course")
                        .WithMany("Students")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UCMS.Models.Domain.Semester", "Semester")
                        .WithMany("Students")
                        .HasForeignKey("SemesterId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("UCMS.Models.Domain.User", "User")
                        .WithOne("Student")
                        .HasForeignKey("UCMS.Models.Domain.Student", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("Semester");

                    b.Navigation("User");
                });

            modelBuilder.Entity("UCMS.Models.Domain.SubjectAssign", b =>
                {
                    b.HasOne("UCMS.Models.Domain.Professor", null)
                        .WithMany("SubjectAssigns")
                        .HasForeignKey("ProfessorUserId");

                    b.HasOne("UCMS.Models.Domain.Semester", "Semester")
                        .WithMany("SubjectAssigns")
                        .HasForeignKey("SemesterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UCMS.Models.Domain.Student", null)
                        .WithMany("SubjectAssigns")
                        .HasForeignKey("StudentUserId");

                    b.HasOne("UCMS.Models.Domain.Subject", "Subject")
                        .WithMany("SubjectAssigns")
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UCMS.Models.Domain.User", null)
                        .WithMany("SubjectAssigns")
                        .HasForeignKey("UserId");

                    b.Navigation("Semester");

                    b.Navigation("Subject");
                });

            modelBuilder.Entity("UCMS.Models.Domain.Course", b =>
                {
                    b.Navigation("Semesters");

                    b.Navigation("Students");
                });

            modelBuilder.Entity("UCMS.Models.Domain.Professor", b =>
                {
                    b.Navigation("Lectures");

                    b.Navigation("ProfessorAssigns");

                    b.Navigation("SubjectAssigns");
                });

            modelBuilder.Entity("UCMS.Models.Domain.Semester", b =>
                {
                    b.Navigation("Lectures");

                    b.Navigation("ProfessorAssigns");

                    b.Navigation("Students");

                    b.Navigation("SubjectAssigns");
                });

            modelBuilder.Entity("UCMS.Models.Domain.Student", b =>
                {
                    b.Navigation("SubjectAssigns");
                });

            modelBuilder.Entity("UCMS.Models.Domain.Subject", b =>
                {
                    b.Navigation("Lectures");

                    b.Navigation("ProfessorAssigns");

                    b.Navigation("SubjectAssigns");
                });

            modelBuilder.Entity("UCMS.Models.Domain.User", b =>
                {
                    b.Navigation("Lectures");

                    b.Navigation("Professor")
                        .IsRequired();

                    b.Navigation("Student")
                        .IsRequired();

                    b.Navigation("SubjectAssigns");
                });

            modelBuilder.Entity("UCMS.Models.Domain.Venue", b =>
                {
                    b.Navigation("Lectures");
                });
#pragma warning restore 612, 618
        }
    }
}
