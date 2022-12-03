using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blazor.Presentation.Server.Migrations
{
    public partial class RemoveSourceFromCarouselItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "image_src",
                table: "CarouselItems");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "image_src",
                table: "CarouselItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "CarouselItems",
                keyColumn: "pk_carousel_item",
                keyValue: 1,
                column: "image_src",
                value: "images/carousel/image-01.jpg");

            migrationBuilder.UpdateData(
                table: "CarouselItems",
                keyColumn: "pk_carousel_item",
                keyValue: 2,
                column: "image_src",
                value: "images/carousel/image-02.jpg");

            migrationBuilder.UpdateData(
                table: "CarouselItems",
                keyColumn: "pk_carousel_item",
                keyValue: 3,
                column: "image_src",
                value: "images/carousel/image-03.jpg");

            migrationBuilder.UpdateData(
                table: "CarouselItems",
                keyColumn: "pk_carousel_item",
                keyValue: 4,
                column: "image_src",
                value: "images/carousel/image-04.jpg");
        }
    }
}
