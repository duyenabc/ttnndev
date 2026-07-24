using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ttnndev.Server.Data;
using ttnndev.Server.DTOs;
using ttnndev.Server.Models;
using ttnndev.Server.Services;

namespace ttnndev.Server.Controllers
{
    // Thao tác của Giáo vụ khoa (E02): gửi yêu cầu cấp/khóa/mở khóa tài khoản, xóa user chưa phát sinh dữ liệu
    [ApiController]
    [Route("api/giaovu")]
    [Authorize(Roles = "GiaoVu")]
    public class GiaoVuController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAuditService _audit;

        private static readonly string[] ManageableRoles = { "GiangVien", "GiaoVu", "SinhVien" };

        public GiaoVuController(AppDbContext context, IAuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        // E02.2/E02.3 - Tạo yêu cầu gửi Admin
        [HttpPost("requests")]
        public async Task<IActionResult> CreateRequest([FromBody] CreateAccountRequestDto model)
        {
            var actorId = CurrentUserId();
            var loai = model.LoaiYeuCau;
            if (loai != "CapTaiKhoan" && loai != "KhoaTaiKhoan" && loai != "MoKhoaTaiKhoan")
                return BadRequest(new { message = "Loại yêu cầu không hợp lệ" });

            int? maDoiTuong = model.MaDoiTuong;

            if (loai == "CapTaiKhoan")
            {
                // E02.2: tạo user Nháp rồi gửi yêu cầu cấp
                var m = model.NguoiDungMoi;
                if (m == null) return BadRequest(new { message = "Thiếu thông tin người dùng" });
                if (!ManageableRoles.Contains(m.VaiTro))
                    return BadRequest(new { message = "Vai trò không hợp lệ" });
                if (string.IsNullOrWhiteSpace(m.MaDinhDanh) || string.IsNullOrWhiteSpace(m.HoTen))
                    return BadRequest(new { message = "Thiếu mã định danh hoặc họ tên" });
                if (string.IsNullOrWhiteSpace(m.Email) || !m.Email.Contains('@'))
                    return BadRequest(new { message = "Email không hợp lệ" });
                if (await _context.NguoiDungs.AnyAsync(u => u.MaDinhDanh == m.MaDinhDanh && !u.DaXoa))
                    return BadRequest(new { message = "Mã định danh đã tồn tại" });
                if (await _context.NguoiDungs.AnyAsync(u => u.Email == m.Email && !u.DaXoa))
                    return BadRequest(new { message = "Email đã tồn tại" });

                var user = new NguoiDung
                {
                    MaDinhDanh = m.MaDinhDanh.Trim(),
                    HoTen = m.HoTen.Trim(),
                    Email = m.Email.Trim(),
                    SoDienThoai = m.SoDienThoai,
                    VaiTro = m.VaiTro,
                    TrangThaiTaiKhoan = "Nhap",
                    NgayTao = DateTimeOffset.UtcNow,
                    NgayCapNhat = DateTimeOffset.UtcNow
                };
                _context.NguoiDungs.Add(user);
                await _context.SaveChangesAsync();
                maDoiTuong = user.MaNguoiDung;
                await _audit.LogAsync(actorId, "ThemNguoiDung", user.MaNguoiDung);
            }
            else
            {
                // Khóa/mở khóa cần lý do
                if (string.IsNullOrWhiteSpace(model.LyDo))
                    return BadRequest(new { message = "Vui lòng nhập lý do" });
                if (maDoiTuong == null)
                    return BadRequest(new { message = "Thiếu tài khoản mục tiêu" });
            }

            var req = new YeuCauTaiKhoan
            {
                LoaiYeuCau = loai,
                MaNguoiYeuCau = actorId,
                MaDoiTuong = maDoiTuong,
                LyDoYeuCau = model.LyDo,
                TrangThai = "ChoXuLy",
                NgayTao = DateTimeOffset.UtcNow
            };
            _context.YeuCauTaiKhoans.Add(req);

            var action = loai switch
            {
                "CapTaiKhoan" => "YeuCauCapTaiKhoan",
                "KhoaTaiKhoan" => "YeuCauKhoaTaiKhoan",
                "MoKhoaTaiKhoan" => "YeuCauMoKhoaTaiKhoan",
                _ => "YeuCauCapTaiKhoan"
            };
            await _audit.LogAsync(actorId, action, maDoiTuong);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã gửi yêu cầu đến quản trị viên", maYeuCau = req.MaYeuCau });
        }

        // E02 - Yêu cầu của tôi
        [HttpGet("requests")]
        public async Task<IActionResult> MyRequests([FromQuery] string? status)
        {
            var actorId = CurrentUserId();
            var q = _context.YeuCauTaiKhoans
                .Include(r => r.DoiTuong)
                .Where(r => r.MaNguoiYeuCau == actorId);
            if (!string.IsNullOrWhiteSpace(status) && status != "all")
                q = q.Where(r => r.TrangThai == status);

            var list = await q.OrderByDescending(r => r.NgayTao).Take(200).ToListAsync();
            var items = list.Select(r => new AccountRequestDto
            {
                MaYeuCau = r.MaYeuCau,
                LoaiYeuCau = r.LoaiYeuCau,
                MaNguoiYeuCau = r.MaNguoiYeuCau,
                MaDoiTuong = r.MaDoiTuong,
                TenDoiTuong = r.DoiTuong?.HoTen,
                MaDinhDanhDoiTuong = r.DoiTuong?.MaDinhDanh,
                EmailDoiTuong = r.DoiTuong?.Email,
                LyDoYeuCau = r.LyDoYeuCau,
                TrangThai = r.TrangThai,
                LyDoTuChoi = r.LyDoTuChoi,
                NgayTao = r.NgayTao,
                NgayXuLy = r.NgayXuLy
            }).ToList();
            return Ok(items);
        }

        // E02.4 - Xóa user chưa phát sinh dữ liệu (chỉ Nhap/ChoKichHoat)
        [HttpDelete("users/{id:int}")]
        public async Task<IActionResult> DeleteDraftUser(int id)
        {
            var actorId = CurrentUserId();
            var u = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.MaNguoiDung == id && !x.DaXoa);
            if (u == null) return NotFound();
            if (u.TrangThaiTaiKhoan != "Nhap" && u.TrangThaiTaiKhoan != "ChoKichHoat")
                return BadRequest(new { message = "Chỉ xóa được tài khoản chưa kích hoạt, chưa phát sinh dữ liệu" });

            // Không cho xóa nếu đã có dữ liệu học vụ liên quan
            var hasData = await _context.GhiDanhSinhViens.AnyAsync(g => g.MaSinhVien == id)
                || await _context.LopThucTaps.AnyAsync(l => l.MaGiangVien == id);
            if (hasData)
                return BadRequest(new { message = "Không thể xóa: tài khoản đã phát sinh dữ liệu" });

            u.DaXoa = true;
            u.NgayXoa = DateTimeOffset.UtcNow;
            await _audit.LogAsync(actorId, "XoaTaiKhoan", u.MaNguoiDung);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Đã xóa tài khoản {u.HoTen}" });
        }

        private int CurrentUserId()
        {
            var sub = User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                      ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(sub, out var id) ? id : 0;
        }
    }
}
