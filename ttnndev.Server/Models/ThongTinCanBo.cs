using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ttnndev.Server.Models
{
    // Thông tin mở rộng cho Giảng viên + Giáo vụ (liên kết 1-1 với NguoiDung)
    [Table("ThongTinCanBo")]
    public class ThongTinCanBo
    {
        [Key]
        public int MaThongTinCanBo { get; set; }

        [Required]
        public int MaCanBo { get; set; }

        [ForeignKey(nameof(MaCanBo))]
        public NguoiDung? CanBo { get; set; }

        [Required]
        public int MaKhoa { get; set; }

        [ForeignKey(nameof(MaKhoa))]
        public Khoa? Khoa { get; set; }

        public int? MaBoMon { get; set; }

        [ForeignKey(nameof(MaBoMon))]
        public BoMon? BoMon { get; set; }
    }
}
