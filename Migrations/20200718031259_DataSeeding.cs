using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FakeXieCheng.API.Migrations
{
    public partial class DataSeeding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TouristRoutes",
                columns: new[] { "Id", "CreateTime", "DepatureTime", "Description", "DiscountPresent", "Features", "Fees", "Notes", "OriginalPrice", "Title", "UpdateTime" },
                values: new object[] { new Guid("7d9c5ddf-67b6-48c8-b250-8b50b1643a31"), new DateTime(2020, 7, 18, 3, 12, 58, 479, DateTimeKind.Utc).AddTicks(6173), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "shuoming", null, null, null, null, 0m, "ceshititle", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TouristRoutes",
                keyColumn: "Id",
                keyValue: new Guid("7d9c5ddf-67b6-48c8-b250-8b50b1643a31"));
        }
    }
}
