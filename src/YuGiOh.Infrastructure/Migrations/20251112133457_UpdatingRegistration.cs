using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YuGiOh.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsPlaying",
                table: "Registrations",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                comment: "Indicates whether the player is currently active in the tournament.",
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false,
                oldComment: "Indicates whether the player is currently active in the tournament.");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Registrations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "(timezone('utc', now()))",
                comment: "Timestamp indicating when the registration was created.",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Timestamp indicating when the registration was created.");

            migrationBuilder.AlterColumn<bool>(
                name: "Accepted",
                table: "Registrations",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                comment: "Indicates whether the tournament administrator accepted this registration.",
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false,
                oldComment: "Indicates whether the tournament administrator accepted this registration.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsPlaying",
                table: "Registrations",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Indicates whether the player is currently active in the tournament.",
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true,
                oldComment: "Indicates whether the player is currently active in the tournament.");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Registrations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Timestamp indicating when the registration was created.",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "(timezone('utc', now()))",
                oldComment: "Timestamp indicating when the registration was created.");

            migrationBuilder.AlterColumn<bool>(
                name: "Accepted",
                table: "Registrations",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Indicates whether the tournament administrator accepted this registration.",
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true,
                oldComment: "Indicates whether the tournament administrator accepted this registration.");
        }
    }
}
