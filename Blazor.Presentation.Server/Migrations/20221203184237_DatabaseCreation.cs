using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blazor.Presentation.Server.Migrations
{
    public partial class DatabaseCreation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarouselItems",
                columns: table => new
                {
                    pk_carousel_item = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    image_alt = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    image_caption = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    image_src = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarouselItems", x => x.pk_carousel_item);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    pk_image = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    image_unsafe_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    image_safe_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fk_carousel_item = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.pk_image);
                    table.ForeignKey(
                        name: "FK_Images_CarouselItems_fk_carousel_item",
                        column: x => x.fk_carousel_item,
                        principalTable: "CarouselItems",
                        principalColumn: "pk_carousel_item",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Images_fk_carousel_item",
                table: "Images",
                column: "fk_carousel_item",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "CarouselItems");
        }
    }
}
