using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blazor.Presentation.Server.Migrations
{
    public partial class InitialData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CarouselItems",
                columns: new[] { "pk_carousel_item", "image_alt", "image_caption", "image_src" },
                values: new object[,]
                {
                    { 1, "First image", "First image", "images/carousel/image-01.jpg" },
                    { 2, "Second image", "Second image", "images/carousel/image-02.jpg" },
                    { 3, "Third image", "Third image", "images/carousel/image-03.jpg" },
                    { 4, "Fourth image", "Fourth image", "images/carousel/image-04.jpg" }
                });

            migrationBuilder.InsertData(
                table: "Images",
                columns: new[] { "pk_image", "fk_carousel_item", "image_safe_name", "image_unsafe_name" },
                values: new object[,]
                {
                    { 1, 1, "adhdwe.jpg", "image-01.jpg" },
                    { 2, 2, "cxjuad.jpg", "image-02.jpg" },
                    { 3, 3, "ioyweq.jpg", "image-03.jpg" },
                    { 4, 4, "khfhew.jpg", "image-04.jpg" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "pk_image",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "pk_image",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "pk_image",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "pk_image",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "CarouselItems",
                keyColumn: "pk_carousel_item",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CarouselItems",
                keyColumn: "pk_carousel_item",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CarouselItems",
                keyColumn: "pk_carousel_item",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "CarouselItems",
                keyColumn: "pk_carousel_item",
                keyValue: 4);
        }
    }
}
