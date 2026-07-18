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
        }
    }
}