using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Food.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoriteMeals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "favorite_meals",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RecipeId = table.Column<long>(type: "bigint", nullable: true),
                    ServingsCount = table.Column<decimal>(type: "numeric", nullable: true),
                    IngredientId = table.Column<long>(type: "bigint", nullable: true),
                    quantity_g = table.Column<decimal>(type: "numeric", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_favorite_meals", x => x.Id);
                    table.CheckConstraint("CK_favorite_meals_exactly_one_source", "(\"RecipeId\" IS NOT NULL) <> (\"IngredientId\" IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_favorite_meals_ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_favorite_meals_recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_favorite_meals_IngredientId",
                table: "favorite_meals",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_favorite_meals_RecipeId",
                table: "favorite_meals",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_favorite_meals_UserId",
                table: "favorite_meals",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "favorite_meals");
        }
    }
}
