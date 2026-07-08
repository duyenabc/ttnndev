using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ttnndev.Server.Models
{
    [Table("DiemSinhVien")]
    public class DiemSinhVien
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaDiem { get; set; }

        [Required]
        public int MaGhiDanh { get; set; }

        [ForeignKey("MaGhiDanh")]
        public virtual GhiDanhSinhVien GhiDanh { get; set; } = null!;

        [Required]
        public int MaCotDiem { get; set; } // Liên kết với bảng định nghĩa cột điểm (ví dụ: Chuyên cần, Báo cáo...)

        [Required]
        [Range(0, 10)]
        public decimal DiemSo { get; set; }

        public string? GhiChu { get; set; }

        public DateTimeOffset NgayCapNhat { get; set; } = DateTimeOffset.UtcNow;
    }
}