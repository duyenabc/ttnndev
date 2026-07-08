using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ttnndev.Server.Models
{
    [Table("PhienChat")]
    public class PhienChat
    {
        [Key] public long MaPhien { get; set; }
        public int MaNguoiDung { get; set; }
        public DateTimeOffset NgayTao { get; set; } = DateTimeOffset.UtcNow;
    }
}