using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ttnndev.Server.Models
{
    // Nhóm sinh viên trong một lớp thực tập (E04.7/E04.8/E04.9)
    [Table("Nhom")]
    public class Nhom
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaNhom { get; set; }

        public int MaLop { get; set; }
        [ForeignKey("MaLop")]
        public virtual LopThucTap LopThucTap { get; set; } = null!;

        public int SoThuTu { get; set; }

        [Required]
        [MaxLength(100)]
        public string TenNhom { get; set; } = null!;

        public DateTimeOffset NgayTao { get; set; } = DateTimeOffset.UtcNow;
    }
}
