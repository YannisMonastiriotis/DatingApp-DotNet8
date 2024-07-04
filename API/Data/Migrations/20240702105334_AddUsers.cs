using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                    migrationBuilder.InsertData(
                    table: "Users",
                    columns: new[] { "Id","UserName" },
                    values: new object[,]
                    {
                        { 1,"Bob" },
                        { 2,"Tom" },
                        { 3,"Jane" }
                     });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
