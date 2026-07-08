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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình Unique Constraints cho các trường định danh
            modelBuilder.Entity<NguoiDung>().HasIndex(u => u.MaDinhDanh).IsUnique();
            modelBuilder.Entity<NguoiDung>().HasIndex(u => u.Email).IsUnique();

            // Cấu hình GhiDanhSinhVien: 1 Sinh viên chỉ ghi danh 1 lần vào 1 lớp
            modelBuilder.Entity<GhiDanhSinhVien>()
                .HasIndex(g => new { g.MaSinhVien, g.MaLop }).IsUnique();

            // Cấu hình quan hệ xóa (Restrict để tránh lỗi vòng lặp xóa)
            modelBuilder.Entity<LopThucTap>()
                .HasOne(l => l.GiangVien)
                .WithMany()
                .HasForeignKey(l => l.MaGiangVien)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}