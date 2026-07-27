using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ttnndev.Server.Models
{
    [Table("NguoiDung")]
    public class NguoiDung
    {
        [Key]
        // Sẽ được cấu hình GENERATED ALWAYS AS IDENTITY trong DbContext
        public int MaNguoiDung { get; set; }

        [Required]
        [MaxLength(20)]
        public string MaDinhDanh { get; set; }

        [Required]
        [MaxLength(150)]
        public string Email { get; set; }

        [Required]
        [MaxLength(150)]
        public string HoTen { get; set; }

        [MaxLength(20)]
        public string? SoDienThoai { get; set; }

        [Required]
        [MaxLength(20)]
        public string VaiTro { get; set; }

        [Required]
        [MaxLength(20)]
        public string TrangThaiTaiKhoan { get; set; } = "Nhap";

        [MaxLength(256)]
        public string? MatKhauHash { get; set; }

        // BIT chuyển thành BOOLEAN (bool)
        public bool BuocDoiMatKhau { get; set; } = false;

        public short SoLanDangNhapSai { get; set; } = 0;

        // DATETIME chuyển thành TIMESTAMPTZ (DateTimeOffset)
        public DateTimeOffset? KhoaDangNhapDenLuc { get; set; }

        public DateTimeOffset TokenValidFrom { get; set; } = DateTimeOffset.UtcNow;

        // Lưu URL hoặc data URL ảnh đại diện (không giới hạn độ dài)
        [Column(TypeName = "text")]
        public string? AnhDaiDien { get; set; }

        public DateTimeOffset NgayTao { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset NgayCapNhat { get; set; } = DateTimeOffset.UtcNow;

        public bool DaXoa { get; set; } = false;

        public DateTimeOffset? NgayXoa { get; set; }
    }
}