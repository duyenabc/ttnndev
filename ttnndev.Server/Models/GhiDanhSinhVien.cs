using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ttnndev.Server.Models
{
    [Table("GhiDanhSinhVien")]
    public class GhiDanhSinhVien
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaGhiDanh { get; set; }

        public int MaLop { get; set; }
        [ForeignKey("MaLop")]
        public virtual LopThucTap LopThucTap { get; set; } = null!;

        public int MaSinhVien { get; set; } // Map trực tiếp để dễ query
        [ForeignKey("MaSinhVien")]
        public virtual NguoiDung SinhVien { get; set; } = null!;

        public int? MaLopChuyenTu { get; set; }

        [Required]
        [MaxLength(20)]
        public string TrangThaiThucTap { get; set; } = "DangThucTap"; // ChoGhiDanh, DangThucTap, DungThucTap, HoanThanh

        [MaxLength(20)]
        public string? TinhTrangTienDo { get; set; } // DungTienDo, ChamTienDo, CanhBao, CanXuLy

        [MaxLength(500)]
        public string? LyDoDung { get; set; }
        public DateTime? NgayDung { get; set; }

        public DateTimeOffset NgayGhiDanh { get; set; } = DateTimeOffset.UtcNow;

        [Required]
        [MaxLength(20)]
        public string GhiDanhBang { get; set; } = "MaLop"; // MaLop, Import, ThuCong
    }
}