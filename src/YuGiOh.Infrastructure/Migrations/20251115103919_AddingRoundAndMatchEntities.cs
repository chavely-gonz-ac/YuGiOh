using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace YuGiOh.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingRoundAndMatchEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rounds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "text", nullable: false, comment: "Type of round (Classification, KnockOut)."),
                    TournamentId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "(timezone('utc', now()))", comment: "Timestamp indicating when the registration was created.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rounds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rounds_Tournaments",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoundId = table.Column<int>(type: "integer", nullable: false, comment: "FK to the Round this match belongs to."),
                    IsRunning = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Indicates whether the match is currently in progress."),
                    IsFinished = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Indicates whether the match has been completed."),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Exact timestamp when the match started."),
                    WhitePlayerId = table.Column<string>(type: "character varying(50)", nullable: false, comment: "User ID of the white-side player."),
                    WhitePlayerResult = table.Column<int>(type: "integer", nullable: false, defaultValue: 0, comment: "Result score of the white player (0-2)."),
                    BlackPlayerId = table.Column<string>(type: "character varying(50)", nullable: false, comment: "User ID of the black-side player."),
                    BlackPlayerResult = table.Column<int>(type: "integer", nullable: false, defaultValue: 0, comment: "Result score of the black player (0-2)."),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Timestamp indicating when this match was created."),
                    PlayerId = table.Column<string>(type: "character varying(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matches_Players_Black",
                        column: x => x.BlackPlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matches_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Matches_Players_White",
                        column: x => x.WhitePlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matches_Rounds",
                        column: x => x.RoundId,
                        principalTable: "Rounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_BlackPlayerId",
                table: "Matches",
                column: "BlackPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_PlayerId",
                table: "Matches",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_Round_White_Black",
                table: "Matches",
                columns: new[] { "RoundId", "WhitePlayerId", "BlackPlayerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matches_WhitePlayerId",
                table: "Matches",
                column: "WhitePlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Rounds_Tournament",
                table: "Rounds",
                column: "TournamentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "Rounds");
        }
    }
}
