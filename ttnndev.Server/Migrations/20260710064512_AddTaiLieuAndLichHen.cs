using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ttnndev.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddTaiLieuAndLichHen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaiLieus_NguoiDung_NguoiTaiLenMaNguoiDung",
                table: "TaiLieus");

            migrationBuilder.DropIndex(
                name: "IX_TaiLieus_NguoiTaiLenMaNguoiDung",
                table: "TaiLieus");

            migrationBuilder.DropColumn(
                name: "NguoiTaiLenMaNguoiDung",
                table: "TaiLieus");

            migrationBuilder.AlterColumn<int>(
                name: "NguoiTaiLenId",
                table: "TaiLieus",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_TaiLieus_NguoiTaiLenId",
                table: "TaiLieus",
                column: "NguoiTaiLenId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaiLieus_NguoiDung_NguoiTaiLenId",
                table: "TaiLieus",
                column: "NguoiTaiLenId",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaiLieus_NguoiDung_NguoiTaiLenId",
                table: "TaiLieus");

            migrationBuilder.DropIndex(
                name: "IX_TaiLieus_NguoiTaiLenId",
                table: "TaiLieus");

            migrationBuilder.AlterColumn<string>(
                name: "NguoiTaiLenId",
                table: "TaiLieus",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "NguoiTaiLenMaNguoiDung",
                table: "TaiLieus",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TaiLieus_NguoiTaiLenMaNguoiDung",
                table: "TaiLieus",
                column: "NguoiTaiLenMaNguoiDung");

            migrationBuilder.AddForeignKey(
                name: "FK_TaiLieus_NguoiDung_NguoiTaiLenMaNguoiDung",
                table: "TaiLieus",
                column: "NguoiTaiLenMaNguoiDung",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
