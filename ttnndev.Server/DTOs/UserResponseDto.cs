using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ttnndev.Server.DTOs
{
    public class UserResponseDto
    {
        public int MaNguoiDung { get; set; }
        public string MaDinhDanh { get; set; } = null!;
        public string HoTen { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string VaiTro { get; set; } = null!;
    }
}
