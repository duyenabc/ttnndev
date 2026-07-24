using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ttnndev.Server.Data;
using ttnndev.Server.DTOs;
using ttnndev.Server.Models;
using ttnndev.Server.Services;

namespace ttnndev.Server.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly PasswordHasher<NguoiDung> _hasher = new();

        private const int MaxFailed = 3;
        private const int LockMinutes = 15;

        public AuthController(AppDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        // E15.1 - Đăng nhập
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (string.IsNullOrWhiteSpace(model.MaDinhDanh))
                return BadRequest(new { message = "Vui lòng nhập mã định danh" });
            if (string.IsNullOrWhiteSpace(model.MatKhau))
                return BadRequest(new { message = "Vui lòng nhập mật khẩu" });

            var user = await _context.NguoiDungs
                .FirstOrDefaultAsync(u => u.MaDinhDanh == model.MaDinhDanh && !u.DaXoa);

            // Không tiết lộ tài khoản có tồn tại hay không
            if (user == null)
                return Unauthorized(new { message = "Mã định danh hoặc mật khẩu không đúng" });

            if (user.TrangThaiTaiKhoan == "BiKhoa")
                return Unauthorized(new { message = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên", code = "LOCKED" });

            if (user.KhoaDangNhapDenLuc.HasValue && user.KhoaDangNhapDenLuc.Value > DateTimeOffset.UtcNow)
                return Unauthorized(new { message = $"Tài khoản tạm thời bị khóa do đăng nhập sai nhiều lần. Vui lòng thử lại sau {LockMinutes} phút", code = "TEMP_LOCK" });

            if (user.TrangThaiTaiKhoan == "ChoKichHoat")
                return Unauthorized(new { message = "Tài khoản chưa được kích hoạt. Vui lòng kiểm tra email để đặt mật khẩu", code = "NOT_ACTIVATED" });

            // Nhap = chưa cấp tài khoản → coi như thông tin đăng nhập không hợp lệ
            if (user.TrangThaiTaiKhoan == "Nhap" || string.IsNullOrEmpty(user.MatKhauHash))
                return Unauthorized(new { message = "Mã định danh hoặc mật khẩu không đúng" });

            var verify = _hasher.VerifyHashedPassword(user, user.MatKhauHash, model.MatKhau);
            if (verify == PasswordVerificationResult.Failed)
            {
                user.SoLanDangNhapSai += 1;
                user.NgayCapNhat = DateTimeOffset.UtcNow;
                if (user.SoLanDangNhapSai >= MaxFailed)
                {
                    user.KhoaDangNhapDenLuc = DateTimeOffset.UtcNow.AddMinutes(LockMinutes);
                    await _context.SaveChangesAsync();
                    return Unauthorized(new { message = $"Tài khoản tạm thời bị khóa do đăng nhập sai nhiều lần. Vui lòng thử lại sau {LockMinutes} phút", code = "TEMP_LOCK" });
                }
                await _context.SaveChangesAsync();
                return Unauthorized(new { message = "Mã định danh hoặc mật khẩu không đúng" });
            }

            // Đăng nhập thành công → reset bộ đếm
            user.SoLanDangNhapSai = 0;
            user.KhoaDangNhapDenLuc = null;
            user.NgayCapNhat = DateTimeOffset.UtcNow;

            var result = await IssueTokensAsync(user);
            await _context.SaveChangesAsync();
            return Ok(result);
        }

        // E15.7 - làm mới access token bằng refresh token
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshDto model)
        {
            if (string.IsNullOrWhiteSpace(model.RefreshToken))
                return Unauthorized(new { message = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại" });

            var hash = _tokenService.HashToken(model.RefreshToken);
            var token = await _context.RefreshTokens
                .Include(t => t.NguoiDung)
                .FirstOrDefaultAsync(t => t.TokenHash == hash);

            if (token == null || token.DaThuHoi || token.ThoiDiemHetHan <= DateTimeOffset.UtcNow)
                return Unauthorized(new { message = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại" });

            var user = token.NguoiDung!;
            if (user.DaXoa || user.TrangThaiTaiKhoan == "BiKhoa")
                return Unauthorized(new { message = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên" });

            // rotate: thu hồi token cũ, phát token mới
            token.DaThuHoi = true;
            var result = await IssueTokensAsync(user);
            await _context.SaveChangesAsync();
            return Ok(result);
        }

        // E15.2 - Đăng xuất (thu hồi refresh token của phiên hiện tại)
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshDto? model)
        {
            if (model != null && !string.IsNullOrWhiteSpace(model.RefreshToken))
            {
                var hash = _tokenService.HashToken(model.RefreshToken);
                var token = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash);
                if (token != null)
                {
                    token.DaThuHoi = true;
                    await _context.SaveChangesAsync();
                }
            }
            return Ok(new { message = "Đã đăng xuất" });
        }

        // E15.3 - Đổi mật khẩu
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return Unauthorized();

            if (string.IsNullOrEmpty(model.MatKhauHienTai))
                return BadRequest(new { message = "Vui lòng nhập mật khẩu hiện tại" });

            if (!string.IsNullOrEmpty(user.MatKhauHash))
            {
                var verify = _hasher.VerifyHashedPassword(user, user.MatKhauHash, model.MatKhauHienTai);
                if (verify == PasswordVerificationResult.Failed)
                    return BadRequest(new { message = "Mật khẩu hiện tại không đúng" });
            }

            if (!PasswordPolicy.IsValid(model.MatKhauMoi))
                return BadRequest(new { message = "Mật khẩu phải có ít nhất 8 ký tự, gồm chữ hoa, chữ thường và số" });

            if (model.MatKhauMoi == model.MatKhauHienTai)
                return BadRequest(new { message = "Mật khẩu mới không được trùng mật khẩu hiện tại" });

            if (model.MatKhauMoi != model.XacNhanMatKhau)
                return BadRequest(new { message = "Mật khẩu xác nhận không khớp" });

            user.MatKhauHash = _hasher.HashPassword(user, model.MatKhauMoi);
            user.BuocDoiMatKhau = false;
            user.TokenValidFrom = DateTimeOffset.UtcNow; // đăng xuất các thiết bị khác
            user.NgayCapNhat = DateTimeOffset.UtcNow;
            await RevokeAllRefreshTokensAsync(user.MaNguoiDung);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Đổi mật khẩu thành công" });
        }

        // E15.4 - Quên mật khẩu (gửi link reset)
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
                return BadRequest(new { message = "Vui lòng nhập email của bạn" });

            var user = await _context.NguoiDungs
                .FirstOrDefaultAsync(u => u.Email == model.Email && !u.DaXoa);

            if (user == null)
                return NotFound(new { message = "Không tìm thấy tài khoản. Vui lòng kiểm tra lại hoặc liên hệ quản trị viên." });
            if (user.TrangThaiTaiKhoan == "ChoKichHoat" || user.TrangThaiTaiKhoan == "Nhap")
                return BadRequest(new { message = "Tài khoản chưa kích hoạt. Vui lòng kiểm tra email kích hoạt." });
            if (user.TrangThaiTaiKhoan == "BiKhoa")
                return BadRequest(new { message = "Tài khoản đã bị khóa. Vui lòng liên hệ quản trị viên." });

            var link = await CreateVerificationLinkAsync(user, "DatLaiMatKhau", 30, "/reset-password");

            return Ok(new
            {
                message = "Chúng tôi đã gửi link đặt lại mật khẩu đến email đã đăng ký của bạn.",
                // DEV ONLY: chưa cấu hình SMTP nên trả link để kiểm thử
                devResetLink = link
            });
        }

        // E15.4 - Đặt lại mật khẩu
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (!PasswordPolicy.IsValid(model.MatKhauMoi))
                return BadRequest(new { message = "Mật khẩu phải có ít nhất 8 ký tự, gồm chữ hoa, chữ thường và số" });
            if (model.MatKhauMoi != model.XacNhanMatKhau)
                return BadRequest(new { message = "Mật khẩu xác nhận không khớp" });

            var req = await FindValidTokenAsync(model.Token, "DatLaiMatKhau");
            if (req == null)
                return BadRequest(new { message = "Link đã hết hạn.", code = "EXPIRED" });

            var user = req.NguoiDung!;
            user.MatKhauHash = _hasher.HashPassword(user, model.MatKhauMoi);
            user.TokenValidFrom = DateTimeOffset.UtcNow;
            user.BuocDoiMatKhau = false;
            user.NgayCapNhat = DateTimeOffset.UtcNow;
            req.DaSuDung = true;
            await RevokeAllRefreshTokensAsync(user.MaNguoiDung);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Đặt lại mật khẩu thành công" });
        }

        // E15.6 - Kiểm tra token kích hoạt
        [HttpGet("activation/{token}")]
        public async Task<IActionResult> CheckActivation(string token)
        {
            var hash = _tokenService.HashToken(token);
            var req = await _context.YeuCauXacThucs.Include(r => r.NguoiDung)
                .FirstOrDefaultAsync(r => r.TokenHash == hash && r.LoaiYeuCau == "KichHoat");

            if (req == null)
                return BadRequest(new { message = "Link kích hoạt không hợp lệ.", code = "INVALID" });

            var user = req.NguoiDung!;
            if (user.TrangThaiTaiKhoan == "BiKhoa")
                return BadRequest(new { message = "Tài khoản đang bị khóa, vui lòng liên hệ quản trị viên để biết thêm chi tiết.", code = "LOCKED" });
            if (req.DaSuDung || user.TrangThaiTaiKhoan == "DangHoatDong")
                return BadRequest(new { message = "Tài khoản này đã được kích hoạt. Vui lòng đăng nhập bình thường.", code = "USED" });
            if (req.ThoiDiemHetHan <= DateTimeOffset.UtcNow)
                return BadRequest(new { message = "Link kích hoạt đã hết hạn (hiệu lực 72 giờ).", code = "EXPIRED" });

            return Ok(new { hoTen = user.HoTen, maDinhDanh = user.MaDinhDanh, email = user.Email });
        }

        // E15.6 - Kích hoạt tài khoản (đặt mật khẩu lần đầu)
        [HttpPost("activate")]
        public async Task<IActionResult> Activate([FromBody] ActivateDto model)
        {
            if (!PasswordPolicy.IsValid(model.MatKhauMoi))
                return BadRequest(new { message = "Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường và số" });
            if (model.MatKhauMoi != model.XacNhanMatKhau)
                return BadRequest(new { message = "Mật khẩu xác nhận không khớp" });

            var hash = _tokenService.HashToken(model.Token);
            var req = await _context.YeuCauXacThucs.Include(r => r.NguoiDung)
                .FirstOrDefaultAsync(r => r.TokenHash == hash && r.LoaiYeuCau == "KichHoat");
            if (req == null)
                return BadRequest(new { message = "Link kích hoạt không hợp lệ.", code = "INVALID" });

            var user = req.NguoiDung!;
            if (user.TrangThaiTaiKhoan == "BiKhoa")
                return BadRequest(new { message = "Tài khoản đang bị khóa, vui lòng liên hệ quản trị viên để biết thêm chi tiết.", code = "LOCKED" });
            if (req.DaSuDung || user.TrangThaiTaiKhoan == "DangHoatDong")
                return BadRequest(new { message = "Tài khoản này đã được kích hoạt. Vui lòng đăng nhập bình thường.", code = "USED" });
            if (req.ThoiDiemHetHan <= DateTimeOffset.UtcNow)
                return BadRequest(new { message = "Link kích hoạt đã hết hạn (hiệu lực 72 giờ).", code = "EXPIRED" });

            user.MatKhauHash = _hasher.HashPassword(user, model.MatKhauMoi);
            user.TrangThaiTaiKhoan = "DangHoatDong";
            user.BuocDoiMatKhau = false;
            user.TokenValidFrom = DateTimeOffset.UtcNow;
            user.NgayCapNhat = DateTimeOffset.UtcNow;
            req.DaSuDung = true;

            var result = await IssueTokensAsync(user);
            result.RedirectTo = RoleWelcomeRedirect(user.VaiTro);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                accessToken = result.AccessToken,
                refreshToken = result.RefreshToken,
                buocDoiMatKhau = result.BuocDoiMatKhau,
                redirectTo = result.RedirectTo,
                user = result.User,
                welcomeMessage = RoleWelcomeMessage(user.VaiTro)
            });
        }

        // E15.6 - Gửi lại link kích hoạt
        [HttpPost("resend-activation")]
        public async Task<IActionResult> ResendActivation([FromBody] ResendActivationDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
                return BadRequest(new { message = "Vui lòng nhập email của bạn" });

            var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.Email == model.Email && !u.DaXoa);
            if (user == null)
                return NotFound(new { message = "Không tìm thấy tài khoản. Vui lòng kiểm tra lại hoặc liên hệ quản trị viên." });
            if (user.TrangThaiTaiKhoan == "DangHoatDong")
                return BadRequest(new { message = "Tài khoản này đã được kích hoạt. Vui lòng đăng nhập bình thường." });
            if (user.TrangThaiTaiKhoan == "BiKhoa")
                return BadRequest(new { message = "Tài khoản đang bị khóa, vui lòng liên hệ quản trị viên để biết thêm chi tiết." });

            // Tối đa 3 lần/24 giờ
            var since = DateTimeOffset.UtcNow.AddHours(-24);
            var count = await _context.YeuCauXacThucs.CountAsync(r =>
                r.MaNguoiDung == user.MaNguoiDung && r.LoaiYeuCau == "KichHoat" && r.ThoiDiemTao >= since);
            if (count >= 3)
                return BadRequest(new { message = "Bạn đã yêu cầu quá nhiều lần. Vui lòng liên hệ quản trị viên để được hỗ trợ." });

            var link = await CreateVerificationLinkAsync(user, "KichHoat", 72 * 60, "/activate");
            return Ok(new { message = $"Đã gửi lại link kích hoạt cho {user.HoTen}", devActivationLink = link });
        }

        // E15.5 - Xem hồ sơ cá nhân
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return Unauthorized();

            var dto = new ProfileDto
            {
                MaNguoiDung = user.MaNguoiDung,
                MaDinhDanh = user.MaDinhDanh,
                HoTen = user.HoTen,
                Email = user.Email,
                VaiTro = user.VaiTro,
                SoDienThoai = user.SoDienThoai,
                AnhDaiDien = user.AnhDaiDien
            };

            if (user.VaiTro == "SinhVien")
            {
                var tt = await _context.ThongTinSinhViens.Include(t => t.Khoa)
                    .FirstOrDefaultAsync(t => t.MaSinhVien == user.MaNguoiDung);
                dto.TenKhoa = tt?.Khoa?.TenKhoa;
                dto.LopSinhHoat = tt?.LopSinhHoat;
            }
            else if (user.VaiTro == "GiangVien" || user.VaiTro == "GiaoVu")
            {
                var tt = await _context.ThongTinCanBos.Include(t => t.Khoa).Include(t => t.BoMon)
                    .FirstOrDefaultAsync(t => t.MaCanBo == user.MaNguoiDung);
                dto.TenKhoa = tt?.Khoa?.TenKhoa;
                dto.TenBoMon = tt?.BoMon?.TenBoMon;
            }
            return Ok(dto);
        }

        // E15.5 - Cập nhật hồ sơ (chỉ avatar)
        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateProfileDto model)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return Unauthorized();
            user.AnhDaiDien = model.AnhDaiDien;
            user.NgayCapNhat = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật hồ sơ thành công" });
        }

        // ---------- Helpers ----------

        private async Task<LoginResultDto> IssueTokensAsync(NguoiDung user)
        {
            var access = _tokenService.CreateAccessToken(user);
            var (plain, hash, expires) = _tokenService.CreateRefreshToken();
            _context.RefreshTokens.Add(new RefreshToken
            {
                MaNguoiDung = user.MaNguoiDung,
                TokenHash = hash,
                ThoiDiemTao = DateTimeOffset.UtcNow,
                ThoiDiemHetHan = expires,
                DaThuHoi = false
            });

            bool quyenQL = false;
            if (user.VaiTro == "GiaoVu")
            {
                quyenQL = await _context.QuyenGiaoVus
                    .AnyAsync(q => q.MaGiaoVu == user.MaNguoiDung && q.QuyenQuanLyNguoiDung);
            }

            return new LoginResultDto
            {
                AccessToken = access,
                RefreshToken = plain,
                BuocDoiMatKhau = user.BuocDoiMatKhau,
                RedirectTo = user.BuocDoiMatKhau ? "/change-password" : RoleRedirect(user.VaiTro),
                User = new AuthUserDto
                {
                    MaNguoiDung = user.MaNguoiDung,
                    MaDinhDanh = user.MaDinhDanh,
                    HoTen = user.HoTen,
                    Email = user.Email,
                    VaiTro = user.VaiTro,
                    TrangThaiTaiKhoan = user.TrangThaiTaiKhoan,
                    BuocDoiMatKhau = user.BuocDoiMatKhau,
                    AnhDaiDien = user.AnhDaiDien,
                    QuyenQuanLyNguoiDung = quyenQL
                }
            };
        }

        private static string RoleRedirect(string role) => role switch
        {
            "Admin" => "/admin/accounts",
            _ => "/dashboard"
        };

        private static string RoleWelcomeRedirect(string role) => role switch
        {
            "Admin" => "/admin/accounts",
            _ => "/dashboard"
        };

        private static string RoleWelcomeMessage(string role) => role switch
        {
            "GiangVien" => "Chào mừng! Bắt đầu bằng cách tạo lớp thực tập đầu tiên của bạn.",
            "SinhVien" => "Chào mừng! Hãy ghi danh tham gia lớp thực tập của bạn.",
            "GiaoVu" => "Chào mừng! Bắt đầu bằng cách tạo kỳ thực tập đầu tiên của bạn.",
            _ => "Chào mừng bạn đến với IMS!"
        };

        private async Task RevokeAllRefreshTokensAsync(int userId)
        {
            var tokens = await _context.RefreshTokens
                .Where(t => t.MaNguoiDung == userId && !t.DaThuHoi).ToListAsync();
            foreach (var t in tokens) t.DaThuHoi = true;
        }

        private async Task<string> CreateVerificationLinkAsync(NguoiDung user, string loai, int minutes, string path)
        {
            // vô hiệu các token cùng loại còn hiệu lực
            var old = await _context.YeuCauXacThucs
                .Where(r => r.MaNguoiDung == user.MaNguoiDung && r.LoaiYeuCau == loai && !r.DaSuDung)
                .ToListAsync();
            foreach (var o in old) o.DaSuDung = true;

            var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(48);
            var plain = Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
            _context.YeuCauXacThucs.Add(new YeuCauXacThuc
            {
                MaNguoiDung = user.MaNguoiDung,
                LoaiYeuCau = loai,
                TokenHash = _tokenService.HashToken(plain),
                ThoiDiemTao = DateTimeOffset.UtcNow,
                ThoiDiemHetHan = DateTimeOffset.UtcNow.AddMinutes(minutes),
                DaSuDung = false
            });
            await _context.SaveChangesAsync();
            return $"{path}?token={plain}";
        }

        private async Task<YeuCauXacThuc?> FindValidTokenAsync(string token, string loai)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;
            var hash = _tokenService.HashToken(token);
            var req = await _context.YeuCauXacThucs.Include(r => r.NguoiDung)
                .FirstOrDefaultAsync(r => r.TokenHash == hash && r.LoaiYeuCau == loai);
            if (req == null || req.DaSuDung || req.ThoiDiemHetHan <= DateTimeOffset.UtcNow)
                return null;
            return req;
        }

        private async Task<NguoiDung?> GetCurrentUserAsync()
        {
            var sub = User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                      ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(sub, out var id)) return null;
            return await _context.NguoiDungs.FirstOrDefaultAsync(u => u.MaNguoiDung == id && !u.DaXoa);
        }
    }
}
