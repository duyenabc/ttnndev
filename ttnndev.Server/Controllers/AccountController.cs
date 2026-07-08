using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ttnndev.Server.Data;
using ttnndev.Server.Models;

namespace ttnndev.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // E00.5 - Lấy danh sách tài khoản theo vai trò
        [HttpGet("users/{role}")]
        public async Task<IActionResult> GetUsersByRole(string role)
        {
            var users = await _context.NguoiDungs
                                      .Where(u => u.VaiTro == role && !u.DaXoa)
                                      .ToListAsync();
            return Ok(users);
        }

        // E00.2 - Import danh sách (Mô phỏng logic, bạn cần thêm thư viện EPPlus để đọc Excel)
        [HttpPost("import")]
        public async Task<IActionResult> ImportUsers([FromBody] List<NguoiDung> users)
        {
            var hasher = new PasswordHasher<NguoiDung>();
            foreach (var user in users)
            {
                // Logic kiểm tra trùng lặp mã định danh
                if (await _context.NguoiDungs.AnyAsync(u => u.MaDinhDanh == user.MaDinhDanh))
                    continue;
                user.MatKhauHash = hasher.HashPassword(user, user.MatKhauHash);
                _context.NguoiDungs.Add(user);
            }
            await _context.SaveChangesAsync();
            return Ok("Import thành công!");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            // 1. Tìm người dùng theo MaDinhDanh
            var user = await _context.NguoiDungs
                .FirstOrDefaultAsync(u => u.MaDinhDanh == model.MaDinhDanh && !u.DaXoa);

            if (user == null)
                return Unauthorized("Mã định danh không tồn tại hoặc tài khoản đã bị khóa!");

            // 2. Kiểm tra mật khẩu (Sử dụng PasswordHasher)
            var hasher = new PasswordHasher<NguoiDung>();
            var result = hasher.VerifyHashedPassword(user, user.MatKhauHash ?? "", model.MatKhau);

            if (result == PasswordVerificationResult.Failed)
                return Unauthorized("Mật khẩu không chính xác!");

            // 3. Đăng nhập thành công (Trả về thông tin user để lưu vào Pinia Store)
            return Ok(new
            {
                token = "fake-token-tam-thoi", // Sau này bạn sẽ cấu hình JWT thực sự
                user = new
                {
                    user.MaDinhDanh,
                    user.HoTen,
                    user.VaiTro
                }
            });
        }

        // Thêm class DTO này ngay dưới cùng của file (hoặc file riêng)
        public class LoginDto
        {
            public string MaDinhDanh { get; set; }
            public string MatKhau { get; set; }
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            // Kiểm tra xem đã tồn tại chưa
            if (await _context.NguoiDungs.AnyAsync(u => u.MaDinhDanh == model.MaDinhDanh))
                return BadRequest("Mã định danh này đã tồn tại!");

            // 1. Tạo đối tượng NguoiDung
            var hasher = new PasswordHasher<NguoiDung>();
            var user = new NguoiDung
            {
                MaDinhDanh = model.MaDinhDanh,
                HoTen = model.HoTen,
                Email = model.Email,
                VaiTro = model.VaiTro,
                TrangThaiTaiKhoan = "Nhap", // Giá trị mặc định bạn đã cấu hình
                DaXoa = false
            };

            // 2. Băm mật khẩu
            user.MatKhauHash = hasher.HashPassword(user, model.MatKhau);

            // 3. Lưu vào DB
            _context.NguoiDungs.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Đăng ký thành công!");
        }
    }
}