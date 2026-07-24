using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ttnndev.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountAuthTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SoDienThoai",
                table: "NguoiDung",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "MatKhauHash",
                table: "NguoiDung",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "AnhDaiDien",
                table: "NguoiDung",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.CreateTable(
                name: "AuditLog",
                columns: table => new
                {
                    MaLog = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    MaNguoiThucHien = table.Column<int>(type: "integer", nullable: false),
                    HanhDong = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MaDoiTuong = table.Column<int>(type: "integer", nullable: true),
                    MaBatch = table.Column<int>(type: "integer", nullable: true),
                    GiaTriCu = table.Column<string>(type: "jsonb", nullable: true),
                    GiaTriMoi = table.Column<string>(type: "jsonb", nullable: true),
                    ThoiDiem = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.MaLog);
                    table.ForeignKey(
                        name: "FK_AuditLog_NguoiDung_MaDoiTuong",
                        column: x => x.MaDoiTuong,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditLog_NguoiDung_MaNguoiThucHien",
                        column: x => x.MaNguoiThucHien,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Khoa",
                columns: table => new
                {
                    MaKhoa = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    TenKhoa = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Khoa", x => x.MaKhoa);
                });

            migrationBuilder.CreateTable(
                name: "LichSuImport",
                columns: table => new
                {
                    MaBatch = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    MaNguoiThucHien = table.Column<int>(type: "integer", nullable: false),
                    VaiTroImport = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MaLop = table.Column<int>(type: "integer", nullable: true),
                    SoHopLe = table.Column<int>(type: "integer", nullable: false),
                    SoGhiDe = table.Column<int>(type: "integer", nullable: false),
                    SoLoi = table.Column<int>(type: "integer", nullable: false),
                    NgayThucHien = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuImport", x => x.MaBatch);
                    table.ForeignKey(
                        name: "FK_LichSuImport_NguoiDung_MaNguoiThucHien",
                        column: x => x.MaNguoiThucHien,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuyenGiaoVu",
                columns: table => new
                {
                    MaQuyenGiaoVu = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    MaGiaoVu = table.Column<int>(type: "integer", nullable: false),
                    QuyenQuanLyNguoiDung = table.Column<bool>(type: "boolean", nullable: false),
                    NgayCapGanNhat = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CapBoiGanNhat = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuyenGiaoVu", x => x.MaQuyenGiaoVu);
                    table.ForeignKey(
                        name: "FK_QuyenGiaoVu_NguoiDung_MaGiaoVu",
                        column: x => x.MaGiaoVu,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    MaRefreshToken = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    MaNguoiDung = table.Column<int>(type: "integer", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ThoiDiemTao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ThoiDiemHetHan = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DaThuHoi = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.MaRefreshToken);
                    table.ForeignKey(
                        name: "FK_RefreshToken_NguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YeuCauTaiKhoan",
                columns: table => new
                {
                    MaYeuCau = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    LoaiYeuCau = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MaNguoiYeuCau = table.Column<int>(type: "integer", nullable: false),
                    MaDoiTuong = table.Column<int>(type: "integer", nullable: true),
                    MaBatch = table.Column<int>(type: "integer", nullable: true),
                    LyDoYeuCau = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TrangThai = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MaNguoiXuLy = table.Column<int>(type: "integer", nullable: true),
                    LyDoTuChoi = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    NgayTao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    NgayXuLy = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YeuCauTaiKhoan", x => x.MaYeuCau);
                    table.ForeignKey(
                        name: "FK_YeuCauTaiKhoan_NguoiDung_MaDoiTuong",
                        column: x => x.MaDoiTuong,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_YeuCauTaiKhoan_NguoiDung_MaNguoiXuLy",
                        column: x => x.MaNguoiXuLy,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_YeuCauTaiKhoan_NguoiDung_MaNguoiYeuCau",
                        column: x => x.MaNguoiYeuCau,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "YeuCauXacThuc",
                columns: table => new
                {
                    MaYeuCau = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    MaNguoiDung = table.Column<int>(type: "integer", nullable: false),
                    LoaiYeuCau = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ThoiDiemTao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ThoiDiemHetHan = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DaSuDung = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YeuCauXacThuc", x => x.MaYeuCau);
                    table.ForeignKey(
                        name: "FK_YeuCauXacThuc_NguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BoMon",
                columns: table => new
                {
                    MaBoMon = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    TenBoMon = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MaKhoa = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoMon", x => x.MaBoMon);
                    table.ForeignKey(
                        name: "FK_BoMon_Khoa_MaKhoa",
                        column: x => x.MaKhoa,
                        principalTable: "Khoa",
                        principalColumn: "MaKhoa",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThongTinSinhVien",
                columns: table => new
                {
                    MaThongTinSinhVien = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    MaSinhVien = table.Column<int>(type: "integer", nullable: false),
                    MaKhoa = table.Column<int>(type: "integer", nullable: false),
                    LopSinhHoat = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongTinSinhVien", x => x.MaThongTinSinhVien);
                    table.ForeignKey(
                        name: "FK_ThongTinSinhVien_Khoa_MaKhoa",
                        column: x => x.MaKhoa,
                        principalTable: "Khoa",
                        principalColumn: "MaKhoa",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThongTinSinhVien_NguoiDung_MaSinhVien",
                        column: x => x.MaSinhVien,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThongTinCanBo",
                columns: table => new
                {
                    MaThongTinCanBo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    MaCanBo = table.Column<int>(type: "integer", nullable: false),
                    MaKhoa = table.Column<int>(type: "integer", nullable: false),
                    MaBoMon = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongTinCanBo", x => x.MaThongTinCanBo);
                    table.ForeignKey(
                        name: "FK_ThongTinCanBo_BoMon_MaBoMon",
                        column: x => x.MaBoMon,
                        principalTable: "BoMon",
                        principalColumn: "MaBoMon");
                    table.ForeignKey(
                        name: "FK_ThongTinCanBo_Khoa_MaKhoa",
                        column: x => x.MaKhoa,
                        principalTable: "Khoa",
                        principalColumn: "MaKhoa",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThongTinCanBo_NguoiDung_MaCanBo",
                        column: x => x.MaCanBo,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_MaDoiTuong",
                table: "AuditLog",
                column: "MaDoiTuong");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_MaNguoiThucHien",
                table: "AuditLog",
                column: "MaNguoiThucHien");

            migrationBuilder.CreateIndex(
                name: "IX_BoMon_MaKhoa",
                table: "BoMon",
                column: "MaKhoa");

            migrationBuilder.CreateIndex(
                name: "IX_BoMon_TenBoMon",
                table: "BoMon",
                column: "TenBoMon",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Khoa_TenKhoa",
                table: "Khoa",
                column: "TenKhoa",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LichSuImport_MaNguoiThucHien",
                table: "LichSuImport",
                column: "MaNguoiThucHien");

            migrationBuilder.CreateIndex(
                name: "IX_QuyenGiaoVu_MaGiaoVu",
                table: "QuyenGiaoVu",
                column: "MaGiaoVu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_MaNguoiDung",
                table: "RefreshToken",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_TokenHash",
                table: "RefreshToken",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThongTinCanBo_MaBoMon",
                table: "ThongTinCanBo",
                column: "MaBoMon");

            migrationBuilder.CreateIndex(
                name: "IX_ThongTinCanBo_MaCanBo",
                table: "ThongTinCanBo",
                column: "MaCanBo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThongTinCanBo_MaKhoa",
                table: "ThongTinCanBo",
                column: "MaKhoa");

            migrationBuilder.CreateIndex(
                name: "IX_ThongTinSinhVien_MaKhoa",
                table: "ThongTinSinhVien",
                column: "MaKhoa");

            migrationBuilder.CreateIndex(
                name: "IX_ThongTinSinhVien_MaSinhVien",
                table: "ThongTinSinhVien",
                column: "MaSinhVien",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauTaiKhoan_MaDoiTuong",
                table: "YeuCauTaiKhoan",
                column: "MaDoiTuong");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauTaiKhoan_MaNguoiXuLy",
                table: "YeuCauTaiKhoan",
                column: "MaNguoiXuLy");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauTaiKhoan_MaNguoiYeuCau",
                table: "YeuCauTaiKhoan",
                column: "MaNguoiYeuCau");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauXacThuc_MaNguoiDung",
                table: "YeuCauXacThuc",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauXacThuc_TokenHash",
                table: "YeuCauXacThuc",
                column: "TokenHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLog");

            migrationBuilder.DropTable(
                name: "LichSuImport");

            migrationBuilder.DropTable(
                name: "QuyenGiaoVu");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "ThongTinCanBo");

            migrationBuilder.DropTable(
                name: "ThongTinSinhVien");

            migrationBuilder.DropTable(
                name: "YeuCauTaiKhoan");

            migrationBuilder.DropTable(
                name: "YeuCauXacThuc");

            migrationBuilder.DropTable(
                name: "BoMon");

            migrationBuilder.DropTable(
                name: "Khoa");

            migrationBuilder.AlterColumn<string>(
                name: "SoDienThoai",
                table: "NguoiDung",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MatKhauHash",
                table: "NguoiDung",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AnhDaiDien",
                table: "NguoiDung",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);
        }
    }
}
