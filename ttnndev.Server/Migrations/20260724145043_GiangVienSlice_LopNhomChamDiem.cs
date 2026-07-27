using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ttnndev.Server.Migrations
{
    /// <inheritdoc />
    public partial class GiangVienSlice_LopNhomChamDiem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DiemSinhVien_MaGhiDanh",
                table: "DiemSinhVien");

            migrationBuilder.AddColumn<string>(
                name: "DonViThucTap",
                table: "GhiDanhSinhVien",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaNhom",
                table: "GhiDanhSinhVien",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ViTriThucTap",
                table: "GhiDanhSinhVien",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CotDiem",
                columns: table => new
                {
                    MaCotDiem = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    MaLop = table.Column<int>(type: "integer", nullable: false),
                    TenCot = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DiemToiDa = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    ThuTu = table.Column<int>(type: "integer", nullable: false),
                    NgayTao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotDiem", x => x.MaCotDiem);
                    table.ForeignKey(
                        name: "FK_CotDiem_LopThucTap_MaLop",
                        column: x => x.MaLop,
                        principalTable: "LopThucTap",
                        principalColumn: "MaLop",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Nhom",
                columns: table => new
                {
                    MaNhom = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    MaLop = table.Column<int>(type: "integer", nullable: false),
                    SoThuTu = table.Column<int>(type: "integer", nullable: false),
                    TenNhom = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NgayTao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nhom", x => x.MaNhom);
                    table.ForeignKey(
                        name: "FK_Nhom_LopThucTap_MaLop",
                        column: x => x.MaLop,
                        principalTable: "LopThucTap",
                        principalColumn: "MaLop",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GhiDanhSinhVien_MaNhom",
                table: "GhiDanhSinhVien",
                column: "MaNhom");

            migrationBuilder.CreateIndex(
                name: "IX_DiemSinhVien_MaGhiDanh_MaCotDiem",
                table: "DiemSinhVien",
                columns: new[] { "MaGhiDanh", "MaCotDiem" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CotDiem_MaLop",
                table: "CotDiem",
                column: "MaLop");

            migrationBuilder.CreateIndex(
                name: "IX_Nhom_MaLop",
                table: "Nhom",
                column: "MaLop");

            migrationBuilder.AddForeignKey(
                name: "FK_GhiDanhSinhVien_Nhom_MaNhom",
                table: "GhiDanhSinhVien",
                column: "MaNhom",
                principalTable: "Nhom",
                principalColumn: "MaNhom",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GhiDanhSinhVien_Nhom_MaNhom",
                table: "GhiDanhSinhVien");

            migrationBuilder.DropTable(
                name: "CotDiem");

            migrationBuilder.DropTable(
                name: "Nhom");

            migrationBuilder.DropIndex(
                name: "IX_GhiDanhSinhVien_MaNhom",
                table: "GhiDanhSinhVien");

            migrationBuilder.DropIndex(
                name: "IX_DiemSinhVien_MaGhiDanh_MaCotDiem",
                table: "DiemSinhVien");

            migrationBuilder.DropColumn(
                name: "DonViThucTap",
                table: "GhiDanhSinhVien");

            migrationBuilder.DropColumn(
                name: "MaNhom",
                table: "GhiDanhSinhVien");

            migrationBuilder.DropColumn(
                name: "ViTriThucTap",
                table: "GhiDanhSinhVien");

            migrationBuilder.CreateIndex(
                name: "IX_DiemSinhVien_MaGhiDanh",
                table: "DiemSinhVien",
                column: "MaGhiDanh");
        }
    }
}
