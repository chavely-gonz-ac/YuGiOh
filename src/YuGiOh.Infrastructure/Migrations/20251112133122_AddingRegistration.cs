using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace YuGiOh.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Registrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Accepted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Indicates whether the tournament administrator accepted this registration."),
                    IsWinner = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Indicates whether the player won the tournament using this registration."),
                    IsPlaying = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Indicates whether the player is currently active in the tournament."),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true, comment: "Optional textual note about the registration (e.g., special conditions or remarks)."),
                    DeckId = table.Column<int>(type: "integer", nullable: false),
                    TournamentId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Timestamp indicating when the registration was created.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Registrations_Decks",
                        column: x => x.DeckId,
                        principalTable: "Decks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Registrations_Tournaments",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_CreatedAt",
                table: "Registrations",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_DeckId",
                table: "Registrations",
                column: "DeckId");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_Tournament_Deck",
                table: "Registrations",
                columns: new[] { "TournamentId", "DeckId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Registrations");
        }
    }
}
