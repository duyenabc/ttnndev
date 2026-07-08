using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ttnndev.Server.Models
{
    public class TaiLieu
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string TenTaiLieu { get; set; } // Tên hiển thị

        [Required]
        public string DuongDanFile { get; set; } // Đường dẫn lưu trên server (vd: /uploads/files/abc.pdf)

        public string LoaiTaiLieu { get; set; } // "BieuMau", "HuongDan", "BaoCaoSinhVien"

        public DateTime NgayTaiLen { get; set; } = DateTime.Now;

        public bool TrangThaiAn { get; set; } = false; // true là bị ẩn

        // Người tải lên
        [ForeignKey("NguoiDung")]
        public int NguoiTaiLenId { get; set; } // Khớp với kiểu dữ liệu của MaDinhDanh
        public NguoiDung NguoiTaiLen { get; set; }
    }
}