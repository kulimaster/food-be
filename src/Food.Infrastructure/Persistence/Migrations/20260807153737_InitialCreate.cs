using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Food.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ingredients",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    calories_per_100g = table.Column<decimal>(type: "numeric", nullable: false),
                    protein_per_100g = table.Column<decimal>(type: "numeric", nullable: false),
                    carbs_per_100g = table.Column<decimal>(type: "numeric", nullable: false),
                    fat_per_100g = table.Column<decimal>(type: "numeric", nullable: false),
                    fiber_per_100g = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ingredients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ingredient_tags",
                columns: table => new
                {
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IngredientId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ingredient_tags", x => new { x.IngredientId, x.name });
                    table.ForeignKey(
                        name: "FK_ingredient_tags_ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ingredient_tags");

            migrationBuilder.DropTable(
                name: "ingredients");
        }
    }
}
