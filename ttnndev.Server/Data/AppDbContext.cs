using Microsoft.EntityFrameworkCore;
using ttnndev.Server.Models;

namespace ttnndev.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Khai báo các DbSets
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<KyThucTap> KyThucTaps { get; set; }
        public DbSet<LopThucTap> LopThucTaps { get; set; }
        public DbSet<GhiDanhSinhVien> GhiDanhSinhViens { get; set; }
        public DbSet<DeTai> DeTais { get; set; }
        public DbSet<TinNhanChat> TinNhanChats { get; set; }
        public DbSet<CauHinhDiemLop> CauHinhDiemLops { get; set; }
        public DbSet<TieuChiRubric> TieuChiRubrics { get; set; }
        public DbSet<NhomDiem> NhomDiems { get; set; }
        public DbSet<PhienChat> PhienChats { get; set; }
        public DbSet<NhatKy> NhatKys { get; set; }
        public DbSet<DiemSinhVien> DiemSinhViens { get; set; }
        public DbSet<TaiLieu> TaiLieus { get; set; }
        public DbSet<LichHen> LichHens { get; set; }
        public DbSet<Nhom> Nhoms { get; set; }
        public DbSet<CotDiem> CotDiems { get; set; }

        // Tài khoản & phân quyền (E00/E01/E02/E15)
        public DbSet<Khoa> Khoas { get; set; }
        public DbSet<BoMon> BoMons { get; set; }
        public DbSet<ThongTinCanBo> ThongTinCanBos { get; set; }
        public DbSet<ThongTinSinhVien> ThongTinSinhViens { get; set; }
        public DbSet<QuyenGiaoVu> QuyenGiaoVus { get; set; }
        public DbSet<YeuCauTaiKhoan> YeuCauTaiKhoans { get; set; }
        public DbSet<LichSuImport> LichSuImports { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<YeuCauXacThuc> YeuCauXacThucs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Áp dụng rule GENERATED ALWAYS AS IDENTITY cho tất cả các bảng
            modelBuilder.UseIdentityAlwaysColumns();

            // 2. Cấu hình chi tiết cho bảng NguoiDung
            modelBuilder.Entity<NguoiDung>(entity =>
            {
                // Set UNIQUE constraints
                entity.HasIndex(e => e.MaDinhDanh).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();

                // Ép kiểu tường minh cho CSDL (Tùy chọn, Npgsql tự động map DateTimeOffset sang timestamptz)
                entity.Property(e => e.NgayTao).HasColumnType("timestamp with time zone");
                entity.Property(e => e.NgayCapNhat).HasColumnType("timestamp with time zone");
            });

            // 3. Cấu hình chi tiết cho bảng KyThucTap
            modelBuilder.Entity<KyThucTap>(entity =>
            {
                // Composite Unique Key chặn tạo trùng kỳ thực tập
                entity.HasIndex(e => new { e.LoaiThucTap, e.HocKy, e.NamHoc, e.MaKhoa }).IsUnique();

                // Mapping kiểu DATE thuần túy (không có time)
                entity.Property(e => e.NgayBatDau).HasColumnType("date");
                entity.Property(e => e.NgayKetThuc).HasColumnType("date");
            });

            // --- THỰC HIỆN TƯƠNG TỰ CHO CÁC BẢNG KHÁC ---

            // 4. Cấu hình các bảng tài khoản & phân quyền
            modelBuilder.Entity<Khoa>().HasIndex(e => e.TenKhoa).IsUnique();
            modelBuilder.Entity<BoMon>().HasIndex(e => e.TenBoMon).IsUnique();
            modelBuilder.Entity<ThongTinCanBo>().HasIndex(e => e.MaCanBo).IsUnique();
            modelBuilder.Entity<ThongTinSinhVien>().HasIndex(e => e.MaSinhVien).IsUnique();
            modelBuilder.Entity<QuyenGiaoVu>().HasIndex(e => e.MaGiaoVu).IsUnique();
            modelBuilder.Entity<RefreshToken>().HasIndex(e => e.TokenHash).IsUnique();
            modelBuilder.Entity<YeuCauXacThuc>().HasIndex(e => e.TokenHash).IsUnique();

            // Tránh multiple cascade paths: các FK phụ dùng Restrict
            modelBuilder.Entity<QuyenGiaoVu>()
                .HasOne(e => e.GiaoVu).WithMany().HasForeignKey(e => e.MaGiaoVu)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ThongTinCanBo>()
                .HasOne(e => e.CanBo).WithMany().HasForeignKey(e => e.MaCanBo)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ThongTinSinhVien>()
                .HasOne(e => e.SinhVien).WithMany().HasForeignKey(e => e.MaSinhVien)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<YeuCauTaiKhoan>()
                .HasOne(e => e.NguoiYeuCau).WithMany().HasForeignKey(e => e.MaNguoiYeuCau)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<YeuCauTaiKhoan>()
                .HasOne(e => e.DoiTuong).WithMany().HasForeignKey(e => e.MaDoiTuong)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<YeuCauTaiKhoan>()
                .HasOne(e => e.NguoiXuLy).WithMany().HasForeignKey(e => e.MaNguoiXuLy)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AuditLog>()
                .HasOne(e => e.NguoiThucHien).WithMany().HasForeignKey(e => e.MaNguoiThucHien)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AuditLog>()
                .HasOne(e => e.DoiTuong).WithMany().HasForeignKey(e => e.MaDoiTuong)
                .OnDelete(DeleteBehavior.Restrict);

            // Slice Giảng viên: Lớp/Nhóm, chấm điểm
            modelBuilder.Entity<Nhom>()
                .HasOne(e => e.LopThucTap).WithMany().HasForeignKey(e => e.MaLop)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GhiDanhSinhVien>()
                .HasOne(e => e.Nhom).WithMany().HasForeignKey(e => e.MaNhom)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<CotDiem>()
                .HasOne(e => e.LopThucTap).WithMany().HasForeignKey(e => e.MaLop)
                .OnDelete(DeleteBehavior.Cascade);

            // Mỗi (ghi danh, cột điểm) chỉ có 1 bản ghi điểm
            modelBuilder.Entity<DiemSinhVien>()
                .HasIndex(e => new { e.MaGhiDanh, e.MaCotDiem }).IsUnique();
        }
    }
}