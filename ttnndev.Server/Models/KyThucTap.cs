using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ttnndev.Server.Models
{
    [Table("KyThucTap")]
    public class KyThucTap
    {
        [Key]
        public int MaKy { get; set; }

        [Required]
        [MaxLength(200)]
        public string TenKy { get; set; }

        [Required]
        [MaxLength(9)]
        public string NamHoc { get; set; }

        [Required]
        [MaxLength(10)]
        public string HocKy { get; set; }

        [Required]
        [MaxLength(50)]
        public string LoaiThucTap { get; set; }

        // Ngày bắt đầu/kết thúc (DATE trong Postgres)
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }

        [Required]
        [MaxLength(20)]
        public string TrangThai { get; set; }

        // TIMESTAMPTZ
        public DateTimeOffset? NgayKhoaSoDiem { get; set; }

        // BOOLEAN
        public bool DaKhoaSoDiem { get; set; } = false;
        public bool DaKhoaVinhVien { get; set; } = false;

        public DateTimeOffset? NgayKhoaSoDiemThucTe { get; set; }

        public short SoNgayCanhBaoSom { get; set; } = 3;

        public int? KhoaBoi { get; set; }
        public int MaGiaoVuTao { get; set; }

        [MaxLength(50)]
        public string MaKhoa { get; set; } = "KTTH";

        public DateTimeOffset NgayTao { get; set; } = DateTimeOffset.UtcNow;
    }
}