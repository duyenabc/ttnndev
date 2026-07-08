using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ttnndev.Server.Models
{
    [Table("CauHinhRubric")]
    public class CauHinhRubric
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaRubric { get; set; }

        public int MaSuKien { get; set; }

        [Required]
        [MaxLength(20)]
        public string PhuongThucTinh { get; set; } = "TongDiem"; // TongDiem, TrungBinh

        public virtual ICollection<TieuChiRubric> TieuChis { get; set; } = new List<TieuChiRubric>();
    }
}