using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ttnndev.Server.Models
{
    [Table("Khoa")]
    public class Khoa
    {
        [Key]
        public int MaKhoa { get; set; }

        [Required]
        [MaxLength(100)]
        public string TenKhoa { get; set; } = null!;
    }
}
