using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ttnndev.Server.Models
{
    public class LichHen
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string TieuDe { get; set; }

        public string NoiDung { get; set; }

        public DateTime ThoiGianHop { get; set; }

        public string LinkMeeting { get; set; } // Link Google Meet/Zoom

        public string TrangThai { get; set; } = "Chờ xác nhận"; // "Đã xác nhận", "Đã hủy", "Hoàn thành"

        // Giảng viên tạo lịch
        [ForeignKey("GiangVien")]
        public int GiangVienId { get; set; }
        public NguoiDung GiangVien { get; set; }

        // Có thể hẹn với 1 sinh viên cụ thể hoặc 1 nhóm (tuỳ logic của bạn)
        [ForeignKey("SinhVien")]
        public int SinhVienId { get; set; }
        public NguoiDung SinhVien { get; set; }
    }
}