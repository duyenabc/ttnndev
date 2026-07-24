using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ttnndev.Server.Data;
using ttnndev.Server.Models;

namespace ttnndev.Server.Controllers
{
    // Bootstrap dữ liệu tài khoản để phát triển / kiểm thử (chỉ chạy khi chưa có Admin)
    [ApiController]
    [Route("api/seed")]
    public class SeedController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<NguoiDung> _hasher = new();

        public SeedController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("accounts")]
        public async Task<IActionResult> SeedAccounts()
        {
            if (await _context.NguoiDungs.AnyAsync(u => u.VaiTro == "Admin" && !u.DaXoa))
                return Ok(new { message = "Đã có Admin, bỏ qua seed." });

            var khoa = await _context.Khoas.FirstOrDefaultAsync(k => k.TenKhoa == "Khoa Thống kê - Tin học");
            if (khoa == null)
            {
                khoa = new Khoa { TenKhoa = "Khoa Thống kê - Tin học" };
                _context.Khoas.Add(khoa);
                await _context.SaveChangesAsync();
            }

            NguoiDung Create(string ma, string ten, string email, string role, string status, string? pwd)
            {
                var u = new NguoiDung
                {
                    MaDinhDanh = ma,
                    HoTen = ten,
                    Email = email,
                    VaiTro = role,
                    TrangThaiTaiKhoan = status,
                    NgayTao = DateTimeOffset.UtcNow,
                    NgayCapNhat = DateTimeOffset.UtcNow
                };
                if (pwd != null) u.MatKhauHash = _hasher.HashPassword(u, pwd);
                _context.NguoiDungs.Add(u);
                return u;
            }

            var admin = Create("admin", "Quản trị viên", "admin@due.udn.vn", "Admin", "DangHoatDong", "Admin@123");
            var gvu = Create("gvu001", "Lê Thị Giáo Vụ", "giaovu@due.udn.vn", "GiaoVu", "DangHoatDong", "Test@1234");
            var gv = Create("gv001", "Nguyễn Văn Thành", "giangvien@due.udn.vn", "GiangVien", "DangHoatDong", "Test@1234");
            var sv = Create("sv001", "Trần Thị Lan", "sinhvien@due.udn.vn", "SinhVien", "DangHoatDong", "Test@1234");
            Create("gv002", "Phạm Văn Chờ", "cho.gv@due.udn.vn", "GiangVien", "ChoKichHoat", null);
            Create("sv002", "Hoàng Thị Nháp", "nhap.sv@due.udn.vn", "SinhVien", "Nhap", null);
            Create("sv003", "Đỗ Văn Khóa", "khoa.sv@due.udn.vn", "SinhVien", "BiKhoa", "Test@1234");
            await _context.SaveChangesAsync();

            _context.QuyenGiaoVus.Add(new QuyenGiaoVu
            {
                MaGiaoVu = gvu.MaNguoiDung,
                QuyenQuanLyNguoiDung = false,
                NgayCapGanNhat = DateTimeOffset.UtcNow,
                CapBoiGanNhat = admin.MaNguoiDung
            });
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Seed tài khoản thành công",
                taiKhoan = new object[]
                {
                    new { ma = "admin", matKhau = "Admin@123", vaiTro = "Admin" },
                    new { ma = "gvu001", matKhau = "Test@1234", vaiTro = "GiaoVu" },
                    new { ma = "gv001", matKhau = "Test@1234", vaiTro = "GiangVien" },
                    new { ma = "sv001", matKhau = "Test@1234", vaiTro = "SinhVien" }
                }
            });
        }
    }
}
