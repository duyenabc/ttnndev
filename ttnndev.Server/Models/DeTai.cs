using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ttnndev.Server.Models
{
    [Table("DeTai")]
    public class DeTai
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaDeTai { get; set; }

        public int MaGhiDanh { get; set; }

        [MaxLength(500)]
        [Required]
        public string TenDeTai { get; set; } = null!;

        [Required]
        public string MoTa { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string TrangThai { get; set; } = "Nhap"; // Nhap, ChoDuyet, YeuCauSua, DaDuyet, TuChoi

        public bool KhoaChinhSua { get; set; } = false;

        public DateTimeOffset NgayTao { get; set; } = DateTimeOffset.UtcNow;
    }
}