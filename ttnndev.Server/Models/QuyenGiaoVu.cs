using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ttnndev.Server.Models
{
    // Quyền quản lý người dùng của Giáo vụ (E01.1)
    [Table("QuyenGiaoVu")]
    public class QuyenGiaoVu
    {
        [Key]
        public int MaQuyenGiaoVu { get; set; }

        [Required]
        public int MaGiaoVu { get; set; }

        [ForeignKey(nameof(MaGiaoVu))]
        public NguoiDung? GiaoVu { get; set; }

        public bool QuyenQuanLyNguoiDung { get; set; } = false;

        public DateTimeOffset NgayCapGanNhat { get; set; } = DateTimeOffset.UtcNow;

        [Required]
        public int CapBoiGanNhat { get; set; }
    }
}
