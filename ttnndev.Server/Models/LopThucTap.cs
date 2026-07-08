using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ttnndev.Server.Models
{
    [Table("LopThucTap")]
    public class LopThucTap
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaLop { get; set; }

        public int MaKy { get; set; }
        [ForeignKey("MaKy")]
        public virtual KyThucTap KyThucTap { get; set; } = null!;

        public int MaGiangVien { get; set; }
        [ForeignKey("MaGiangVien")]
        public virtual NguoiDung GiangVien { get; set; } = null!;

        public int SoThuTuLop { get; set; }

        [Required]
        [MaxLength(100)]
        public string TenLop { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string MaThamGia { get; set; } = null!;

        public bool GhiDanhMo { get; set; } = true;
        public DateTimeOffset? HanGhiDanh { get; set; }

        public bool MoDangKyDeTaiCapLop { get; set; } = true;

        public int? KhoaDangKyBoi { get; set; }
        public DateTimeOffset? NgayKhoaDangKy { get; set; }

        [MaxLength(500)]
        public string? LinkHopMacDinh { get; set; }

        public DateTimeOffset NgayTao { get; set; } = DateTimeOffset.UtcNow;

        public virtual ICollection<GhiDanhSinhVien> DanhSachGhiDanh { get; set; } = new List<GhiDanhSinhVien>();
    }
}