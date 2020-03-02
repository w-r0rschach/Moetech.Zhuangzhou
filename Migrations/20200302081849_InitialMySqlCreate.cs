using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Moetech.Zhuangzhou.Migrations
{
    public partial class InitialMySqlCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommonAuthority",
                columns: table => new
                {
                    AuthorityId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AuthorityName = table.Column<int>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonAuthority", x => x.AuthorityId);
                });

            migrationBuilder.CreateTable(
                name: "CommonCorrelation",
                columns: table => new
                {
                    CorrelationId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PersonnelId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false),
                    Remark = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonCorrelation", x => x.CorrelationId);
                });

            migrationBuilder.CreateTable(
                name: "CommonDepartment",
                columns: table => new
                {
                    DepId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ParentNumber = table.Column<int>(nullable: false),
                    DepName = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonDepartment", x => x.DepId);
                });

            migrationBuilder.CreateTable(
                name: "CommonPersonnelInfo",
                columns: table => new
                {
                    PersonnelId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PersonnelNo = table.Column<int>(nullable: false),
                    PersonnelName = table.Column<string>(maxLength: 50, nullable: false),
                    DepId = table.Column<int>(nullable: false),
                    Avatar = table.Column<string>(maxLength: 200, nullable: true),
                    PersonnelSex = table.Column<int>(nullable: false),
                    BirthDate = table.Column<DateTime>(nullable: true),
                    IdentityCard = table.Column<string>(maxLength: 18, nullable: true),
                    IsWork = table.Column<int>(nullable: false),
                    Nation = table.Column<string>(maxLength: 6, nullable: true),
                    MaritalStatus = table.Column<int>(nullable: false),
                    LiveAddress = table.Column<string>(maxLength: 200, nullable: true),
                    Phone = table.Column<string>(maxLength: 500, nullable: true),
                    WeChatAccount = table.Column<string>(maxLength: 100, nullable: true),
                    Mailbox = table.Column<string>(maxLength: 500, nullable: true),
                    Degree = table.Column<int>(nullable: false),
                    Address = table.Column<string>(maxLength: 200, nullable: true),
                    OnboardingTime = table.Column<DateTime>(nullable: true),
                    DepartureTime = table.Column<DateTime>(nullable: true),
                    TrialTime = table.Column<DateTime>(nullable: true),
                    IsStruggle = table.Column<int>(nullable: false),
                    IsSecrecy = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(maxLength: 255, nullable: false),
                    Password = table.Column<string>(maxLength: 255, nullable: false),
                    AppMaxCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonPersonnelInfo", x => x.PersonnelId);
                });

            migrationBuilder.CreateTable(
                name: "CommonRole",
                columns: table => new
                {
                    RoleId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleName = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonRole", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "CommonRoleAuthority",
                columns: table => new
                {
                    RoleAuthorityId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<int>(nullable: false),
                    AuthorityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonRoleAuthority", x => x.RoleAuthorityId);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    LogId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ModuleName = table.Column<string>(maxLength: 255, nullable: false),
                    OpenationType = table.Column<int>(nullable: false),
                    Level = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    Content = table.Column<string>(maxLength: 2048, nullable: false),
                    OccurredTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.LogId);
                });

            migrationBuilder.CreateTable(
                name: "MachApplyAndReturn",
                columns: table => new
                {
                    ApplyAndReturnId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OprationType = table.Column<int>(nullable: false),
                    ApplyUserID = table.Column<int>(nullable: false),
                    ExamineUserID = table.Column<int>(nullable: false),
                    MachineInfoID = table.Column<int>(nullable: false),
                    ExamineResult = table.Column<int>(nullable: false),
                    ApplyTime = table.Column<DateTime>(nullable: false),
                    ResultTime = table.Column<DateTime>(nullable: false),
                    Remark = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachApplyAndReturn", x => x.ApplyAndReturnId);
                });

            migrationBuilder.CreateTable(
                name: "MachineInfo",
                columns: table => new
                {
                    MachineId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MachineIP = table.Column<string>(maxLength: 20, nullable: false),
                    MachineSystem = table.Column<string>(maxLength: 50, nullable: false),
                    MachineDiskCount = table.Column<double>(nullable: false),
                    MachineMemory = table.Column<double>(nullable: false),
                    MachineState = table.Column<int>(nullable: false),
                    MachineUser = table.Column<string>(maxLength: 20, nullable: false),
                    MachinePassword = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineInfo", x => x.MachineId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommonAuthority");

            migrationBuilder.DropTable(
                name: "CommonCorrelation");

            migrationBuilder.DropTable(
                name: "CommonDepartment");

            migrationBuilder.DropTable(
                name: "CommonPersonnelInfo");

            migrationBuilder.DropTable(
                name: "CommonRole");

            migrationBuilder.DropTable(
                name: "CommonRoleAuthority");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "MachApplyAndReturn");

            migrationBuilder.DropTable(
                name: "MachineInfo");
        }
    }
}
