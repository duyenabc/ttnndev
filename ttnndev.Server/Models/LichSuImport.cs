using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ttnndev.Server.Models
{
    // Batch import danh sách người dùng (E00.2, E02.1)
    [Table("LichSuImport")]
    public class LichSuImport
    {
        [Key]
        public int MaBatch { get; set; }

        [Required]
        public int MaNguoiThucHien { get; set; }

        [ForeignKey(nameof(MaNguoiThucHien))]
        public NguoiDung? NguoiThucHien { get; set; }

        // GiangVien, GiaoVu, SinhVien
        [Required]
        [MaxLength(20)]
        public string VaiTroImport { get; set; } = null!;

        public int? MaLop { get; set; }

        public int SoHopLe { get; set; } = 0;

        public int SoGhiDe { get; set; } = 0;

        public int SoLoi { get; set; } = 0;

        public DateTimeOffset NgayThucHien { get; set; } = DateTimeOffset.UtcNow;
    }
}
