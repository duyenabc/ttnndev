using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ttnndev.Server.Models
{
    [Table("BoMon")]
    public class BoMon
    {
        [Key]
        public int MaBoMon { get; set; }

        [Required]
        [MaxLength(100)]
        public string TenBoMon { get; set; } = null!;

        [Required]
        public int MaKhoa { get; set; }

        [ForeignKey(nameof(MaKhoa))]
        public Khoa? Khoa { get; set; }
    }
}
