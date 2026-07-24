using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ttnndev.Server.Models
{
    // Thông tin mở rộng cho Sinh viên (liên kết 1-1 với NguoiDung)
    [Table("ThongTinSinhVien")]
    public class ThongTinSinhVien
    {
        [Key]
        public int MaThongTinSinhVien { get; set; }

        [Required]
        public int MaSinhVien { get; set; }

        [ForeignKey(nameof(MaSinhVien))]
        public NguoiDung? SinhVien { get; set; }

        [Required]
        public int MaKhoa { get; set; }

        [ForeignKey(nameof(MaKhoa))]
        public Khoa? Khoa { get; set; }

        [MaxLength(20)]
        public string? LopSinhHoat { get; set; }
    }
}
