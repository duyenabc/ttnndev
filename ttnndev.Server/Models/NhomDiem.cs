using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ttnndev.Server.Models
{
    [Table("NhomDiem")]
    public class NhomDiem
    {
        [Key] public int MaNhomDiem { get; set; }
        public int MaCauHinh { get; set; }
        public string TenNhom { get; set; } = null!;
    }
}