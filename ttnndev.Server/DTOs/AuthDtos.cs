namespace ttnndev.Server.DTOs
{
    public class LoginDto
    {
        public string MaDinhDanh { get; set; } = null!;
        public string MatKhau { get; set; } = null!;
    }

    public class AuthUserDto
    {
        public int MaNguoiDung { get; set; }
        public string MaDinhDanh { get; set; } = null!;
        public string HoTen { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string VaiTro { get; set; } = null!;
        public string TrangThaiTaiKhoan { get; set; } = null!;
        public bool BuocDoiMatKhau { get; set; }
        public string? AnhDaiDien { get; set; }
        public bool QuyenQuanLyNguoiDung { get; set; }
    }

    public class LoginResultDto
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public bool BuocDoiMatKhau { get; set; }
        public string RedirectTo { get; set; } = "/dashboard";
        public AuthUserDto User { get; set; } = null!;
    }

    public class RefreshDto
    {
        public string RefreshToken { get; set; } = null!;
    }

    public class ChangePasswordDto
    {
        public string MatKhauHienTai { get; set; } = null!;
        public string MatKhauMoi { get; set; } = null!;
        public string XacNhanMatKhau { get; set; } = null!;
    }

    public class ForgotPasswordDto
    {
        public string Email { get; set; } = null!;
    }

    public class ResetPasswordDto
    {
        public string Token { get; set; } = null!;
        public string MatKhauMoi { get; set; } = null!;
        public string XacNhanMatKhau { get; set; } = null!;
    }

    public class ActivateDto
    {
        public string Token { get; set; } = null!;
        public string MatKhauMoi { get; set; } = null!;
        public string XacNhanMatKhau { get; set; } = null!;
    }

    public class ResendActivationDto
    {
        public string Email { get; set; } = null!;
    }

    public class UpdateProfileDto
    {
        public string? AnhDaiDien { get; set; }
    }

    public class ProfileDto
    {
        public int MaNguoiDung { get; set; }
        public string MaDinhDanh { get; set; } = null!;
        public string HoTen { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string VaiTro { get; set; } = null!;
        public string? SoDienThoai { get; set; }
        public string? AnhDaiDien { get; set; }
        public string? TenKhoa { get; set; }
        public string? TenBoMon { get; set; }
        public string? LopSinhHoat { get; set; }
    }
}
