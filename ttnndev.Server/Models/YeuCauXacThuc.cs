using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ttnndev.Server.Models
{
    // Yêu cầu kích hoạt tài khoản / đặt lại mật khẩu qua email (E15.4, E15.6)
    [Table("YeuCauXacThuc")]
    public class YeuCauXacThuc
    {
        [Key]
        public int MaYeuCau { get; set; }

        [Required]
        public int MaNguoiDung { get; set; }

        [ForeignKey(nameof(MaNguoiDung))]
        public NguoiDung? NguoiDung { get; set; }

        // KichHoat, DatLaiMatKhau
        [Required]
        [MaxLength(20)]
        public string LoaiYeuCau { get; set; } = null!;

        [Required]
        [MaxLength(256)]
        public string TokenHash { get; set; } = null!;

        public DateTimeOffset ThoiDiemTao { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset ThoiDiemHetHan { get; set; }

        public bool DaSuDung { get; set; } = false;
    }
}
