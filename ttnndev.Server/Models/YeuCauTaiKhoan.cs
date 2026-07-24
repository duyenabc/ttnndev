using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ttnndev.Server.Models
{
    // Yêu cầu cấp/khóa tài khoản do Giáo vụ tạo, Admin xử lý (E02.2, E02.3, E00.5, E00.8)
    [Table("YeuCauTaiKhoan")]
    public class YeuCauTaiKhoan
    {
        [Key]
        public int MaYeuCau { get; set; }

        // CapTaiKhoan, KhoaTaiKhoan, MoKhoaTaiKhoan, CapQuyenGiaoVu, ThuHoiQuyenGiaoVu
        [Required]
        [MaxLength(20)]
        public string LoaiYeuCau { get; set; } = null!;

        [Required]
        public int MaNguoiYeuCau { get; set; }

        [ForeignKey(nameof(MaNguoiYeuCau))]
        public NguoiDung? NguoiYeuCau { get; set; }

        public int? MaDoiTuong { get; set; }

        [ForeignKey(nameof(MaDoiTuong))]
        public NguoiDung? DoiTuong { get; set; }

        public int? MaBatch { get; set; }

        [MaxLength(500)]
        public string? LyDoYeuCau { get; set; }

        // ChoXuLy, DaDuyet, TuChoi
        [Required]
        [MaxLength(20)]
        public string TrangThai { get; set; } = "ChoXuLy";

        public int? MaNguoiXuLy { get; set; }

        [ForeignKey(nameof(MaNguoiXuLy))]
        public NguoiDung? NguoiXuLy { get; set; }

        [MaxLength(500)]
        public string? LyDoTuChoi { get; set; }

        public DateTimeOffset NgayTao { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset? NgayXuLy { get; set; }
    }
}
