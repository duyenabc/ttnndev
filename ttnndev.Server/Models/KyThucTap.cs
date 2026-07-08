using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ttnndev.Server.Models
{
    [Table("KyThucTap")]
    public class KyThucTap
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaKy { get; set; }

        [Required]
        [MaxLength(9)]
        public string NamHoc { get; set; } = null!;

        [Required]
        [MaxLength(10)]
        public string HocKy { get; set; } = null!; // HK1, HK2, He

        [Required]
        public int MaLoaiThucTap { get; set; }

        [Required]
        public DateTime NgayBatDau { get; set; }

        [Required]
        public DateTime NgayKetThuc { get; set; }

        [Required]
        [MaxLength(10)]
        public string TrangThaiCongBo { get; set; } = "Nhap"; // Nhap, CongBo

        [Required]
        [MaxLength(20)]
        public string TrangThaiKy { get; set; } = null!; // SapDienRa, DangDienRa, DaKetThuc

        public DateTimeOffset? NgayKhoaSoDiem { get; set; }
        public bool DaKhoaSoDiem { get; set; } = false;
        public DateTimeOffset? NgayKhoaSoDiemThucTe { get; set; }
        public short SoNgayCanhBaoSom { get; set; } = 3;

        public int? KhoaBoi { get; set; }
        public int MaGiaoVuTao { get; set; }
        public int MaKhoa { get; set; } = 1;

        public DateTimeOffset NgayTao { get; set; } = DateTimeOffset.UtcNow;
        public bool DaKhoaVinhVien { get; set; } = false;

        public virtual ICollection<LopThucTap> LopThucTaps { get; set; } = new List<LopThucTap>();
    }
}