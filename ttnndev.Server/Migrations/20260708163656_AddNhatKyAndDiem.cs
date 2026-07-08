using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ttnndev.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddNhatKyAndDiem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiemSinhVien",
                columns: table => new
                {
                    MaDiem = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaGhiDanh = table.Column<int>(type: "integer", nullable: false),
                    MaCotDiem = table.Column<int>(type: "integer", nullable: false),
                    DiemSo = table.Column<decimal>(type: "numeric", nullable: false),
                    GhiChu = table.Column<string>(type: "text", nullable: true),
                    NgayCapNhat = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiemSinhVien", x => x.MaDiem);
                    table.ForeignKey(
                        name: "FK_DiemSinhVien_GhiDanhSinhVien_MaGhiDanh",
                        column: x => x.MaGhiDanh,
                        principalTable: "GhiDanhSinhVien",
                        principalColumn: "MaGhiDanh",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NhatKy",
                columns: table => new
                {
                    MaNhatKy = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaGhiDanh = table.Column<int>(type: "integer", nullable: false),
                    Tuan = table.Column<int>(type: "integer", nullable: false),
                    NgayThucHien = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CongViecDaLam = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    KetQua = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    KhoKhan = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    KeHoachTuanToi = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    TrangThaiDuyet = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    NhanXetGiangVien = table.Column<string>(type: "text", nullable: true),
                    NgayTao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhatKy", x => x.MaNhatKy);
                    table.ForeignKey(
                        name: "FK_NhatKy_GhiDanhSinhVien_MaGhiDanh",
                        column: x => x.MaGhiDanh,
                        principalTable: "GhiDanhSinhVien",
                        principalColumn: "MaGhiDanh",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiemSinhVien_MaGhiDanh",
                table: "DiemSinhVien",
                column: "MaGhiDanh");

            migrationBuilder.CreateIndex(
                name: "IX_NhatKy_MaGhiDanh",
                table: "NhatKy",
                column: "MaGhiDanh");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiemSinhVien");

            migrationBuilder.DropTable(
                name: "NhatKy");
        }
    }
}
