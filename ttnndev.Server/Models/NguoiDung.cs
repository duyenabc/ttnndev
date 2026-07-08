using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ttnndev.Server.Models
{
    [Table("NguoiDung")]
    public class NguoiDung
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaNguoiDung { get; set; }

        [Required]
        [MaxLength(20)]
        public string MaDinhDanh { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        public string HoTen { get; set; } = null!;

        [MaxLength(20)]
        public string? SoDienThoai { get; set; }

        [Required]
        [MaxLength(20)]
        public string VaiTro { get; set; } = null!; // Admin, GiaoVu, GiangVien, SinhVien

        [Required]
        [MaxLength(20)]
        public string TrangThaiTaiKhoan { get; set; } = "Nhap"; // Nhap, ChoKichHoat, DangHoatDong, BiKhoa

        [MaxLength(256)]
        public string? MatKhauHash { get; set; }

        public bool BuocDoiMatKhau { get; set; } = false;
        public short SoLanDangNhapSai { get; set; } = 0;
        public DateTimeOffset? KhoaDangNhapDenLuc { get; set; }

        public DateTimeOffset TokenValidFrom { get; set; } = DateTimeOffset.UtcNow;

        [MaxLength(500)]
        public string? AnhDaiDien { get; set; }

        public DateTimeOffset NgayTao { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset NgayCapNhat { get; set; } = DateTimeOffset.UtcNow;

        public bool DaXoa { get; set; } = false;
        public DateTimeOffset? NgayXoa { get; set; }
    }
}