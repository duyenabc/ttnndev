using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ttnndev.Server.Models
{
    // Định nghĩa cột điểm của một lớp thực tập (E03.10 / E07.3)
    [Table("CotDiem")]
    public class CotDiem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaCotDiem { get; set; }

        public int MaLop { get; set; }
        [ForeignKey("MaLop")]
        public virtual LopThucTap LopThucTap { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string TenCot { get; set; } = null!;

        [Column(TypeName = "numeric(5,2)")]
        public decimal DiemToiDa { get; set; } = 10m;

        public int ThuTu { get; set; }

        public DateTimeOffset NgayTao { get; set; } = DateTimeOffset.UtcNow;
    }
}
