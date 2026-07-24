using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ttnndev.Server.Models
{
    // Refresh token cho phiên đăng nhập (E15.7)
    [Table("RefreshToken")]
    public class RefreshToken
    {
        [Key]
        public long MaRefreshToken { get; set; }

        [Required]
        public int MaNguoiDung { get; set; }

        [ForeignKey(nameof(MaNguoiDung))]
        public NguoiDung? NguoiDung { get; set; }

        [Required]
        [MaxLength(256)]
        public string TokenHash { get; set; } = null!;

        public DateTimeOffset ThoiDiemTao { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset ThoiDiemHetHan { get; set; }

        public bool DaThuHoi { get; set; } = false;
    }
}
