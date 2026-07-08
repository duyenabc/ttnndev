using Microsoft.EntityFrameworkCore;
using ttnndev.Server.Models;

namespace ttnndev.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

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

            // 1. Cấu hình cho NguoiDung
            modelBuilder.Entity<NguoiDung>(entity =>
            {
                entity.HasIndex(u => u.MaDinhDanh).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();

                // Cấu hình giá trị mặc định cho Database
                entity.Property(e => e.TrangThaiTaiKhoan).HasDefaultValue("Nhap");
                entity.Property(e => e.BuocDoiMatKhau).HasDefaultValue(false);
                entity.Property(e => e.SoLanDangNhapSai).HasDefaultValue(0);
                entity.Property(e => e.DaXoa).HasDefaultValue(false);
                entity.Property(e => e.NgayTao).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.NgayCapNhat).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.TokenValidFrom).HasColumnType("timestamp with time zone")
                                                        .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // 2. Cấu hình các bảng khác giữ nguyên
            modelBuilder.Entity<GhiDanhSinhVien>()
                .HasIndex(g => new { g.MaSinhVien, g.MaLop }).IsUnique();

            modelBuilder.Entity<LopThucTap>()
                .HasOne(l => l.GiangVien)
                .WithMany()
                .HasForeignKey(l => l.MaGiangVien)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}