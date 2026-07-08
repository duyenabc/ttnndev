using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ttnndev.Server.Models
{
    [Table("TinNhanChat")]
    public class TinNhanChat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long MaTinNhan { get; set; }

        public long MaPhien { get; set; }
        [ForeignKey("MaPhien")]
        public virtual PhienChat PhienChat { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string VaiTroGui { get; set; } = null!; // NguoiDung, AI

        [Required]
        [MaxLength(1000)]
        public string NoiDung { get; set; } = null!;

        [MaxLength(500)]
        public string? LinkThamChieu { get; set; }

        public DateTimeOffset ThoiDiem { get; set; } = DateTimeOffset.UtcNow;
    }
}