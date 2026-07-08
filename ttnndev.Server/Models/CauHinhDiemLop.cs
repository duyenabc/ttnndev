using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ttnndev.Server.Models
{
    [Table("CauHinhDiemLop")]
    public class CauHinhDiemLop
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaCauHinh { get; set; }

        public int MaLop { get; set; }
        [ForeignKey("MaLop")]
        public virtual LopThucTap LopThucTap { get; set; } = null!;

        public short SoChuSoThapPhan { get; set; } = 1;
        public decimal ThangDiem { get; set; } = 10m;

        public virtual ICollection<NhomDiem> NhomDiems { get; set; } = new List<NhomDiem>();
    }
}