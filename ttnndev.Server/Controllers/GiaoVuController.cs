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
            if (loai != "CapTaiKhoan" && loai != "KhoaTaiKhoan" && loai != "MoKhoaTaiKhoan"
                && loai != "CapQuyenGiaoVu" && loai != "ThuHoiQuyenGiaoVu")
                return BadRequest(new { message = "Loại yêu cầu không hợp lệ" });

            int? maDoiTuong = model.MaDoiTuong;

            if (loai == "CapTaiKhoan")
            {
                if (!await CanManageUsersAsync(actorId))
                    return Forbid();

                if (maDoiTuong.HasValue)
                {
                    var existing = await _context.NguoiDungs
                        .FirstOrDefaultAsync(u => u.MaNguoiDung == maDoiTuong.Value && !u.DaXoa);
                    if (existing == null) return NotFound();
                    if (existing.TrangThaiTaiKhoan != "Nhap")
                        return BadRequest(new { message = "Chỉ yêu cầu cấp tài khoản ở trạng thái Nháp" });
                }
                else
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
            }
            else
            {
                // Khóa/mở khóa cần lý do
                if (string.IsNullOrWhiteSpace(model.LyDo))
                    return BadRequest(new { message = "Vui lòng nhập lý do" });
                if (model.LyDo.Length > 200)
                    return BadRequest(new { message = "Lý do tối đa 200 ký tự" });
                if (maDoiTuong == null)
                    return BadRequest(new { message = "Thiếu tài khoản mục tiêu" });

                var target = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.MaNguoiDung == maDoiTuong.Value && !u.DaXoa);
                if (target == null) return NotFound();
                if (loai == "KhoaTaiKhoan" && target.TrangThaiTaiKhoan != "DangHoatDong" && target.TrangThaiTaiKhoan != "ChoKichHoat")
                    return BadRequest(new { message = "Chỉ yêu cầu khóa tài khoản Đang hoạt động hoặc Chờ kích hoạt" });
                if (loai == "MoKhoaTaiKhoan" && target.TrangThaiTaiKhoan != "BiKhoa")
                    return BadRequest(new { message = "Chỉ yêu cầu mở khóa tài khoản đang bị khóa" });
                if ((loai == "CapQuyenGiaoVu" || loai == "ThuHoiQuyenGiaoVu") && target.VaiTro != "GiaoVu")
                    return BadRequest(new { message = "Chỉ áp dụng quyền cho tài khoản Giáo vụ" });
                if ((loai == "CapQuyenGiaoVu" || loai == "ThuHoiQuyenGiaoVu") && target.TrangThaiTaiKhoan != "DangHoatDong")
                    return BadRequest(new { message = "Không thể thay đổi quyền khi tài khoản không hoạt động" });
            }

            var hasPending = await _context.YeuCauTaiKhoans.AnyAsync(r =>
                r.MaDoiTuong == maDoiTuong && r.LoaiYeuCau == loai && r.TrangThai == "ChoXuLy");
            if (hasPending)
                return BadRequest(new { message = "Yêu cầu đang chờ xử lý đã tồn tại" });

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
                "CapQuyenGiaoVu" => "YeuCauCapQuyenQL",
                "ThuHoiQuyenGiaoVu" => "YeuCauThuHoiQuyenQL",
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
            if (!await CanManageUsersAsync(actorId)) return Forbid();
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

        private async Task<bool> CanManageUsersAsync(int userId)
        {
            return await _context.QuyenGiaoVus
                .AnyAsync(q => q.MaGiaoVu == userId && q.QuyenQuanLyNguoiDung);
        }

        private int CurrentUserId()
        {
            var sub = User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                      ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(sub, out var id) ? id : 0;
        }
    }
}
