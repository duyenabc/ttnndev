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

        [HttpGet("users/{role}")]
        public async Task<IActionResult> GetUsersByRole(string role)
        {
            var users = await _context.NguoiDungs
                                      .Where(u => u.VaiTro.ToLower() == role.ToLower() && !u.DaXoa)
                                      .ToListAsync();
            return Ok(users);
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportUsers([FromBody] List<NguoiDung> users)
        {
            var hasher = new PasswordHasher<NguoiDung>();
            foreach (var user in users)
            {
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
            var user = await _context.NguoiDungs
                .FirstOrDefaultAsync(u => u.MaDinhDanh == model.MaDinhDanh && !u.DaXoa);

            if (user == null)
                return Unauthorized("Mã định danh không tồn tại hoặc tài khoản đã bị khóa!");

            var hasher = new PasswordHasher<NguoiDung>();
            var result = hasher.VerifyHashedPassword(user, user.MatKhauHash ?? "", model.MatKhau);

            if (result == PasswordVerificationResult.Failed)
                return Unauthorized("Mật khẩu không chính xác!");

            return Ok(new
            {
                token = "fake-token-tam-thoi",
                user = new
                {
                    maDinhDanh = user.MaDinhDanh,
                    hoTen = user.HoTen,
                    vaiTro = user.VaiTro
                }
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (await _context.NguoiDungs.AnyAsync(u => u.MaDinhDanh == model.MaDinhDanh))
                return BadRequest("Mã định danh này đã tồn tại!");

            var hasher = new PasswordHasher<NguoiDung>();
            var user = new NguoiDung
            {
                MaDinhDanh = model.MaDinhDanh,
                HoTen = model.HoTen,
                Email = model.Email,
                VaiTro = model.VaiTro,
                TrangThaiTaiKhoan = "Nhap",
                DaXoa = false
            };

            user.MatKhauHash = hasher.HashPassword(user, model.MatKhau);

            _context.NguoiDungs.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Đăng ký thành công!");
        }

        // SEED - Tạo dữ liệu mẫu để test hệ thống
        [HttpPost("seed")]
        public async Task<IActionResult> SeedData()
        {
            if (await _context.NguoiDungs.AnyAsync(u => u.MaDinhDanh == "GV001" || u.MaDinhDanh == "SV001"))
                return Ok("Dữ liệu mẫu đã tồn tại!");

            var hasher = new PasswordHasher<NguoiDung>();

            // 1. Tạo Giảng viên mẫu
            var giangVien = new NguoiDung
            {
                MaDinhDanh = "GV001",
                HoTen = "Nguyễn Văn Thành",
                Email = "giangvien@test.com",
                VaiTro = "GiangVien",
                TrangThaiTaiKhoan = "HoatDong",
                SoDienThoai = "0901234567",
                DaXoa = false
            };
            giangVien.MatKhauHash = hasher.HashPassword(giangVien, "123");
            _context.NguoiDungs.Add(giangVien);

            // 2. Tạo Sinh viên mẫu
            var sinhVien = new NguoiDung
            {
                MaDinhDanh = "SV001",
                HoTen = "Trần Thị Lan",
                Email = "sinhvien@test.com",
                VaiTro = "SinhVien",
                TrangThaiTaiKhoan = "HoatDong",
                SoDienThoai = "0909876543",
                DaXoa = false
            };
            sinhVien.MatKhauHash = hasher.HashPassword(sinhVien, "123");
            _context.NguoiDungs.Add(sinhVien);

            await _context.SaveChangesAsync();

            // 3. Tạo Kỳ thực tập
            var kyThucTap = new KyThucTap
            {
                TenKy = "Thực tập tốt nghiệp HK1 2025-2026",
                NamHoc = "2025-2026",
                HocKy = "HK1",
                LoaiThucTap = "TotNghiep",
                NgayBatDau = new DateTime(2025, 9, 1),
                NgayKetThuc = new DateTime(2025, 12, 31),
                TrangThai = "DangDienRa",
                MaGiaoVuTao = giangVien.MaNguoiDung,
                MaKhoa = "KTTH"
            };
            _context.KyThucTaps.Add(kyThucTap);
            await _context.SaveChangesAsync();

            // 4. Tạo Lớp thực tập
            var lopThucTap = new LopThucTap
            {
                MaKy = kyThucTap.MaKy,
                MaGiangVien = giangVien.MaNguoiDung,
                SoThuTuLop = 1,
                TenLop = "Lớp Thực Tập A01",
                MaThamGia = "A01-2025",
                GhiDanhMo = true,
                MoDangKyDeTaiCapLop = true
            };
            _context.LopThucTaps.Add(lopThucTap);
            await _context.SaveChangesAsync();

            // 5. Ghi danh Sinh viên vào lớp
            var ghiDanh = new GhiDanhSinhVien
            {
                MaLop = lopThucTap.MaLop,
                MaSinhVien = sinhVien.MaNguoiDung,
                TrangThaiThucTap = "DangThucTap",
                GhiDanhBang = "MaLop"
            };
            _context.GhiDanhSinhViens.Add(ghiDanh);
            await _context.SaveChangesAsync();

            // 6. Tạo Đề tài mẫu của Sinh viên đang chờ duyệt
            var deTai = new DeTai
            {
                MaGhiDanh = ghiDanh.MaGhiDanh,
                TenDeTai = "Xây dựng hệ thống quản lý thực tập IMS",
                MoTa = "Xây dựng hệ thống quản lý thực tập trực tuyến cho khoa KTTH",
                TrangThai = "ChoDuyet",
                KhoaChinhSua = false
            };
            _context.DeTais.Add(deTai);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Seed dữ liệu thành công!",
                taiKhoanGiangVien = new { ma = "GV001", matKhau = "123" },
                taiKhoanSinhVien = new { ma = "SV001", matKhau = "123" }
            });
        }

        public class LoginDto
        {
            public string MaDinhDanh { get; set; }
            public string MatKhau { get; set; }
        }
    }
}