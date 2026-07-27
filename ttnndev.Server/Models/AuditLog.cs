using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ttnndev.Server.Models
{
    // Lịch sử thao tác quản lý tài khoản (E01.2, E00.5)
    [Table("AuditLog")]
    public class AuditLog
    {
        [Key]
        public long MaLog { get; set; }

        [Required]
        public int MaNguoiThucHien { get; set; }

        [ForeignKey(nameof(MaNguoiThucHien))]
        public NguoiDung? NguoiThucHien { get; set; }

        [Required]
        [MaxLength(50)]
        public string HanhDong { get; set; } = null!;

        public int? MaDoiTuong { get; set; }

        [ForeignKey(nameof(MaDoiTuong))]
        public NguoiDung? DoiTuong { get; set; }

        public int? MaBatch { get; set; }

        [Column(TypeName = "jsonb")]
        public string? GiaTriCu { get; set; }

        [Column(TypeName = "jsonb")]
        public string? GiaTriMoi { get; set; }

        public DateTimeOffset ThoiDiem { get; set; } = DateTimeOffset.UtcNow;
    }
}
