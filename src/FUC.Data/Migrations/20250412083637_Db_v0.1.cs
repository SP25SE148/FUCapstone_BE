using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUC.Data.Migrations
{
    /// <inheritdoc />
    public partial class Db_v01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessArea",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("4aa870a3-ef57-469d-945f-c2858a61d39c")),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
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
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
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
                    OccurredOnUtc = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ProcessedOnUtc = table.Column<DateTime>(type: "timestamp", nullable: true),
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
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
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
                    StartDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Semester", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TemplateDocument",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("b0e0b4f0-2bae-4d15-ae80-20d33cbcc489")),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    FileUrl = table.Column<string>(type: "text", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsFile = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateDocument", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimeConfiguration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamUpDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    TeamUpExpirationDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    RegistTopicDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    RegistTopicExpiredDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    IsActived = table.Column<bool>(type: "boolean", nullable: false),
                    CampusId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeConfiguration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeConfiguration_Campus_CampusId",
                        column: x => x.CampusId,
                        principalTable: "Campus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Major",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    MajorGroupId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
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
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
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
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
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
                name: "ReviewCriteria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("7ca915ac-6800-4225-bf1b-651d25e907ec")),
                    CapstoneId = table.Column<string>(type: "text", nullable: false),
                    Attempt = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Requirement = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewCriteria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewCriteria_Capstone_CapstoneId",
                        column: x => x.CapstoneId,
                        principalTable: "Capstone",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    GPA = table.Column<float>(type: "real", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "InProgress"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Student_BusinessArea_BusinessAreaId",
                        column: x => x.BusinessAreaId,
                        principalTable: "BusinessArea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("f531b156-512a-4551-981f-4fdf85530662")),
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
                    GroupCode = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "Pending"),
                    IsAssignedToGroup = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DifficultyLevel = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
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
                name: "CoSupervisor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("a0b1338c-de58-4e96-84cd-97786eac4c94")),
                    SupervisorId = table.Column<string>(type: "text", nullable: false),
                    TopicId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
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
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DefendCapstoneProjectInformationCalendar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TopicId = table.Column<Guid>(type: "uuid", nullable: false),
                    TopicCode = table.Column<string>(type: "text", nullable: false),
                    CampusId = table.Column<string>(type: "text", nullable: false),
                    SemesterId = table.Column<string>(type: "text", nullable: false),
                    CapstoneId = table.Column<string>(type: "text", nullable: false),
                    DefendAttempt = table.Column<int>(type: "integer", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    Slot = table.Column<int>(type: "integer", nullable: false),
                    IsUploadedThesisMinute = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "NotStarted"),
                    DefenseDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefendCapstoneProjectInformationCalendar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DefendCapstoneProjectInformationCalendar_Campus_CampusId",
                        column: x => x.CampusId,
                        principalTable: "Campus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DefendCapstoneProjectInformationCalendar_Capstone_CapstoneId",
                        column: x => x.CapstoneId,
                        principalTable: "Capstone",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DefendCapstoneProjectInformationCalendar_Semester_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "Semester",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DefendCapstoneProjectInformationCalendar_Topic_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Group",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("b0addf6c-2f82-4640-9959-7800b7a61fcc")),
                    SemesterId = table.Column<string>(type: "text", nullable: false),
                    MajorId = table.Column<string>(type: "text", nullable: false),
                    CampusId = table.Column<string>(type: "text", nullable: false),
                    SupervisorId = table.Column<string>(type: "text", nullable: true),
                    CapstoneId = table.Column<string>(type: "text", nullable: false),
                    TopicId = table.Column<Guid>(type: "uuid", nullable: true),
                    GPA = table.Column<float>(type: "real", nullable: false),
                    GroupCode = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "Pending"),
                    IsReDefendCapstoneProject = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsUploadGroupDocument = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_Group_Supervisor_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "Supervisor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Group_Topic_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TopicAnalysis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("d024efde-09f3-4387-8b01-d2e5668e34c3")),
                    AnalysisResult = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValue: new DateTime(2025, 4, 12, 15, 36, 36, 874, DateTimeKind.Local).AddTicks(2661)),
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("fec10100-b829-42e2-98cf-1a2810e0b8e9")),
                    SupervisorId = table.Column<string>(type: "text", nullable: false),
                    TopicId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttemptTime = table.Column<int>(type: "integer", nullable: false),
                    AppraisalContent = table.Column<string>(type: "text", nullable: true),
                    AppraisalComment = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "Pending"),
                    AppraisalDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TopicAppraisal_Topic_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DefendCapstoneProjectCouncilMember",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DefendCapstoneProjectInformationCalendarId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupervisorId = table.Column<string>(type: "text", nullable: false),
                    IsPresident = table.Column<bool>(type: "boolean", nullable: false),
                    IsSecretary = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefendCapstoneProjectCouncilMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DefendCapstoneProjectCouncilMember_DefendCapstoneProjectInf~",
                        column: x => x.DefendCapstoneProjectInformationCalendarId,
                        principalTable: "DefendCapstoneProjectInformationCalendar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DefendCapstoneProjectCouncilMember_Supervisor_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "Supervisor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DefendCapstoneProjectDecision",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupervisorId = table.Column<string>(type: "text", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    Decision = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefendCapstoneProjectDecision", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DefendCapstoneProjectDecision_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DefendCapstoneProjectDecision_Supervisor_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "Supervisor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupMember",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("9eafff7c-22e4-4d04-ac2c-ac9247347af9")),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<string>(type: "text", nullable: false),
                    IsLeader = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "UnderReview"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupMember_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JoinGroupRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "Pending"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinGroupRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JoinGroupRequest_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinGroupRequest_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    MeetingDate = table.Column<string>(type: "text", nullable: false),
                    Slot = table.Column<string>(type: "text", nullable: false),
                    SupervisorId = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectProgress_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectProgress_Supervisor_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "Supervisor",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ReviewCalendar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("1bd1fe5a-d1c7-4d51-adf3-60e92e9dcf3a")),
                    TopicId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    MajorId = table.Column<string>(type: "text", nullable: false),
                    CampusId = table.Column<string>(type: "text", nullable: false),
                    SemesterId = table.Column<string>(type: "text", nullable: false),
                    Attempt = table.Column<int>(type: "integer", nullable: false),
                    Slot = table.Column<int>(type: "integer", nullable: false),
                    Room = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "Pending"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewCalendar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewCalendar_Campus_CampusId",
                        column: x => x.CampusId,
                        principalTable: "Campus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewCalendar_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewCalendar_Major_MajorId",
                        column: x => x.MajorId,
                        principalTable: "Major",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewCalendar_Semester_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "Semester",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewCalendar_Topic_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TopicRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("b002b406-d330-4bfd-9874-c75f62654232")),
                    SupervisorId = table.Column<string>(type: "text", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    TopicId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "UnderReview"),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TopicRequest_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TopicRequest_Supervisor_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "Supervisor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TopicRequest_Topic_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    ProgressWeekSummary = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectProgressWeek", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectProgressWeek_ProjectProgress_ProjectProgressId",
                        column: x => x.ProjectProgressId,
                        principalTable: "ProjectProgress",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviewer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("a02a033c-efc6-4cbb-8a2e-18b3620936be")),
                    SupervisorId = table.Column<string>(type: "text", nullable: false),
                    ReviewCalenderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Suggestion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsReview = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviewer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviewer_ReviewCalendar_ReviewCalenderId",
                        column: x => x.ReviewCalenderId,
                        principalTable: "ReviewCalendar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviewer_Supervisor_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "Supervisor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    CompletionDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    DueDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ProjectProgressId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectProgressWeekId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FucTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FucTask_ProjectProgressWeek_ProjectProgressWeekId",
                        column: x => x.ProjectProgressWeekId,
                        principalTable: "ProjectProgressWeek",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FucTask_ProjectProgress_ProjectProgressId",
                        column: x => x.ProjectProgressId,
                        principalTable: "ProjectProgress",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FucTask_Student_AssigneeId",
                        column: x => x.AssigneeId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FucTask_Student_ReporterId",
                        column: x => x.ReporterId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyEvaluation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<string>(type: "text", nullable: false),
                    ProjectProgressWeekId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContributionPercentage = table.Column<double>(type: "double precision", nullable: false),
                    Comments = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    SupervisorId = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyEvaluation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeeklyEvaluation_ProjectProgressWeek_ProjectProgressWeekId",
                        column: x => x.ProjectProgressWeekId,
                        principalTable: "ProjectProgressWeek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WeeklyEvaluation_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WeeklyEvaluation_Supervisor_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "Supervisor",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FucTaskHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FucTaskHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FucTaskHistory_FucTask_TaskId",
                        column: x => x.TaskId,
                        principalTable: "FucTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_DefendCapstoneProjectCouncilMember_DefendCapstoneProjectInf~",
                table: "DefendCapstoneProjectCouncilMember",
                column: "DefendCapstoneProjectInformationCalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_DefendCapstoneProjectCouncilMember_SupervisorId",
                table: "DefendCapstoneProjectCouncilMember",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_DefendCapstoneProjectDecision_GroupId",
                table: "DefendCapstoneProjectDecision",
                column: "GroupId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DefendCapstoneProjectDecision_SupervisorId",
                table: "DefendCapstoneProjectDecision",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_DefendCapstoneProjectInformationCalendar_CampusId",
                table: "DefendCapstoneProjectInformationCalendar",
                column: "CampusId");

            migrationBuilder.CreateIndex(
                name: "IX_DefendCapstoneProjectInformationCalendar_CapstoneId",
                table: "DefendCapstoneProjectInformationCalendar",
                column: "CapstoneId");

            migrationBuilder.CreateIndex(
                name: "IX_DefendCapstoneProjectInformationCalendar_SemesterId",
                table: "DefendCapstoneProjectInformationCalendar",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_DefendCapstoneProjectInformationCalendar_TopicId",
                table: "DefendCapstoneProjectInformationCalendar",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_FucTask_AssigneeId",
                table: "FucTask",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_FucTask_ProjectProgressId",
                table: "FucTask",
                column: "ProjectProgressId");

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
                name: "IX_Group_SupervisorId",
                table: "Group",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_Group_TopicId",
                table: "Group",
                column: "TopicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupMember_GroupId",
                table: "GroupMember",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMember_StudentId",
                table: "GroupMember",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinGroupRequest_GroupId",
                table: "JoinGroupRequest",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinGroupRequest_StudentId",
                table: "JoinGroupRequest",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Major_MajorGroupId",
                table: "Major",
                column: "MajorGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProgress_GroupId",
                table: "ProjectProgress",
                column: "GroupId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProgress_SupervisorId",
                table: "ProjectProgress",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProgressWeek_ProjectProgressId",
                table: "ProjectProgressWeek",
                column: "ProjectProgressId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewCalendar_CampusId",
                table: "ReviewCalendar",
                column: "CampusId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewCalendar_GroupId",
                table: "ReviewCalendar",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewCalendar_MajorId",
                table: "ReviewCalendar",
                column: "MajorId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewCalendar_SemesterId",
                table: "ReviewCalendar",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewCalendar_TopicId",
                table: "ReviewCalendar",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewCriteria_CapstoneId",
                table: "ReviewCriteria",
                column: "CapstoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviewer_ReviewCalenderId",
                table: "Reviewer",
                column: "ReviewCalenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviewer_SupervisorId",
                table: "Reviewer",
                column: "SupervisorId");

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
                name: "IX_TimeConfiguration_CampusId",
                table: "TimeConfiguration",
                column: "CampusId");

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
                name: "IX_Topic_Code",
                table: "Topic",
                column: "Code",
                unique: true);

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
                name: "DefendCapstoneProjectCouncilMember");

            migrationBuilder.DropTable(
                name: "DefendCapstoneProjectDecision");

            migrationBuilder.DropTable(
                name: "FucTaskHistory");

            migrationBuilder.DropTable(
                name: "GroupMember");

            migrationBuilder.DropTable(
                name: "IntegrationEventLogs");

            migrationBuilder.DropTable(
                name: "JoinGroupRequest");

            migrationBuilder.DropTable(
                name: "ReviewCriteria");

            migrationBuilder.DropTable(
                name: "Reviewer");

            migrationBuilder.DropTable(
                name: "TemplateDocument");

            migrationBuilder.DropTable(
                name: "TimeConfiguration");

            migrationBuilder.DropTable(
                name: "TopicAnalysis");

            migrationBuilder.DropTable(
                name: "TopicAppraisal");

            migrationBuilder.DropTable(
                name: "TopicRequest");

            migrationBuilder.DropTable(
                name: "WeeklyEvaluation");

            migrationBuilder.DropTable(
                name: "DefendCapstoneProjectInformationCalendar");

            migrationBuilder.DropTable(
                name: "FucTask");

            migrationBuilder.DropTable(
                name: "ReviewCalendar");

            migrationBuilder.DropTable(
                name: "ProjectProgressWeek");

            migrationBuilder.DropTable(
                name: "Student");

            migrationBuilder.DropTable(
                name: "ProjectProgress");

            migrationBuilder.DropTable(
                name: "Group");

            migrationBuilder.DropTable(
                name: "Topic");

            migrationBuilder.DropTable(
                name: "BusinessArea");

            migrationBuilder.DropTable(
                name: "Capstone");

            migrationBuilder.DropTable(
                name: "Semester");

            migrationBuilder.DropTable(
                name: "Supervisor");

            migrationBuilder.DropTable(
                name: "Campus");

            migrationBuilder.DropTable(
                name: "Major");

            migrationBuilder.DropTable(
                name: "MajorGroup");
        }
    }
}
