using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ttnndev.Server.Models
{
    [Table("NhatKy")]
    public class NhatKy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaNhatKy { get; set; }

        [Required]
        public int MaGhiDanh { get; set; } // Liên kết với GhiDanhSinhVien

        [ForeignKey("MaGhiDanh")]
        public virtual GhiDanhSinhVien GhiDanh { get; set; } = null!;

        [Required]
        public int Tuan { get; set; }

        [Required]
        public DateTime NgayThucHien { get; set; }

        [Required]
        [MaxLength(2000)]
        public string CongViecDaLam { get; set; } = null!;

        [MaxLength(2000)]
        public string? KetQua { get; set; }

        [MaxLength(2000)]
        public string? KhoKhan { get; set; }

        [MaxLength(2000)]
        public string? KeHoachTuanToi { get; set; }

        [Required]
        [MaxLength(20)]
        public string TrangThaiDuyet { get; set; } = "ChoDuyet"; // ChoDuyet, DaDuyet, YeuCauSua

        public string? NhanXetGiangVien { get; set; }

        public DateTimeOffset NgayTao { get; set; } = DateTimeOffset.UtcNow;
    }
}