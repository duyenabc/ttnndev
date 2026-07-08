using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ttnndev.Server.Models
{
    [Table("TieuChiRubric")]
    public class TieuChiRubric
    {
        [Key] public int MaTieuChi { get; set; }
        public int MaRubric { get; set; }
        public string TenTieuChi { get; set; } = null!;
    }
}