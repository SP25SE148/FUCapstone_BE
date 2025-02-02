﻿// <auto-generated />
using System;
using FUC.Data.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FUC.Data.Migrations
{
    [DbContext(typeof(FucDbContext))]
    partial class FucDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FUC.Data.Entities.Campus", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Campus", (string)null);
                });

            modelBuilder.Entity("FUC.Data.Entities.Capstone", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("MajorId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("MaxMember")
                        .HasColumnType("integer");

                    b.Property<int>("MinMember")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ReviewCount")
                        .HasColumnType("integer");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("MajorId");

                    b.ToTable("Capstone", (string)null);
                });

            modelBuilder.Entity("FUC.Data.Entities.Group", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CampusId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CapstoneId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("GroupCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("MajorId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SemesterId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TopicCode")
                        .HasColumnType("text");

                    b.Property<Guid?>("TopicId")
                        .HasColumnType("uuid");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("CampusId");

                    b.HasIndex("CapstoneId");

                    b.HasIndex("MajorId");

                    b.HasIndex("SemesterId");

                    b.ToTable("Group", (string)null);
                });

            modelBuilder.Entity("FUC.Data.Entities.GroupMember", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsLeader")
                        .HasColumnType("boolean");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("StudentId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.HasIndex("StudentId")
                        .IsUnique();

                    b.ToTable("GroupMember", (string)null);
                });

            modelBuilder.Entity("FUC.Data.Entities.Major", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("MajorGroupId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("MajorGroupId");

                    b.ToTable("Major", (string)null);
                });

            modelBuilder.Entity("FUC.Data.Entities.MajorGroup", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("MajorGroup", (string)null);
                });

            modelBuilder.Entity("FUC.Data.Entities.Semester", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Semester", (string)null);
                });

            modelBuilder.Entity("FUC.Data.Entities.Student", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("CampusId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CapstoneId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsEligible")
                        .HasColumnType("boolean");

                    b.Property<string>("MajorId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("CampusId");

                    b.HasIndex("MajorId");

                    b.ToTable("Student", (string)null);
                });

            modelBuilder.Entity("FUC.Data.Entities.Capstone", b =>
                {
                    b.HasOne("FUC.Data.Entities.Major", "Major")
                        .WithMany("Capstones")
                        .HasForeignKey("MajorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Major");
                });

            modelBuilder.Entity("FUC.Data.Entities.Group", b =>
                {
                    b.HasOne("FUC.Data.Entities.Campus", "Campus")
                        .WithMany("Groups")
                        .HasForeignKey("CampusId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("FUC.Data.Entities.Capstone", "Capstone")
                        .WithMany("Groups")
                        .HasForeignKey("CapstoneId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("FUC.Data.Entities.Major", "Major")
                        .WithMany("Groups")
                        .HasForeignKey("MajorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("FUC.Data.Entities.Semester", "Semester")
                        .WithMany("Groups")
                        .HasForeignKey("SemesterId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Campus");

                    b.Navigation("Capstone");

                    b.Navigation("Major");

                    b.Navigation("Semester");
                });

            modelBuilder.Entity("FUC.Data.Entities.GroupMember", b =>
                {
                    b.HasOne("FUC.Data.Entities.Group", "Group")
                        .WithMany("GroupMembers")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("FUC.Data.Entities.Student", "Student")
                        .WithOne("GroupMember")
                        .HasForeignKey("FUC.Data.Entities.GroupMember", "StudentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("FUC.Data.Entities.Major", b =>
                {
                    b.HasOne("FUC.Data.Entities.MajorGroup", "MajorGroup")
                        .WithMany("Majors")
                        .HasForeignKey("MajorGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MajorGroup");
                });

            modelBuilder.Entity("FUC.Data.Entities.Student", b =>
                {
                    b.HasOne("FUC.Data.Entities.Campus", "Campus")
                        .WithMany("Students")
                        .HasForeignKey("CampusId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("FUC.Data.Entities.Capstone", "Capstone")
                        .WithMany("Students")
                        .HasForeignKey("CampusId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("FUC.Data.Entities.Major", "Major")
                        .WithMany("Students")
                        .HasForeignKey("MajorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Campus");

                    b.Navigation("Capstone");

                    b.Navigation("Major");
                });

            modelBuilder.Entity("FUC.Data.Entities.Campus", b =>
                {
                    b.Navigation("Groups");

                    b.Navigation("Students");
                });

            modelBuilder.Entity("FUC.Data.Entities.Capstone", b =>
                {
                    b.Navigation("Groups");

                    b.Navigation("Students");
                });

            modelBuilder.Entity("FUC.Data.Entities.Group", b =>
                {
                    b.Navigation("GroupMembers");
                });

            modelBuilder.Entity("FUC.Data.Entities.Major", b =>
                {
                    b.Navigation("Capstones");

                    b.Navigation("Groups");

                    b.Navigation("Students");
                });

            modelBuilder.Entity("FUC.Data.Entities.MajorGroup", b =>
                {
                    b.Navigation("Majors");
                });

            modelBuilder.Entity("FUC.Data.Entities.Semester", b =>
                {
                    b.Navigation("Groups");
                });

            modelBuilder.Entity("FUC.Data.Entities.Student", b =>
                {
                    b.Navigation("GroupMember")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
