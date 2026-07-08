using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using ttnndev.Server.Models;

namespace ttnndev.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Khai báo các bảng
        public DbSet<CauHinhDiemLop> CauHinhDiemLops { get; set; }
        public DbSet<NhomDiem> NhomDiems { get; set; }
        public DbSet<CotDiem> CotDiems { get; set; }
        public DbSet<DiemSinhVien> DiemSinhViens { get; set; }
        public DbSet<CauHinhRubric> CauHinhRubrics { get; set; }
        public DbSet<DeTai> DeTais { get; set; }
        public DbSet<PhienChat> PhienChats { get; set; }
        public DbSet<TinNhanChat> TinNhanChats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. UNIQUE constraint: Đảm bảo Mã định danh và Email không trùng
            modelBuilder.Entity<NguoiDung>()
                .HasIndex(u => u.MaDinhDanh).IsUnique();
            modelBuilder.Entity<NguoiDung>()
                .HasIndex(u => u.Email).IsUnique();

            // 2. UNIQUE constraint: Chống tạo trùng kỳ thực tập (Phòng Race Condition)
            modelBuilder.Entity<KyThucTap>()
                .HasIndex(k => new { k.MaLoaiThucTap, k.HocKy, k.NamHoc, k.MaKhoa }).IsUnique();

            // 3. UNIQUE constraint: 1 Sinh viên chỉ ghi danh 1 lần vào 1 lớp
            modelBuilder.Entity<GhiDanhSinhVien>()
                .HasIndex(g => new { g.MaSinhVien, g.MaLop }).IsUnique();

            // Xử lý Delete Behavior để tránh lỗi "Cascade paths" của SQL
            modelBuilder.Entity<LopThucTap>()
                .HasOne(l => l.GiangVien)
                .WithMany()
                .HasForeignKey(l => l.MaGiangVien)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DeTai>()
                .HasIndex(d => d.MaGhiDanh)
                .IsUnique();
        }
    }
}