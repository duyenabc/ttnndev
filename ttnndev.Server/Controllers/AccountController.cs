using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
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
    // Quản lý tài khoản dành cho Admin và Giáo vụ có quyền (E00, E01)
    [ApiController]
    [Route("api/account")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IAuditService _audit;
        private readonly PasswordHasher<NguoiDung> _hasher = new();

        private static readonly string[] ManageableRoles = { "GiangVien", "GiaoVu", "SinhVien" };

        public AccountController(AppDbContext context, ITokenService tokenService, IAuditService audit)
        {
            _context = context;
            _tokenService = tokenService;
            _audit = audit;
        }

        // E00.7 - Thẻ tổng quan số lượng tài khoản
        [HttpGet("summary")]
        public async Task<IActionResult> Summary([FromQuery] string? role)
        {
            if (!await CanManageAsync()) return Forbid();
            var q = _context.NguoiDungs.Where(u => !u.DaXoa && u.VaiTro != "Admin");
            if (!string.IsNullOrWhiteSpace(role)) q = q.Where(u => u.VaiTro == role);

            var dto = new AccountSummaryDto
            {
                TongSo = await q.CountAsync(),
                ChoKichHoat = await q.CountAsync(u => u.TrangThaiTaiKhoan == "ChoKichHoat"),
                DangHoatDong = await q.CountAsync(u => u.TrangThaiTaiKhoan == "DangHoatDong"),
                BiKhoa = await q.CountAsync(u => u.TrangThaiTaiKhoan == "BiKhoa")
            };
            return Ok(dto);
        }

        // E00.7 - Danh sách tài khoản theo tab (role) + lọc + tìm kiếm + phân trang
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers(
            [FromQuery] string? role,
            [FromQuery] string? status,
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 25)
        {
            if (!await CanManageAsync()) return Forbid();
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 200) pageSize = 25;

            var q = _context.NguoiDungs.Where(u => !u.DaXoa && u.VaiTro != "Admin");
            if (!string.IsNullOrWhiteSpace(role)) q = q.Where(u => u.VaiTro == role);
            if (!string.IsNullOrWhiteSpace(status)) q = q.Where(u => u.TrangThaiTaiKhoan == status);
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(u => u.MaDinhDanh.ToLower().Contains(s)
                              || u.HoTen.ToLower().Contains(s)
                              || u.Email.ToLower().Contains(s));
            }

            var total = await q.CountAsync();
            var users = await q.OrderByDescending(u => u.NgayTao)
                .Skip((page - 1) * pageSize).Take(pageSize)
                .ToListAsync();

            var giaoVuIds = users.Where(u => u.VaiTro == "GiaoVu").Select(u => u.MaNguoiDung).ToList();
            var quyen = await _context.QuyenGiaoVus
                .Where(q2 => giaoVuIds.Contains(q2.MaGiaoVu))
                .ToDictionaryAsync(q2 => q2.MaGiaoVu, q2 => q2.QuyenQuanLyNguoiDung);

            var items = users.Select(u => new AccountListItemDto
            {
                MaNguoiDung = u.MaNguoiDung,
                MaDinhDanh = u.MaDinhDanh,
                HoTen = u.HoTen,
                Email = u.Email,
                SoDienThoai = u.SoDienThoai,
                VaiTro = u.VaiTro,
                TrangThaiTaiKhoan = u.TrangThaiTaiKhoan,
                QuyenQuanLyNguoiDung = quyen.TryGetValue(u.MaNguoiDung, out var qv) && qv,
                NgayTao = u.NgayTao
            }).ToList();

            return Ok(new PagedResult<AccountListItemDto>
            {
                Items = items, Total = total, Page = page, PageSize = pageSize
            });
        }

        // E00.7 - Chi tiết tài khoản
        [HttpGet("users/{id:int}")]
        public async Task<IActionResult> GetUser(int id)
        {
            if (!await CanManageAsync()) return Forbid();
            var u = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.MaNguoiDung == id && !x.DaXoa);
            if (u == null) return NotFound();

            var dto = new AccountDetailDto
            {
                MaNguoiDung = u.MaNguoiDung,
                MaDinhDanh = u.MaDinhDanh,
                HoTen = u.HoTen,
                Email = u.Email,
                SoDienThoai = u.SoDienThoai,
                VaiTro = u.VaiTro,
                TrangThaiTaiKhoan = u.TrangThaiTaiKhoan,
                NgayTao = u.NgayTao,
                NgayCapNhat = u.NgayCapNhat
            };

            if (u.VaiTro == "SinhVien")
            {
                var tt = await _context.ThongTinSinhViens.Include(t => t.Khoa)
                    .FirstOrDefaultAsync(t => t.MaSinhVien == id);
                dto.TenKhoa = tt?.Khoa?.TenKhoa;
                dto.LopSinhHoat = tt?.LopSinhHoat;
            }
            else if (u.VaiTro == "GiangVien" || u.VaiTro == "GiaoVu")
            {
                var tt = await _context.ThongTinCanBos.Include(t => t.Khoa).Include(t => t.BoMon)
                    .FirstOrDefaultAsync(t => t.MaCanBo == id);
                dto.TenKhoa = tt?.Khoa?.TenKhoa;
                dto.TenBoMon = tt?.BoMon?.TenBoMon;
            }
            if (u.VaiTro == "GiaoVu")
            {
                dto.QuyenQuanLyNguoiDung = await _context.QuyenGiaoVus
                    .AnyAsync(q => q.MaGiaoVu == id && q.QuyenQuanLyNguoiDung);
            }
            return Ok(dto);
        }

        // E00.2/E00.5 - Tạo 1 tài khoản (Nhap hoặc cấp ngay → ChoKichHoat)
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto model)
        {
            var actorId = CurrentUserId();
            if (!await CanManageAsync()) return Forbid();

            if (!ManageableRoles.Contains(model.VaiTro))
                return BadRequest(new { message = "Vai trò không hợp lệ" });
            if (string.IsNullOrWhiteSpace(model.MaDinhDanh) || string.IsNullOrWhiteSpace(model.HoTen))
                return BadRequest(new { message = "Thiếu mã định danh hoặc họ tên" });
            if (string.IsNullOrWhiteSpace(model.Email) || !model.Email.Contains('@'))
                return BadRequest(new { message = "Email không hợp lệ" });
            if (await _context.NguoiDungs.AnyAsync(u => u.MaDinhDanh == model.MaDinhDanh && !u.DaXoa))
                return BadRequest(new { message = "Mã định danh đã tồn tại" });
            if (await _context.NguoiDungs.AnyAsync(u => u.Email == model.Email && !u.DaXoa))
                return BadRequest(new { message = "Email đã tồn tại" });

            var user = new NguoiDung
            {
                MaDinhDanh = model.MaDinhDanh.Trim(),
                HoTen = model.HoTen.Trim(),
                Email = model.Email.Trim(),
                SoDienThoai = model.SoDienThoai,
                VaiTro = model.VaiTro,
                TrangThaiTaiKhoan = model.CapTaiKhoanNgay ? "ChoKichHoat" : "Nhap",
                NgayTao = DateTimeOffset.UtcNow,
                NgayCapNhat = DateTimeOffset.UtcNow
            };
            _context.NguoiDungs.Add(user);
            await _context.SaveChangesAsync();

            if (model.VaiTro == "GiaoVu")
            {
                _context.QuyenGiaoVus.Add(new QuyenGiaoVu
                {
                    MaGiaoVu = user.MaNguoiDung,
                    QuyenQuanLyNguoiDung = model.QuyenQuanLyNguoiDung,
                    NgayCapGanNhat = DateTimeOffset.UtcNow,
                    CapBoiGanNhat = actorId
                });
                await _context.SaveChangesAsync();
            }

            string? link = null;
            if (model.CapTaiKhoanNgay)
                link = await CreateActivationLinkAsync(user);

            await _audit.LogAsync(actorId, "ThemNguoiDung", user.MaNguoiDung,
                giaTriMoi: JsonSerializer.Serialize(new { user.MaDinhDanh, user.VaiTro, user.TrangThaiTaiKhoan }));
            if (model.CapTaiKhoanNgay)
                await _audit.LogAsync(actorId, "CapTaiKhoan", user.MaNguoiDung);

            return Ok(new { message = "Tạo tài khoản thành công", maNguoiDung = user.MaNguoiDung, devActivationLink = link });
        }

        // E00.5 - Cấp tài khoản (Nhap → ChoKichHoat + gửi link kích hoạt)
        [HttpPost("users/{id:int}/grant")]
        public async Task<IActionResult> Grant(int id)
        {
            var actorId = CurrentUserId();
            if (!await CanManageAsync()) return Forbid();
            var u = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.MaNguoiDung == id && !x.DaXoa);
            if (u == null) return NotFound();
            if (u.TrangThaiTaiKhoan != "Nhap")
                return BadRequest(new { message = "Chỉ có thể cấp tài khoản ở trạng thái Nháp" });

            u.TrangThaiTaiKhoan = "ChoKichHoat";
            u.NgayCapNhat = DateTimeOffset.UtcNow;
            var link = await CreateActivationLinkAsync(u);
            await _audit.LogAsync(actorId, "CapTaiKhoan", u.MaNguoiDung);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Đã cấp tài khoản cho {u.HoTen}", devActivationLink = link });
        }

        // E00.6 - Gửi lại link kích hoạt
        [HttpPost("users/{id:int}/resend-activation")]
        public async Task<IActionResult> ResendActivation(int id)
        {
            var actorId = CurrentUserId();
            if (!await CanManageAsync()) return Forbid();
            var u = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.MaNguoiDung == id && !x.DaXoa);
            if (u == null) return NotFound();
            if (u.TrangThaiTaiKhoan != "ChoKichHoat")
                return BadRequest(new { message = "Chỉ gửi lại link cho tài khoản Chờ kích hoạt" });

            var link = await CreateActivationLinkAsync(u);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Đã gửi lại link kích hoạt cho {u.HoTen}", devActivationLink = link });
        }

        // E00.3 - Xóa tài khoản (chỉ Nhap / ChoKichHoat)
        [HttpDelete("users/{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var actorId = CurrentUserId();
            if (!await CanManageAsync()) return Forbid();
            var u = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.MaNguoiDung == id && !x.DaXoa);
            if (u == null) return NotFound();
            if (u.TrangThaiTaiKhoan != "Nhap" && u.TrangThaiTaiKhoan != "ChoKichHoat")
                return BadRequest(new { message = "Chỉ xóa được tài khoản ở trạng thái Nháp hoặc Chờ kích hoạt" });

            u.DaXoa = true;
            u.NgayXoa = DateTimeOffset.UtcNow;
            await _audit.LogAsync(actorId, "XoaTaiKhoan", u.MaNguoiDung);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Đã xóa tài khoản {u.HoTen}" });
        }

        // E00.3 - Xóa nhiều tài khoản
        [HttpPost("users/delete-bulk")]
        public async Task<IActionResult> DeleteBulk([FromBody] BulkIdsDto model)
        {
            var actorId = CurrentUserId();
            if (!await CanManageAsync()) return Forbid();
            var users = await _context.NguoiDungs
                .Where(u => model.Ids.Contains(u.MaNguoiDung) && !u.DaXoa
                    && (u.TrangThaiTaiKhoan == "Nhap" || u.TrangThaiTaiKhoan == "ChoKichHoat"))
                .ToListAsync();
            foreach (var u in users)
            {
                u.DaXoa = true;
                u.NgayXoa = DateTimeOffset.UtcNow;
                await _audit.LogAsync(actorId, "XoaTaiKhoan", u.MaNguoiDung);
            }
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Đã xóa {users.Count} tài khoản", deleted = users.Count });
        }

        // E00.8 - Khóa tài khoản (yêu cầu lý do)
        [HttpPost("users/{id:int}/lock")]
        public async Task<IActionResult> Lock(int id, [FromBody] LockDto model)
        {
            var actorId = CurrentUserId();
            if (!await CanManageAsync()) return Forbid();
            if (string.IsNullOrWhiteSpace(model.LyDo))
                return BadRequest(new { message = "Vui lòng nhập lý do khóa tài khoản" });
            if (model.LyDo.Length > 200)
                return BadRequest(new { message = "Lý do tối đa 200 ký tự" });

            var u = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.MaNguoiDung == id && !x.DaXoa);
            if (u == null) return NotFound();
            if (u.TrangThaiTaiKhoan != "DangHoatDong" && u.TrangThaiTaiKhoan != "ChoKichHoat")
                return BadRequest(new { message = "Chỉ khóa được tài khoản Đang hoạt động hoặc Chờ kích hoạt" });

            var old = u.TrangThaiTaiKhoan;
            u.TrangThaiTaiKhoan = "BiKhoa";
            u.TokenValidFrom = DateTimeOffset.UtcNow; // E15.7.1 hủy phiên ngay lập tức
            u.NgayCapNhat = DateTimeOffset.UtcNow;
            await RevokeAllRefreshTokensAsync(u.MaNguoiDung);
            await _audit.LogAsync(actorId, "KhoaTaiKhoan", u.MaNguoiDung,
                giaTriCu: old, giaTriMoi: JsonSerializer.Serialize(new { LyDo = model.LyDo }));
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Đã khóa tài khoản {u.HoTen}" });
        }

        // E00.8 - Mở khóa
        [HttpPost("users/{id:int}/unlock")]
        public async Task<IActionResult> Unlock(int id)
        {
            var actorId = CurrentUserId();
            if (!await CanManageAsync()) return Forbid();
            var u = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.MaNguoiDung == id && !x.DaXoa);
            if (u == null) return NotFound();
            if (u.TrangThaiTaiKhoan != "BiKhoa")
                return BadRequest(new { message = "Chỉ mở khóa được tài khoản đang bị khóa" });

            // Nếu chưa từng đặt mật khẩu → quay lại ChoKichHoat, ngược lại DangHoatDong
            u.TrangThaiTaiKhoan = string.IsNullOrEmpty(u.MatKhauHash) ? "ChoKichHoat" : "DangHoatDong";
            u.SoLanDangNhapSai = 0;
            u.KhoaDangNhapDenLuc = null;
            u.NgayCapNhat = DateTimeOffset.UtcNow;
            await _audit.LogAsync(actorId, "MoKhoaTaiKhoan", u.MaNguoiDung);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Đã mở khóa tài khoản {u.HoTen}" });
        }

        // E00.9 - Cấp mật khẩu tạm (buộc đổi lần đăng nhập kế tiếp)
        [HttpPost("users/{id:int}/temp-password")]
        public async Task<IActionResult> TempPassword(int id)
        {
            var actorId = CurrentUserId();
            if (!await CanManageAsync()) return Forbid();
            var u = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.MaNguoiDung == id && !x.DaXoa);
            if (u == null) return NotFound();
            if (u.TrangThaiTaiKhoan != "DangHoatDong")
                return BadRequest(new { message = "Chỉ cấp mật khẩu tạm cho tài khoản Đang hoạt động" });

            var temp = PasswordPolicy.GenerateTemporary();
            u.MatKhauHash = _hasher.HashPassword(u, temp);
            u.BuocDoiMatKhau = true;
            u.TokenValidFrom = DateTimeOffset.UtcNow;
            u.SoLanDangNhapSai = 0;
            u.KhoaDangNhapDenLuc = null;
            u.NgayCapNhat = DateTimeOffset.UtcNow;
            await RevokeAllRefreshTokensAsync(u.MaNguoiDung);
            await _audit.LogAsync(actorId, "CapMatKhauTam", u.MaNguoiDung);
            await _context.SaveChangesAsync();
            return Ok(new TempPasswordResultDto
            {
                MatKhauTam = temp,
                Message = $"Đã cấp mật khẩu tạm cho {u.HoTen}"
            });
        }

        // E01.1 - Bật/tắt quyền Quản lý người dùng cho Giáo vụ (chỉ Admin)
        [Authorize(Roles = "Admin")]
        [HttpPut("users/{id:int}/permission")]
        public async Task<IActionResult> TogglePermission(int id, [FromBody] PermissionToggleDto model)
        {
            var actorId = CurrentUserId();
            var u = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.MaNguoiDung == id && !x.DaXoa);
            if (u == null) return NotFound();
            if (u.VaiTro != "GiaoVu")
                return BadRequest(new { message = "Chỉ áp dụng quyền cho tài khoản Giáo vụ" });

            var quyen = await _context.QuyenGiaoVus.FirstOrDefaultAsync(q => q.MaGiaoVu == id);
            bool old = quyen?.QuyenQuanLyNguoiDung ?? false;
            if (quyen == null)
            {
                quyen = new QuyenGiaoVu { MaGiaoVu = id, CapBoiGanNhat = actorId };
                _context.QuyenGiaoVus.Add(quyen);
            }
            quyen.QuyenQuanLyNguoiDung = model.QuyenQuanLyNguoiDung;
            quyen.NgayCapGanNhat = DateTimeOffset.UtcNow;
            quyen.CapBoiGanNhat = actorId;

            await _audit.LogAsync(actorId, model.QuyenQuanLyNguoiDung ? "CapQuyenQL" : "ThuHoiQuyenQL", id,
                giaTriCu: old.ToString(), giaTriMoi: model.QuyenQuanLyNguoiDung.ToString());
            await _context.SaveChangesAsync();
            return Ok(new
            {
                message = model.QuyenQuanLyNguoiDung
                    ? $"Đã cấp quyền Quản lý người dùng cho {u.HoTen}"
                    : $"Đã thu hồi quyền Quản lý người dùng của {u.HoTen}"
            });
        }

        // E01.2 - Lịch sử thao tác của một tài khoản
        [HttpGet("users/{id:int}/audit")]
        public async Task<IActionResult> Audit(int id,
            [FromQuery] string? hanhDong, [FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to)
        {
            if (!await CanManageAsync()) return Forbid();
            var q = _context.AuditLogs.Where(l => l.MaDoiTuong == id);
            if (!string.IsNullOrWhiteSpace(hanhDong)) q = q.Where(l => l.HanhDong == hanhDong);
            if (from.HasValue) q = q.Where(l => l.ThoiDiem >= from.Value);
            if (to.HasValue) q = q.Where(l => l.ThoiDiem <= to.Value);

            var logs = await q.OrderByDescending(l => l.ThoiDiem).Take(200).ToListAsync();
            var actorIds = logs.Select(l => l.MaNguoiThucHien).Distinct().ToList();
            var actors = await _context.NguoiDungs.Where(u => actorIds.Contains(u.MaNguoiDung))
                .ToDictionaryAsync(u => u.MaNguoiDung, u => u.HoTen);

            var items = logs.Select(l => new AuditLogDto
            {
                MaLog = l.MaLog,
                HanhDong = l.HanhDong,
                MaNguoiThucHien = l.MaNguoiThucHien,
                TenNguoiThucHien = actors.TryGetValue(l.MaNguoiThucHien, out var n) ? n : null,
                MaDoiTuong = l.MaDoiTuong,
                ThoiDiem = l.ThoiDiem
            }).ToList();
            return Ok(items);
        }

        // E00.4 - Danh sách yêu cầu chờ xử lý
        [HttpGet("requests")]
        public async Task<IActionResult> Requests([FromQuery] string? status, [FromQuery] string? loai, [FromQuery] string? search)
        {
            if (!await CanManageAsync()) return Forbid();
            var q = _context.YeuCauTaiKhoans
                .Include(r => r.NguoiYeuCau)
                .Include(r => r.DoiTuong)
                .AsQueryable();
            status ??= "ChoXuLy";
            if (status != "all") q = q.Where(r => r.TrangThai == status);
            if (!string.IsNullOrWhiteSpace(loai)) q = q.Where(r => r.LoaiYeuCau == loai);
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(r => r.DoiTuong != null &&
                    (r.DoiTuong.MaDinhDanh.ToLower().Contains(s)
                     || r.DoiTuong.HoTen.ToLower().Contains(s)
                     || r.DoiTuong.Email.ToLower().Contains(s)));
            }

            var list = await q.OrderByDescending(r => r.NgayTao).Take(200).ToListAsync();
            var items = list.Select(ToRequestDto).ToList();
            return Ok(items);
        }

        // E00.5 - Duyệt yêu cầu
        [HttpPost("requests/{id:int}/approve")]
        public async Task<IActionResult> ApproveRequest(int id)
        {
            var actorId = CurrentUserId();
            if (!await CanManageAsync()) return Forbid();
            var r = await _context.YeuCauTaiKhoans.Include(x => x.DoiTuong)
                .FirstOrDefaultAsync(x => x.MaYeuCau == id);
            if (r == null) return NotFound();
            if (r.TrangThai != "ChoXuLy")
                return BadRequest(new { message = "Yêu cầu đã được xử lý" });

            string? link = null;
            var target = r.DoiTuong;
            switch (r.LoaiYeuCau)
            {
                case "CapTaiKhoan":
                    if (target != null && target.TrangThaiTaiKhoan == "Nhap")
                    {
                        target.TrangThaiTaiKhoan = "ChoKichHoat";
                        target.NgayCapNhat = DateTimeOffset.UtcNow;
                        link = await CreateActivationLinkAsync(target);
                        await _audit.LogAsync(actorId, "CapTaiKhoan", target.MaNguoiDung);
                    }
                    break;
                case "KhoaTaiKhoan":
                    if (target != null && (target.TrangThaiTaiKhoan == "DangHoatDong" || target.TrangThaiTaiKhoan == "ChoKichHoat"))
                    {
                        target.TrangThaiTaiKhoan = "BiKhoa";
                        target.TokenValidFrom = DateTimeOffset.UtcNow;
                        target.NgayCapNhat = DateTimeOffset.UtcNow;
                        await RevokeAllRefreshTokensAsync(target.MaNguoiDung);
                        await _audit.LogAsync(actorId, "KhoaTaiKhoan", target.MaNguoiDung);
                    }
                    break;
                case "MoKhoaTaiKhoan":
                    if (target != null && target.TrangThaiTaiKhoan == "BiKhoa")
                    {
                        target.TrangThaiTaiKhoan = string.IsNullOrEmpty(target.MatKhauHash) ? "ChoKichHoat" : "DangHoatDong";
                        target.NgayCapNhat = DateTimeOffset.UtcNow;
                        await _audit.LogAsync(actorId, "MoKhoaTaiKhoan", target.MaNguoiDung);
                    }
                    break;
            }

            r.TrangThai = "DaDuyet";
            r.MaNguoiXuLy = actorId;
            r.NgayXuLy = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã duyệt yêu cầu", devActivationLink = link });
        }

        // E00.5 - Từ chối yêu cầu (bắt buộc lý do)
        [HttpPost("requests/{id:int}/reject")]
        public async Task<IActionResult> RejectRequest(int id, [FromBody] RejectDto model)
        {
            var actorId = CurrentUserId();
            if (!await CanManageAsync()) return Forbid();
            if (string.IsNullOrWhiteSpace(model.LyDo))
                return BadRequest(new { message = "Vui lòng nhập lý do từ chối" });
            if (model.LyDo.Length > 200)
                return BadRequest(new { message = "Lý do tối đa 200 ký tự" });

            var r = await _context.YeuCauTaiKhoans.FirstOrDefaultAsync(x => x.MaYeuCau == id);
            if (r == null) return NotFound();
            if (r.TrangThai != "ChoXuLy")
                return BadRequest(new { message = "Yêu cầu đã được xử lý" });

            r.TrangThai = "TuChoi";
            r.LyDoTuChoi = model.LyDo;
            r.MaNguoiXuLy = actorId;
            r.NgayXuLy = DateTimeOffset.UtcNow;
            var action = r.LoaiYeuCau switch
            {
                "CapTaiKhoan" => "TuChoiCapTaiKhoan",
                "KhoaTaiKhoan" => "TuChoiKhoaTaiKhoan",
                "MoKhoaTaiKhoan" => "TuChoiMoKhoaTaiKhoan",
                _ => "TuChoiCapTaiKhoan"
            };
            await _audit.LogAsync(actorId, action, r.MaDoiTuong,
                giaTriMoi: JsonSerializer.Serialize(new { LyDo = model.LyDo }));
            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã từ chối yêu cầu" });
        }

        // ---------- Helpers ----------

        private AccountRequestDto ToRequestDto(YeuCauTaiKhoan r) => new()
        {
            MaYeuCau = r.MaYeuCau,
            LoaiYeuCau = r.LoaiYeuCau,
            MaNguoiYeuCau = r.MaNguoiYeuCau,
            TenNguoiYeuCau = r.NguoiYeuCau?.HoTen,
            MaDoiTuong = r.MaDoiTuong,
            TenDoiTuong = r.DoiTuong?.HoTen,
            MaDinhDanhDoiTuong = r.DoiTuong?.MaDinhDanh,
            EmailDoiTuong = r.DoiTuong?.Email,
            LyDoYeuCau = r.LyDoYeuCau,
            TrangThai = r.TrangThai,
            LyDoTuChoi = r.LyDoTuChoi,
            NgayTao = r.NgayTao,
            NgayXuLy = r.NgayXuLy
        };

        private async Task<string> CreateActivationLinkAsync(NguoiDung user)
        {
            var old = await _context.YeuCauXacThucs
                .Where(r => r.MaNguoiDung == user.MaNguoiDung && r.LoaiYeuCau == "KichHoat" && !r.DaSuDung)
                .ToListAsync();
            foreach (var o in old) o.DaSuDung = true;

            var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(48);
            var plain = Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
            _context.YeuCauXacThucs.Add(new YeuCauXacThuc
            {
                MaNguoiDung = user.MaNguoiDung,
                LoaiYeuCau = "KichHoat",
                TokenHash = _tokenService.HashToken(plain),
                ThoiDiemTao = DateTimeOffset.UtcNow,
                ThoiDiemHetHan = DateTimeOffset.UtcNow.AddHours(72),
                DaSuDung = false
            });
            return $"/activate?token={plain}";
        }

        private async Task RevokeAllRefreshTokensAsync(int userId)
        {
            var tokens = await _context.RefreshTokens
                .Where(t => t.MaNguoiDung == userId && !t.DaThuHoi).ToListAsync();
            foreach (var t in tokens) t.DaThuHoi = true;
        }

        private int CurrentUserId()
        {
            var sub = User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                      ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(sub, out var id) ? id : 0;
        }

        // Admin, hoặc Giáo vụ có QuyenQuanLyNguoiDung
        private async Task<bool> CanManageAsync()
        {
            if (User.IsInRole("Admin")) return true;
            if (User.IsInRole("GiaoVu"))
            {
                var id = CurrentUserId();
                return await _context.QuyenGiaoVus.AnyAsync(q => q.MaGiaoVu == id && q.QuyenQuanLyNguoiDung);
            }
            return false;
        }
    }
}
