using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Food.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMealLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "meal_logs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    LogDate = table.Column<DateOnly>(type: "date", nullable: false),
                    MealSlot = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RecipeId = table.Column<long>(type: "bigint", nullable: true),
                    ServingsCount = table.Column<decimal>(type: "numeric", nullable: true),
                    IngredientId = table.Column<long>(type: "bigint", nullable: true),
                    quantity_g = table.Column<decimal>(type: "numeric", nullable: true),
                    LoggedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_meal_logs", x => x.Id);
                    table.CheckConstraint("CK_meal_logs_exactly_one_source", "(\"RecipeId\" IS NOT NULL) <> (\"IngredientId\" IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_meal_logs_ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_meal_logs_recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_meal_logs_IngredientId",
                table: "meal_logs",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_meal_logs_RecipeId",
                table: "meal_logs",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_meal_logs_UserId_LogDate",
                table: "meal_logs",
                columns: new[] { "UserId", "LogDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "meal_logs");
        }
    }
}
