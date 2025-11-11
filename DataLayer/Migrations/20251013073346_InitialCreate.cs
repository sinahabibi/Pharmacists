using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Emails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SmtpPortIncoming = table.Column<int>(type: "int", nullable: false),
                    SmtpPortOutgoing = table.Column<int>(type: "int", nullable: false),
                    SmtpServerIncoming = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SmtpServerOutgoing = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsConnectionSuccessfully = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecentActivityPriorities",
                columns: table => new
                {
                    PriorityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecentActivityPriorities", x => x.PriorityId);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataBool = table.Column<bool>(type: "bit", nullable: true),
                    DataInt = table.Column<int>(type: "int", nullable: true),
                    Changeable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Meesage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SendTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TryCount = table.Column<int>(type: "int", nullable: false),
                    LastTry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsBan = table.Column<bool>(type: "bit", nullable: false),
                    ActiveCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SecurityCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsPhoneNumberActive = table.Column<bool>(type: "bit", nullable: false),
                    IsEmailActive = table.Column<bool>(type: "bit", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    LastChange = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActivePhoneNumberCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "RecentActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PriorityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecentActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecentActivities_RecentActivityPriorities_PriorityId",
                        column: x => x.PriorityId,
                        principalTable: "RecentActivityPriorities",
                        principalColumn: "PriorityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    PostId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MetaDescription = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    ImageName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortKey = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Visit = table.Column<int>(type: "int", nullable: false),
                    IsPublish = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuthorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.PostId);
                    table.ForeignKey(
                        name: "FK_Posts_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Visitors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Browser = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PlatformName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ip = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    UserIdKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    FirstVisit = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsBan = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visitors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Visitors_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "PostComment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPublish = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostComment_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Links",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitedLink = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    VisitedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VisitorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Links", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Links_Visitors_VisitorId",
                        column: x => x.VisitorId,
                        principalTable: "Visitors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "RecentActivityPriorities",
                columns: new[] { "PriorityId", "Title" },
                values: new object[,]
                {
                    { 1, "زیاد" },
                    { 2, "متوسط" },
                    { 3, "کم" }
                });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "Changeable", "DataBool", "DataInt", "DataString", "Description", "Title" },
                values: new object[,]
                {
                    { 1, true, null, null, "فروشگاه", null, "نام سایت" },
                    { 2, true, null, null, "این سایت برای مدریت و فروش بهتر شما اینجاست", "شما میتوانید در توضیحات متا در صفحه اصلی ببنید", "توضیحات سایت" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "ActiveCode", "ActivePhoneNumberCode", "Email", "FirstName", "IsBan", "IsDelete", "IsEmailActive", "IsPhoneNumberActive", "LastChange", "LastLoginDate", "LastName", "LastTry", "Password", "PhoneNumber", "RegisterDate", "SecurityCode", "TryCount", "UserName" },
                values: new object[] { 1, "811b78fab121434b8e20b2b263e6bd7e", "12345", "Admin@example.com", "ادمین", false, false, true, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "", null, "admin123", "09xxxxxxxxx", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "40c87d0a7d83413ab8e8c59229e8949e", 0, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Links_VisitorId",
                table: "Links",
                column: "VisitorId");

            migrationBuilder.CreateIndex(
                name: "IX_PostComment_PostId",
                table: "PostComment",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_AuthorId",
                table: "Posts",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_RecentActivities_PriorityId",
                table: "RecentActivities",
                column: "PriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                table: "Users",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Visitors_UserId",
                table: "Visitors",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Emails");

            migrationBuilder.DropTable(
                name: "Links");

            migrationBuilder.DropTable(
                name: "PostComment");

            migrationBuilder.DropTable(
                name: "RecentActivities");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Sms");

            migrationBuilder.DropTable(
                name: "Visitors");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "RecentActivityPriorities");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
