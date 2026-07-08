using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ttnndev.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeTai",
                columns: table => new
                {
                    MaDeTai = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaGhiDanh = table.Column<int>(type: "integer", nullable: false),
                    TenDeTai = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    MoTa = table.Column<string>(type: "text", nullable: false),
                    TrangThai = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    KhoaChinhSua = table.Column<bool>(type: "boolean", nullable: false),
                    NgayTao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeTai", x => x.MaDeTai);
                });

            migrationBuilder.CreateTable(
                name: "KyThucTap",
                columns: table => new
                {
                    MaKy = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NamHoc = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false),
                    HocKy = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    MaLoaiThucTap = table.Column<int>(type: "integer", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TrangThaiCongBo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    TrangThaiKy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    NgayKhoaSoDiem = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DaKhoaSoDiem = table.Column<bool>(type: "boolean", nullable: false),
                    NgayKhoaSoDiemThucTe = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SoNgayCanhBaoSom = table.Column<short>(type: "smallint", nullable: false),
                    KhoaBoi = table.Column<int>(type: "integer", nullable: true),
                    MaGiaoVuTao = table.Column<int>(type: "integer", nullable: false),
                    MaKhoa = table.Column<int>(type: "integer", nullable: false),
                    NgayTao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DaKhoaVinhVien = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KyThucTap", x => x.MaKy);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDung",
                columns: table => new
                {
                    MaNguoiDung = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaDinhDanh = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    HoTen = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    SoDienThoai = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    VaiTro = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TrangThaiTaiKhoan = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MatKhauHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    BuocDoiMatKhau = table.Column<bool>(type: "boolean", nullable: false),
                    SoLanDangNhapSai = table.Column<short>(type: "smallint", nullable: false),
                    KhoaDangNhapDenLuc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    TokenValidFrom = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AnhDaiDien = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    NgayTao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    NgayCapNhat = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DaXoa = table.Column<bool>(type: "boolean", nullable: false),
                    NgayXoa = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDung", x => x.MaNguoiDung);
                });

            migrationBuilder.CreateTable(
                name: "PhienChat",
                columns: table => new
                {
                    MaPhien = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaNguoiDung = table.Column<int>(type: "integer", nullable: false),
                    NgayTao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhienChat", x => x.MaPhien);
                });

            migrationBuilder.CreateTable(
                name: "TieuChiRubric",
                columns: table => new
                {
                    MaTieuChi = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaRubric = table.Column<int>(type: "integer", nullable: false),
                    TenTieuChi = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TieuChiRubric", x => x.MaTieuChi);
                });

            migrationBuilder.CreateTable(
                name: "LopThucTap",
                columns: table => new
                {
                    MaLop = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaKy = table.Column<int>(type: "integer", nullable: false),
                    MaGiangVien = table.Column<int>(type: "integer", nullable: false),
                    SoThuTuLop = table.Column<int>(type: "integer", nullable: false),
                    TenLop = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MaThamGia = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    GhiDanhMo = table.Column<bool>(type: "boolean", nullable: false),
                    HanGhiDanh = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    MoDangKyDeTaiCapLop = table.Column<bool>(type: "boolean", nullable: false),
                    KhoaDangKyBoi = table.Column<int>(type: "integer", nullable: true),
                    NgayKhoaDangKy = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LinkHopMacDinh = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    NgayTao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LopThucTap", x => x.MaLop);
                    table.ForeignKey(
                        name: "FK_LopThucTap_KyThucTap_MaKy",
                        column: x => x.MaKy,
                        principalTable: "KyThucTap",
                        principalColumn: "MaKy",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LopThucTap_NguoiDung_MaGiangVien",
                        column: x => x.MaGiangVien,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TinNhanChat",
                columns: table => new
                {
                    MaTinNhan = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaPhien = table.Column<long>(type: "bigint", nullable: false),
                    VaiTroGui = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    NoiDung = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    LinkThamChieu = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ThoiDiem = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TinNhanChat", x => x.MaTinNhan);
                    table.ForeignKey(
                        name: "FK_TinNhanChat_PhienChat_MaPhien",
                        column: x => x.MaPhien,
                        principalTable: "PhienChat",
                        principalColumn: "MaPhien",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CauHinhDiemLop",
                columns: table => new
                {
                    MaCauHinh = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaLop = table.Column<int>(type: "integer", nullable: false),
                    SoChuSoThapPhan = table.Column<short>(type: "smallint", nullable: false),
                    ThangDiem = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHinhDiemLop", x => x.MaCauHinh);
                    table.ForeignKey(
                        name: "FK_CauHinhDiemLop_LopThucTap_MaLop",
                        column: x => x.MaLop,
                        principalTable: "LopThucTap",
                        principalColumn: "MaLop",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GhiDanhSinhVien",
                columns: table => new
                {
                    MaGhiDanh = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaLop = table.Column<int>(type: "integer", nullable: false),
                    MaSinhVien = table.Column<int>(type: "integer", nullable: false),
                    MaLopChuyenTu = table.Column<int>(type: "integer", nullable: true),
                    TrangThaiThucTap = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TinhTrangTienDo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    LyDoDung = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    NgayDung = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NgayGhiDanh = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    GhiDanhBang = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GhiDanhSinhVien", x => x.MaGhiDanh);
                    table.ForeignKey(
                        name: "FK_GhiDanhSinhVien_LopThucTap_MaLop",
                        column: x => x.MaLop,
                        principalTable: "LopThucTap",
                        principalColumn: "MaLop",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GhiDanhSinhVien_NguoiDung_MaSinhVien",
                        column: x => x.MaSinhVien,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NhomDiem",
                columns: table => new
                {
                    MaNhomDiem = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaCauHinh = table.Column<int>(type: "integer", nullable: false),
                    TenNhom = table.Column<string>(type: "text", nullable: false),
                    CauHinhDiemLopMaCauHinh = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhomDiem", x => x.MaNhomDiem);
                    table.ForeignKey(
                        name: "FK_NhomDiem_CauHinhDiemLop_CauHinhDiemLopMaCauHinh",
                        column: x => x.CauHinhDiemLopMaCauHinh,
                        principalTable: "CauHinhDiemLop",
                        principalColumn: "MaCauHinh");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CauHinhDiemLop_MaLop",
                table: "CauHinhDiemLop",
                column: "MaLop");

            migrationBuilder.CreateIndex(
                name: "IX_GhiDanhSinhVien_MaLop",
                table: "GhiDanhSinhVien",
                column: "MaLop");

            migrationBuilder.CreateIndex(
                name: "IX_GhiDanhSinhVien_MaSinhVien_MaLop",
                table: "GhiDanhSinhVien",
                columns: new[] { "MaSinhVien", "MaLop" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LopThucTap_MaGiangVien",
                table: "LopThucTap",
                column: "MaGiangVien");

            migrationBuilder.CreateIndex(
                name: "IX_LopThucTap_MaKy",
                table: "LopThucTap",
                column: "MaKy");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_Email",
                table: "NguoiDung",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_MaDinhDanh",
                table: "NguoiDung",
                column: "MaDinhDanh",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NhomDiem_CauHinhDiemLopMaCauHinh",
                table: "NhomDiem",
                column: "CauHinhDiemLopMaCauHinh");

            migrationBuilder.CreateIndex(
                name: "IX_TinNhanChat_MaPhien",
                table: "TinNhanChat",
                column: "MaPhien");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeTai");

            migrationBuilder.DropTable(
                name: "GhiDanhSinhVien");

            migrationBuilder.DropTable(
                name: "NhomDiem");

            migrationBuilder.DropTable(
                name: "TieuChiRubric");

            migrationBuilder.DropTable(
                name: "TinNhanChat");

            migrationBuilder.DropTable(
                name: "CauHinhDiemLop");

            migrationBuilder.DropTable(
                name: "PhienChat");

            migrationBuilder.DropTable(
                name: "LopThucTap");

            migrationBuilder.DropTable(
                name: "KyThucTap");

            migrationBuilder.DropTable(
                name: "NguoiDung");
        }
    }
}
