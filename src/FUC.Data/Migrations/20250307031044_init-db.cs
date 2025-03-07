using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUC.Data.Migrations;

/// <inheritdoc />
public partial class initdb : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "BusinessArea",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("16dbe5cd-85c5-4d59-995b-c6206fd20419")),
                Name = table.Column<string>(type: "text", nullable: false),
                Description = table.Column<string>(type: "text", nullable: false),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BusinessArea", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Campus",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                Address = table.Column<string>(type: "text", nullable: false),
                Phone = table.Column<string>(type: "text", nullable: false),
                Email = table.Column<string>(type: "text", nullable: false),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Campus", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "IntegrationEventLogs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Type = table.Column<string>(type: "text", nullable: false),
                Content = table.Column<string>(type: "text", nullable: false),
                OccurredOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ProcessedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                Error = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_IntegrationEventLogs", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "MajorGroup",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                Description = table.Column<string>(type: "text", nullable: true),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MajorGroup", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Semester",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                MaxGroupsPerSupervisor = table.Column<int>(type: "integer", nullable: false),
                StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Semester", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "TemplateDocument",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("bab14576-b25b-428a-9e0f-c040f2edf488")),
                FileName = table.Column<string>(type: "text", nullable: false),
                FileUrl = table.Column<string>(type: "text", nullable: false),
                ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                IsFile = table.Column<bool>(type: "boolean", nullable: false),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                UpdatedBy = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TemplateDocument", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Major",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                MajorGroupId = table.Column<string>(type: "text", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                Description = table.Column<string>(type: "text", nullable: true),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Major", x => x.Id);
                table.ForeignKey(
                    name: "FK_Major_MajorGroup_MajorGroupId",
                    column: x => x.MajorGroupId,
                    principalTable: "MajorGroup",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Capstone",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                MajorId = table.Column<string>(type: "text", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                MinMember = table.Column<int>(type: "integer", nullable: false),
                MaxMember = table.Column<int>(type: "integer", nullable: false),
                ReviewCount = table.Column<int>(type: "integer", nullable: false),
                DurationWeeks = table.Column<int>(type: "integer", nullable: false),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Capstone", x => x.Id);
                table.ForeignKey(
                    name: "FK_Capstone_Major_MajorId",
                    column: x => x.MajorId,
                    principalTable: "Major",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Supervisor",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                FullName = table.Column<string>(type: "text", nullable: false),
                MajorId = table.Column<string>(type: "text", nullable: false),
                CampusId = table.Column<string>(type: "text", nullable: false),
                MaxGroupsInSemester = table.Column<int>(type: "integer", nullable: false),
                Email = table.Column<string>(type: "text", nullable: false),
                IsAvailable = table.Column<bool>(type: "boolean", nullable: false),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Supervisor", x => x.Id);
                table.ForeignKey(
                    name: "FK_Supervisor_Campus_CampusId",
                    column: x => x.CampusId,
                    principalTable: "Campus",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Supervisor_Major_MajorId",
                    column: x => x.MajorId,
                    principalTable: "Major",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Group",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("4738f073-5faa-4f0a-9208-d3a7fc6c2d5c")),
                SemesterId = table.Column<string>(type: "text", nullable: false),
                MajorId = table.Column<string>(type: "text", nullable: false),
                CampusId = table.Column<string>(type: "text", nullable: false),
                CapstoneId = table.Column<string>(type: "text", nullable: false),
                TopicCode = table.Column<string>(type: "text", nullable: true),
                GroupCode = table.Column<string>(type: "text", nullable: false),
                Status = table.Column<string>(type: "text", nullable: false, defaultValue: "Pending"),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Group", x => x.Id);
                table.ForeignKey(
                    name: "FK_Group_Campus_CampusId",
                    column: x => x.CampusId,
                    principalTable: "Campus",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Group_Capstone_CapstoneId",
                    column: x => x.CapstoneId,
                    principalTable: "Capstone",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Group_Major_MajorId",
                    column: x => x.MajorId,
                    principalTable: "Major",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Group_Semester_SemesterId",
                    column: x => x.SemesterId,
                    principalTable: "Semester",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Student",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                FullName = table.Column<string>(type: "text", nullable: false),
                MajorId = table.Column<string>(type: "text", nullable: false),
                CapstoneId = table.Column<string>(type: "text", nullable: false),
                CampusId = table.Column<string>(type: "text", nullable: false),
                BusinessAreaId = table.Column<Guid>(type: "uuid", nullable: true),
                Email = table.Column<string>(type: "text", nullable: false),
                Mark = table.Column<float>(type: "real", nullable: false),
                IsEligible = table.Column<bool>(type: "boolean", nullable: false),
                Status = table.Column<string>(type: "text", nullable: false, defaultValue: "InProgress"),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Student", x => x.Id);
                table.ForeignKey(
                    name: "FK_Student_BusinessArea_BusinessAreaId",
                    column: x => x.BusinessAreaId,
                    principalTable: "BusinessArea",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Student_Campus_CampusId",
                    column: x => x.CampusId,
                    principalTable: "Campus",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Student_Capstone_CapstoneId",
                    column: x => x.CapstoneId,
                    principalTable: "Capstone",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Student_Major_MajorId",
                    column: x => x.MajorId,
                    principalTable: "Major",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Topic",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("c2641343-e2f2-4671-813c-939ea97dd040")),
                MainSupervisorId = table.Column<string>(type: "text", nullable: false),
                CapstoneId = table.Column<string>(type: "text", nullable: false),
                SemesterId = table.Column<string>(type: "text", nullable: false),
                CampusId = table.Column<string>(type: "text", nullable: false),
                BusinessAreaId = table.Column<Guid>(type: "uuid", nullable: false),
                Code = table.Column<string>(type: "text", nullable: true),
                EnglishName = table.Column<string>(type: "text", nullable: false),
                VietnameseName = table.Column<string>(type: "text", nullable: false),
                Abbreviation = table.Column<string>(type: "text", nullable: false),
                Description = table.Column<string>(type: "text", nullable: false),
                FileName = table.Column<string>(type: "text", nullable: false),
                FileUrl = table.Column<string>(type: "text", nullable: false),
                Status = table.Column<string>(type: "text", nullable: false, defaultValue: "Pending"),
                IsAssignedToGroup = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                DifficultyLevel = table.Column<string>(type: "text", nullable: false),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Topic", x => x.Id);
                table.ForeignKey(
                    name: "FK_Topic_BusinessArea_BusinessAreaId",
                    column: x => x.BusinessAreaId,
                    principalTable: "BusinessArea",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Topic_Campus_CampusId",
                    column: x => x.CampusId,
                    principalTable: "Campus",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Topic_Capstone_CapstoneId",
                    column: x => x.CapstoneId,
                    principalTable: "Capstone",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Topic_Semester_SemesterId",
                    column: x => x.SemesterId,
                    principalTable: "Semester",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Topic_Supervisor_MainSupervisorId",
                    column: x => x.MainSupervisorId,
                    principalTable: "Supervisor",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "ProjectProgress",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                SupervisorId = table.Column<string>(type: "text", nullable: false),
                MeetingDate = table.Column<string>(type: "text", nullable: false),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProjectProgress", x => x.Id);
                table.ForeignKey(
                    name: "FK_ProjectProgress_Group_GroupId",
                    column: x => x.GroupId,
                    principalTable: "Group",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_ProjectProgress_Supervisor_SupervisorId",
                    column: x => x.SupervisorId,
                    principalTable: "Supervisor",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "SupervisorGroupAssignment",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("67d739a1-64f0-437e-a217-dddd79d03aba")),
                GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                SupervisorId = table.Column<string>(type: "text", nullable: false),
                Supervisor2Id = table.Column<string>(type: "text", nullable: true),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SupervisorGroupAssignment", x => x.Id);
                table.ForeignKey(
                    name: "FK_SupervisorGroupAssignment_Group_GroupId",
                    column: x => x.GroupId,
                    principalTable: "Group",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_SupervisorGroupAssignment_Supervisor_Supervisor2Id",
                    column: x => x.Supervisor2Id,
                    principalTable: "Supervisor",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_SupervisorGroupAssignment_Supervisor_SupervisorId",
                    column: x => x.SupervisorId,
                    principalTable: "Supervisor",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "GroupMember",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("fae04bd4-3220-4c4f-acd5-02e9168c4afd")),
                GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                StudentId = table.Column<string>(type: "text", nullable: false),
                IsLeader = table.Column<bool>(type: "boolean", nullable: false),
                Status = table.Column<string>(type: "text", nullable: false, defaultValue: "UnderReview"),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                UpdatedBy = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GroupMember", x => x.Id);
                table.ForeignKey(
                    name: "FK_GroupMember_Group_GroupId",
                    column: x => x.GroupId,
                    principalTable: "Group",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_GroupMember_Student_StudentId",
                    column: x => x.StudentId,
                    principalTable: "Student",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "CoSupervisor",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("1128c7bd-882c-4e0a-9c16-9b481a3ffff7")),
                SupervisorId = table.Column<string>(type: "text", nullable: false),
                TopicId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                UpdatedBy = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CoSupervisor", x => x.Id);
                table.ForeignKey(
                    name: "FK_CoSupervisor_Supervisor_SupervisorId",
                    column: x => x.SupervisorId,
                    principalTable: "Supervisor",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_CoSupervisor_Topic_TopicId",
                    column: x => x.TopicId,
                    principalTable: "Topic",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "TopicAnalysis",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("a78c7337-c82e-416e-8ca7-b7c1cd9cb407")),
                AnalysisResult = table.Column<string>(type: "text", nullable: false),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2025, 3, 7, 3, 10, 44, 173, DateTimeKind.Utc).AddTicks(1497)),
                ProcessedBy = table.Column<string>(type: "text", nullable: false),
                TopicId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TopicAnalysis", x => x.Id);
                table.ForeignKey(
                    name: "FK_TopicAnalysis_Topic_TopicId",
                    column: x => x.TopicId,
                    principalTable: "Topic",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "TopicAppraisal",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("f0ebd88c-9766-4fa9-9306-17bac94db7d2")),
                SupervisorId = table.Column<string>(type: "text", nullable: true),
                ManagerId = table.Column<string>(type: "text", nullable: true),
                TopicId = table.Column<Guid>(type: "uuid", nullable: false),
                AppraisalContent = table.Column<string>(type: "text", nullable: true),
                AppraisalComment = table.Column<string>(type: "text", nullable: true),
                Status = table.Column<string>(type: "text", nullable: false, defaultValue: "Pending"),
                AppraisalDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                UpdatedBy = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TopicAppraisal", x => x.Id);
                table.ForeignKey(
                    name: "FK_TopicAppraisal_Supervisor_SupervisorId",
                    column: x => x.SupervisorId,
                    principalTable: "Supervisor",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_TopicAppraisal_Topic_TopicId",
                    column: x => x.TopicId,
                    principalTable: "Topic",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "TopicRequest",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("72044bae-cd53-4e81-bddf-661da49c1281")),
                SupervisorId = table.Column<string>(type: "text", nullable: false),
                GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                TopicId = table.Column<Guid>(type: "uuid", nullable: false),
                Status = table.Column<string>(type: "text", nullable: false, defaultValue: "UnderReview"),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TopicRequest", x => x.Id);
                table.ForeignKey(
                    name: "FK_TopicRequest_Group_GroupId",
                    column: x => x.GroupId,
                    principalTable: "Group",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_TopicRequest_Supervisor_SupervisorId",
                    column: x => x.SupervisorId,
                    principalTable: "Supervisor",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_TopicRequest_Topic_TopicId",
                    column: x => x.TopicId,
                    principalTable: "Topic",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "ProjectProgressWeek",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ProjectProgressId = table.Column<Guid>(type: "uuid", nullable: false),
                WeekNumber = table.Column<int>(type: "integer", nullable: false),
                TaskDescription = table.Column<string>(type: "text", nullable: false),
                Status = table.Column<string>(type: "text", nullable: false),
                MeetingLocation = table.Column<string>(type: "text", nullable: true),
                MeetingContent = table.Column<string>(type: "text", nullable: true),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProjectProgressWeek", x => x.Id);
                table.ForeignKey(
                    name: "FK_ProjectProgressWeek_ProjectProgress_ProjectProgressId",
                    column: x => x.ProjectProgressId,
                    principalTable: "ProjectProgress",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "FucTask",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                KeyTask = table.Column<string>(type: "text", nullable: false),
                Description = table.Column<string>(type: "text", nullable: false),
                Summary = table.Column<string>(type: "text", nullable: false),
                AssigneeId = table.Column<string>(type: "text", nullable: false),
                ReporterId = table.Column<string>(type: "text", nullable: false),
                Status = table.Column<string>(type: "text", nullable: false, defaultValue: "ToDo"),
                Priority = table.Column<string>(type: "text", nullable: false),
                DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Comment = table.Column<string>(type: "text", nullable: true),
                ProjectProgressWeekId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FucTask", x => x.Id);
                table.ForeignKey(
                    name: "FK_FucTask_ProjectProgressWeek_ProjectProgressWeekId",
                    column: x => x.ProjectProgressWeekId,
                    principalTable: "ProjectProgressWeek",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_FucTask_Student_AssigneeId",
                    column: x => x.AssigneeId,
                    principalTable: "Student",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_FucTask_Student_ReporterId",
                    column: x => x.ReporterId,
                    principalTable: "Student",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "WeeklyEvaluation",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                StudentId = table.Column<string>(type: "text", nullable: false),
                SupervisorId = table.Column<string>(type: "text", nullable: false),
                ProjectProgressWeekId = table.Column<Guid>(type: "uuid", nullable: false),
                ContributionPercentage = table.Column<double>(type: "double precision", nullable: false),
                Comments = table.Column<string>(type: "text", nullable: false),
                Status = table.Column<string>(type: "text", nullable: false),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_WeeklyEvaluation", x => x.Id);
                table.ForeignKey(
                    name: "FK_WeeklyEvaluation_ProjectProgressWeek_ProjectProgressWeekId",
                    column: x => x.ProjectProgressWeekId,
                    principalTable: "ProjectProgressWeek",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_WeeklyEvaluation_Student_StudentId",
                    column: x => x.StudentId,
                    principalTable: "Student",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_WeeklyEvaluation_Supervisor_SupervisorId",
                    column: x => x.SupervisorId,
                    principalTable: "Supervisor",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "FucTaskHistory",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                Content = table.Column<string>(type: "text", nullable: false),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: false),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FucTaskHistory", x => x.Id);
                table.ForeignKey(
                    name: "FK_FucTaskHistory_FucTask_TaskId",
                    column: x => x.TaskId,
                    principalTable: "FucTask",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Capstone_MajorId",
            table: "Capstone",
            column: "MajorId");

        migrationBuilder.CreateIndex(
            name: "IX_CoSupervisor_SupervisorId",
            table: "CoSupervisor",
            column: "SupervisorId");

        migrationBuilder.CreateIndex(
            name: "IX_CoSupervisor_TopicId",
            table: "CoSupervisor",
            column: "TopicId");

        migrationBuilder.CreateIndex(
            name: "IX_FucTask_AssigneeId",
            table: "FucTask",
            column: "AssigneeId");

        migrationBuilder.CreateIndex(
            name: "IX_FucTask_ProjectProgressWeekId",
            table: "FucTask",
            column: "ProjectProgressWeekId");

        migrationBuilder.CreateIndex(
            name: "IX_FucTask_ReporterId",
            table: "FucTask",
            column: "ReporterId");

        migrationBuilder.CreateIndex(
            name: "IX_FucTaskHistory_TaskId",
            table: "FucTaskHistory",
            column: "TaskId");

        migrationBuilder.CreateIndex(
            name: "IX_Group_CampusId",
            table: "Group",
            column: "CampusId");

        migrationBuilder.CreateIndex(
            name: "IX_Group_CapstoneId",
            table: "Group",
            column: "CapstoneId");

        migrationBuilder.CreateIndex(
            name: "IX_Group_MajorId",
            table: "Group",
            column: "MajorId");

        migrationBuilder.CreateIndex(
            name: "IX_Group_SemesterId",
            table: "Group",
            column: "SemesterId");

        migrationBuilder.CreateIndex(
            name: "IX_GroupMember_GroupId",
            table: "GroupMember",
            column: "GroupId");

        migrationBuilder.CreateIndex(
            name: "IX_GroupMember_StudentId",
            table: "GroupMember",
            column: "StudentId");

        migrationBuilder.CreateIndex(
            name: "IX_Major_MajorGroupId",
            table: "Major",
            column: "MajorGroupId");

        migrationBuilder.CreateIndex(
            name: "IX_ProjectProgress_GroupId",
            table: "ProjectProgress",
            column: "GroupId");

        migrationBuilder.CreateIndex(
            name: "IX_ProjectProgress_SupervisorId",
            table: "ProjectProgress",
            column: "SupervisorId");

        migrationBuilder.CreateIndex(
            name: "IX_ProjectProgressWeek_ProjectProgressId",
            table: "ProjectProgressWeek",
            column: "ProjectProgressId");

        migrationBuilder.CreateIndex(
            name: "IX_Student_BusinessAreaId",
            table: "Student",
            column: "BusinessAreaId");

        migrationBuilder.CreateIndex(
            name: "IX_Student_CampusId",
            table: "Student",
            column: "CampusId");

        migrationBuilder.CreateIndex(
            name: "IX_Student_CapstoneId",
            table: "Student",
            column: "CapstoneId");

        migrationBuilder.CreateIndex(
            name: "IX_Student_MajorId",
            table: "Student",
            column: "MajorId");

        migrationBuilder.CreateIndex(
            name: "IX_Supervisor_CampusId",
            table: "Supervisor",
            column: "CampusId");

        migrationBuilder.CreateIndex(
            name: "IX_Supervisor_MajorId",
            table: "Supervisor",
            column: "MajorId");

        migrationBuilder.CreateIndex(
            name: "IX_SupervisorGroupAssignment_GroupId",
            table: "SupervisorGroupAssignment",
            column: "GroupId");

        migrationBuilder.CreateIndex(
            name: "IX_SupervisorGroupAssignment_Supervisor2Id",
            table: "SupervisorGroupAssignment",
            column: "Supervisor2Id");

        migrationBuilder.CreateIndex(
            name: "IX_SupervisorGroupAssignment_SupervisorId",
            table: "SupervisorGroupAssignment",
            column: "SupervisorId");

        migrationBuilder.CreateIndex(
            name: "IX_Topic_BusinessAreaId",
            table: "Topic",
            column: "BusinessAreaId");

        migrationBuilder.CreateIndex(
            name: "IX_Topic_CampusId",
            table: "Topic",
            column: "CampusId");

        migrationBuilder.CreateIndex(
            name: "IX_Topic_CapstoneId",
            table: "Topic",
            column: "CapstoneId");

        migrationBuilder.CreateIndex(
            name: "IX_Topic_MainSupervisorId",
            table: "Topic",
            column: "MainSupervisorId");

        migrationBuilder.CreateIndex(
            name: "IX_Topic_SemesterId",
            table: "Topic",
            column: "SemesterId");

        migrationBuilder.CreateIndex(
            name: "IX_TopicAnalysis_TopicId",
            table: "TopicAnalysis",
            column: "TopicId");

        migrationBuilder.CreateIndex(
            name: "IX_TopicAppraisal_SupervisorId",
            table: "TopicAppraisal",
            column: "SupervisorId");

        migrationBuilder.CreateIndex(
            name: "IX_TopicAppraisal_TopicId",
            table: "TopicAppraisal",
            column: "TopicId");

        migrationBuilder.CreateIndex(
            name: "IX_TopicRequest_GroupId",
            table: "TopicRequest",
            column: "GroupId");

        migrationBuilder.CreateIndex(
            name: "IX_TopicRequest_SupervisorId",
            table: "TopicRequest",
            column: "SupervisorId");

        migrationBuilder.CreateIndex(
            name: "IX_TopicRequest_TopicId",
            table: "TopicRequest",
            column: "TopicId");

        migrationBuilder.CreateIndex(
            name: "IX_WeeklyEvaluation_ProjectProgressWeekId",
            table: "WeeklyEvaluation",
            column: "ProjectProgressWeekId");

        migrationBuilder.CreateIndex(
            name: "IX_WeeklyEvaluation_StudentId",
            table: "WeeklyEvaluation",
            column: "StudentId");

        migrationBuilder.CreateIndex(
            name: "IX_WeeklyEvaluation_SupervisorId",
            table: "WeeklyEvaluation",
            column: "SupervisorId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CoSupervisor");

        migrationBuilder.DropTable(
            name: "FucTaskHistory");

        migrationBuilder.DropTable(
            name: "GroupMember");

        migrationBuilder.DropTable(
            name: "IntegrationEventLogs");

        migrationBuilder.DropTable(
            name: "SupervisorGroupAssignment");

        migrationBuilder.DropTable(
            name: "TemplateDocument");

        migrationBuilder.DropTable(
            name: "TopicAnalysis");

        migrationBuilder.DropTable(
            name: "TopicAppraisal");

        migrationBuilder.DropTable(
            name: "TopicRequest");

        migrationBuilder.DropTable(
            name: "WeeklyEvaluation");

        migrationBuilder.DropTable(
            name: "FucTask");

        migrationBuilder.DropTable(
            name: "Topic");

        migrationBuilder.DropTable(
            name: "ProjectProgressWeek");

        migrationBuilder.DropTable(
            name: "Student");

        migrationBuilder.DropTable(
            name: "ProjectProgress");

        migrationBuilder.DropTable(
            name: "BusinessArea");

        migrationBuilder.DropTable(
            name: "Group");

        migrationBuilder.DropTable(
            name: "Supervisor");

        migrationBuilder.DropTable(
            name: "Capstone");

        migrationBuilder.DropTable(
            name: "Semester");

        migrationBuilder.DropTable(
            name: "Campus");

        migrationBuilder.DropTable(
            name: "Major");

        migrationBuilder.DropTable(
            name: "MajorGroup");
    }
}
