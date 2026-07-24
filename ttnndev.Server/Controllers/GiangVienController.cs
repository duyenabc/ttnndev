using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ttnndev.Server.Data;
using ttnndev.Server.DTOs;
using ttnndev.Server.Models;

namespace ttnndev.Server.Controllers
{
    // Slice Giảng viên: Lớp/Nhóm, Danh sách SV, Chấm điểm, Lịch hướng dẫn
    [ApiController]
    [Route("api/giangvien")]
    [Authorize(Roles = "GiangVien")]
    public class GiangVienController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GiangVienController(AppDbContext context)
        {
            _context = context;
        }

        // ============ Kỳ thực tập (cho dropdown tạo lớp) ============
        [HttpGet("cycles")]
        public async Task<IActionResult> GetCycles()
        {
            var cycles = await _context.KyThucTaps
                .OrderByDescending(k => k.NgayTao)
                .Select(k => new CycleDto
                {
                    MaKy = k.MaKy,
                    TenKy = k.TenKy,
                    NamHoc = k.NamHoc,
                    HocKy = k.HocKy,
                    TrangThai = k.TrangThai
                })
                .ToListAsync();
            return Ok(cycles);
        }

        // ============ E09.1 Lớp của tôi ============
        [HttpGet("classes")]
        public async Task<IActionResult> GetMyClasses()
        {
            var gvId = CurrentUserId();
            var classes = await _context.LopThucTaps
                .Include(l => l.KyThucTap)
                .Where(l => l.MaGiangVien == gvId)
                .OrderByDescending(l => l.NgayTao)
                .Select(l => new TeacherClassDto
                {
                    MaLop = l.MaLop,
                    MaKy = l.MaKy,
                    TenKy = l.KyThucTap.TenKy,
                    TenLop = l.TenLop,
                    SoThuTuLop = l.SoThuTuLop,
                    MaThamGia = l.MaThamGia,
                    GhiDanhMo = l.GhiDanhMo,
                    HanGhiDanh = l.HanGhiDanh,
                    LinkHopMacDinh = l.LinkHopMacDinh,
                    SoSinhVien = _context.GhiDanhSinhViens.Count(g => g.MaLop == l.MaLop),
                    NgayTao = l.NgayTao
                })
                .ToListAsync();
            return Ok(classes);
        }

        [HttpGet("classes/{maLop:int}")]
        public async Task<IActionResult> GetClass(int maLop)
        {
            var gvId = CurrentUserId();
            var l = await _context.LopThucTaps.Include(x => x.KyThucTap)
                .FirstOrDefaultAsync(x => x.MaLop == maLop);
            if (l == null) return NotFound();
            if (l.MaGiangVien != gvId) return Forbid();

            return Ok(new TeacherClassDto
            {
                MaLop = l.MaLop,
                MaKy = l.MaKy,
                TenKy = l.KyThucTap.TenKy,
                TenLop = l.TenLop,
                SoThuTuLop = l.SoThuTuLop,
                MaThamGia = l.MaThamGia,
                GhiDanhMo = l.GhiDanhMo,
                HanGhiDanh = l.HanGhiDanh,
                LinkHopMacDinh = l.LinkHopMacDinh,
                SoSinhVien = await _context.GhiDanhSinhViens.CountAsync(g => g.MaLop == l.MaLop),
                NgayTao = l.NgayTao
            });
        }

        [HttpPost("classes")]
        public async Task<IActionResult> CreateClass([FromBody] CreateClassDto dto)
        {
            var gvId = CurrentUserId();

            if (dto.SoThuTuLop <= 0)
                return BadRequest(new { message = "Vui lòng nhập số lớp" });

            var ky = await _context.KyThucTaps.FirstOrDefaultAsync(k => k.MaKy == dto.MaKy);
            if (ky == null) return BadRequest(new { message = "Kỳ thực tập không tồn tại" });

            var maHocPhan = string.IsNullOrWhiteSpace(dto.MaHocPhan) ? "TT" : dto.MaHocPhan.Trim();
            var tenLop = $"{maHocPhan}_{ky.NamHoc}_{dto.SoThuTuLop}";

            if (await _context.LopThucTaps.AnyAsync(l => l.MaKy == dto.MaKy && l.TenLop == tenLop))
                return BadRequest(new { message = "Tên lớp này đã tồn tại. Vui lòng nhập số thứ tự lớp khác" });

            var lop = new LopThucTap
            {
                MaKy = dto.MaKy,
                MaGiangVien = gvId,
                SoThuTuLop = dto.SoThuTuLop,
                TenLop = tenLop,
                MaThamGia = await GenerateJoinCodeAsync(),
                GhiDanhMo = true,
                NgayTao = DateTimeOffset.UtcNow
            };
            _context.LopThucTaps.Add(lop);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Tạo lớp thành công", maLop = lop.MaLop, tenLop = lop.TenLop });
        }

        [HttpPut("classes/{maLop:int}/enrollment")]
        public async Task<IActionResult> UpdateEnrollment(int maLop, [FromBody] UpdateEnrollmentDto dto)
        {
            var lop = await GetOwnedClassAsync(maLop);
            if (lop == null) return Forbid();

            lop.GhiDanhMo = dto.GhiDanhMo;
            lop.HanGhiDanh = dto.HanGhiDanh;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật thiết lập ghi danh thành công" });
        }

        // E09.2.1 Reset mã tham gia lớp
        [HttpPost("classes/{maLop:int}/reset-code")]
        public async Task<IActionResult> ResetJoinCode(int maLop)
        {
            var lop = await GetOwnedClassAsync(maLop);
            if (lop == null) return Forbid();

            lop.MaThamGia = await GenerateJoinCodeAsync();
            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã tạo mã tham gia mới. Mã cũ không còn hiệu lực.", maThamGia = lop.MaThamGia });
        }

        // ============ E09.3 Thêm/Import sinh viên vào lớp ============
        [HttpPost("classes/{maLop:int}/students")]
        public async Task<IActionResult> AddStudents(int maLop, [FromBody] AddStudentsDto dto)
        {
            var lop = await GetOwnedClassAsync(maLop);
            if (lop == null) return Forbid();

            var results = new List<AddStudentResultDto>();
            var codes = dto.MaSoSinhViens
                .Select(x => x?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            foreach (var code in codes)
            {
                var sv = await _context.NguoiDungs
                    .FirstOrDefaultAsync(u => u.MaDinhDanh == code && u.VaiTro == "SinhVien" && !u.DaXoa);
                if (sv == null)
                {
                    results.Add(new AddStudentResultDto { MaSoSinhVien = code!, ThanhCong = false, LyDo = "Không tìm thấy sinh viên trong hệ thống" });
                    continue;
                }

                if (await _context.GhiDanhSinhViens.AnyAsync(g => g.MaLop == maLop && g.MaSinhVien == sv.MaNguoiDung))
                {
                    results.Add(new AddStudentResultDto { MaSoSinhVien = code!, ThanhCong = false, HoTen = sv.HoTen, LyDo = "Sinh viên này đã có trong lớp" });
                    continue;
                }

                var lopKhac = await _context.GhiDanhSinhViens
                    .Include(g => g.LopThucTap)
                    .FirstOrDefaultAsync(g => g.MaSinhVien == sv.MaNguoiDung && g.LopThucTap.MaKy == lop.MaKy);
                if (lopKhac != null)
                {
                    results.Add(new AddStudentResultDto { MaSoSinhVien = code!, ThanhCong = false, HoTen = sv.HoTen, LyDo = $"Sinh viên đã được phân vào lớp {lopKhac.LopThucTap.TenLop}" });
                    continue;
                }

                _context.GhiDanhSinhViens.Add(new GhiDanhSinhVien
                {
                    MaLop = maLop,
                    MaSinhVien = sv.MaNguoiDung,
                    TrangThaiThucTap = "DangThucTap",
                    GhiDanhBang = "Import",
                    NgayGhiDanh = DateTimeOffset.UtcNow
                });
                results.Add(new AddStudentResultDto { MaSoSinhVien = code!, ThanhCong = true, HoTen = sv.HoTen });
            }

            await _context.SaveChangesAsync();
            var ok = results.Count(r => r.ThanhCong);
            return Ok(new { message = $"Thêm {ok} sinh viên thành công", ketQua = results });
        }

        // ============ E04.5 Danh sách sinh viên ============
        [HttpGet("classes/{maLop:int}/students")]
        public async Task<IActionResult> GetStudents(int maLop, [FromQuery] string? search, [FromQuery] string? trangThai, [FromQuery] int? maNhom)
        {
            var lop = await GetOwnedClassAsync(maLop);
            if (lop == null) return Forbid();

            var q = _context.GhiDanhSinhViens
                .Include(g => g.SinhVien)
                .Include(g => g.Nhom)
                .Where(g => g.MaLop == maLop);

            if (!string.IsNullOrWhiteSpace(trangThai) && trangThai != "all")
                q = q.Where(g => g.TrangThaiThucTap == trangThai);
            if (maNhom.HasValue)
                q = q.Where(g => g.MaNhom == maNhom.Value);
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(g => g.SinhVien.HoTen.ToLower().Contains(s) || g.SinhVien.MaDinhDanh.ToLower().Contains(s));
            }

            var list = await q.OrderBy(g => g.SinhVien.MaDinhDanh).ToListAsync();
            var ids = list.Select(g => g.MaSinhVien).ToList();

            var infoMap = await _context.ThongTinSinhViens
                .Where(t => ids.Contains(t.MaSinhVien))
                .ToDictionaryAsync(t => t.MaSinhVien, t => t.LopSinhHoat);

            var maGhiDanhs = list.Select(g => g.MaGhiDanh).ToList();
            var deTaiMap = await _context.DeTais
                .Where(d => maGhiDanhs.Contains(d.MaGhiDanh))
                .ToListAsync();

            var result = list.Select(g =>
            {
                var dt = deTaiMap.OrderByDescending(x => x.NgayTao).FirstOrDefault(x => x.MaGhiDanh == g.MaGhiDanh);
                return new TeacherStudentDto
                {
                    MaGhiDanh = g.MaGhiDanh,
                    MaSinhVien = g.MaSinhVien,
                    MaSoSinhVien = g.SinhVien.MaDinhDanh,
                    HoTen = g.SinhVien.HoTen,
                    Email = g.SinhVien.Email,
                    SoDienThoai = g.SinhVien.SoDienThoai,
                    LopSinhHoat = infoMap.TryGetValue(g.MaSinhVien, out var lsh) ? lsh : null,
                    MaNhom = g.MaNhom,
                    TenNhom = g.Nhom?.TenNhom,
                    ViTriThucTap = g.ViTriThucTap,
                    DonViThucTap = g.DonViThucTap,
                    TrangThaiThucTap = g.TrangThaiThucTap,
                    TinhTrangTienDo = g.TinhTrangTienDo,
                    TenDeTai = dt?.TenDeTai,
                    TrangThaiDeTai = dt?.TrangThai
                };
            }).ToList();

            return Ok(result);
        }

        // ============ E04.6 Hồ sơ chi tiết sinh viên ============
        [HttpGet("students/{maGhiDanh:int}")]
        public async Task<IActionResult> GetStudentDetail(int maGhiDanh)
        {
            var gvId = CurrentUserId();
            var g = await _context.GhiDanhSinhViens
                .Include(x => x.SinhVien)
                .Include(x => x.Nhom)
                .Include(x => x.LopThucTap)
                .FirstOrDefaultAsync(x => x.MaGhiDanh == maGhiDanh);
            if (g == null) return NotFound();
            if (g.LopThucTap.MaGiangVien != gvId) return Forbid();

            var info = await _context.ThongTinSinhViens.FirstOrDefaultAsync(t => t.MaSinhVien == g.MaSinhVien);
            var deTai = await _context.DeTais.Where(d => d.MaGhiDanh == maGhiDanh)
                .OrderByDescending(d => d.NgayTao).FirstOrDefaultAsync();

            var cols = await _context.CotDiems.Where(c => c.MaLop == g.MaLop).OrderBy(c => c.ThuTu).ToListAsync();
            var diems = await _context.DiemSinhViens.Where(d => d.MaGhiDanh == maGhiDanh).ToListAsync();
            var diemList = cols.Select(c => new
            {
                c.MaCotDiem,
                c.TenCot,
                c.DiemToiDa,
                diemSo = diems.FirstOrDefault(d => d.MaCotDiem == c.MaCotDiem)?.DiemSo
            });

            return Ok(new
            {
                maGhiDanh = g.MaGhiDanh,
                maSoSinhVien = g.SinhVien.MaDinhDanh,
                hoTen = g.SinhVien.HoTen,
                email = g.SinhVien.Email,
                soDienThoai = g.SinhVien.SoDienThoai,
                lopSinhHoat = info?.LopSinhHoat,
                tenNhom = g.Nhom?.TenNhom,
                viTriThucTap = g.ViTriThucTap,
                donViThucTap = g.DonViThucTap,
                trangThaiThucTap = g.TrangThaiThucTap,
                tinhTrangTienDo = g.TinhTrangTienDo,
                lyDoDung = g.LyDoDung,
                ngayDung = g.NgayDung,
                deTai = deTai == null ? null : new { deTai.TenDeTai, deTai.MoTa, deTai.TrangThai },
                diem = diemList
            });
        }

        // E03.5 Đánh dấu dừng thực tập
        [HttpPost("students/{maGhiDanh:int}/stop")]
        public async Task<IActionResult> StopStudent(int maGhiDanh, [FromBody] StopStudentDto dto)
        {
            var gvId = CurrentUserId();
            var g = await _context.GhiDanhSinhViens.Include(x => x.LopThucTap)
                .FirstOrDefaultAsync(x => x.MaGhiDanh == maGhiDanh);
            if (g == null) return NotFound();
            if (g.LopThucTap.MaGiangVien != gvId) return Forbid();
            if (string.IsNullOrWhiteSpace(dto.LyDo))
                return BadRequest(new { message = "Vui lòng chọn lý do dừng" });

            g.TrangThaiThucTap = "DungThucTap";
            g.LyDoDung = dto.LyDo.Trim();
            g.NgayDung = dto.NgayDung ?? DateTime.UtcNow.Date;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã đánh dấu sinh viên dừng thực tập" });
        }

        // ============ E04.7/8/9 Nhóm ============
        [HttpGet("classes/{maLop:int}/groups")]
        public async Task<IActionResult> GetGroups(int maLop)
        {
            var lop = await GetOwnedClassAsync(maLop);
            if (lop == null) return Forbid();

            var nhoms = await _context.Nhoms.Where(n => n.MaLop == maLop).OrderBy(n => n.SoThuTu).ToListAsync();
            var members = await _context.GhiDanhSinhViens.Include(g => g.SinhVien)
                .Where(g => g.MaLop == maLop && g.MaNhom != null).ToListAsync();

            var result = nhoms.Select(n => new GroupDto
            {
                MaNhom = n.MaNhom,
                TenNhom = n.TenNhom,
                SoThuTu = n.SoThuTu,
                ThanhVien = members.Where(m => m.MaNhom == n.MaNhom).Select(m => new GroupMemberDto
                {
                    MaGhiDanh = m.MaGhiDanh,
                    MaSoSinhVien = m.SinhVien.MaDinhDanh,
                    HoTen = m.SinhVien.HoTen
                }).ToList()
            }).ToList();
            return Ok(result);
        }

        [HttpPost("classes/{maLop:int}/groups")]
        public async Task<IActionResult> CreateGroup(int maLop, [FromBody] CreateGroupDto dto)
        {
            var lop = await GetOwnedClassAsync(maLop);
            if (lop == null) return Forbid();
            if (dto.MaGhiDanhs.Count < 2)
                return BadRequest(new { message = "Cần chọn ít nhất 2 sinh viên" });

            var members = await _context.GhiDanhSinhViens
                .Where(g => dto.MaGhiDanhs.Contains(g.MaGhiDanh) && g.MaLop == maLop).ToListAsync();
            if (members.Count != dto.MaGhiDanhs.Count)
                return BadRequest(new { message = "Có sinh viên không thuộc lớp này" });

            var soThuTu = (await _context.Nhoms.Where(n => n.MaLop == maLop).MaxAsync(n => (int?)n.SoThuTu) ?? 0) + 1;
            var nhom = new Nhom
            {
                MaLop = maLop,
                SoThuTu = soThuTu,
                TenNhom = $"Nhóm {soThuTu}",
                NgayTao = DateTimeOffset.UtcNow
            };
            _context.Nhoms.Add(nhom);
            await _context.SaveChangesAsync();

            foreach (var m in members) m.MaNhom = nhom.MaNhom;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Tạo {nhom.TenNhom} thành công — {members.Count} sinh viên", maNhom = nhom.MaNhom });
        }

        [HttpPut("groups/{maNhom:int}")]
        public async Task<IActionResult> UpdateGroup(int maNhom, [FromBody] UpdateGroupDto dto)
        {
            var gvId = CurrentUserId();
            var nhom = await _context.Nhoms.Include(n => n.LopThucTap).FirstOrDefaultAsync(n => n.MaNhom == maNhom);
            if (nhom == null) return NotFound();
            if (nhom.LopThucTap.MaGiangVien != gvId) return Forbid();

            if (!string.IsNullOrWhiteSpace(dto.TenNhom))
                nhom.TenNhom = dto.TenNhom.Trim();

            // Gỡ toàn bộ thành viên hiện tại rồi gán lại theo danh sách mới
            var current = await _context.GhiDanhSinhViens.Where(g => g.MaNhom == maNhom).ToListAsync();
            foreach (var c in current) c.MaNhom = null;

            var newMembers = await _context.GhiDanhSinhViens
                .Where(g => dto.MaGhiDanhs.Contains(g.MaGhiDanh) && g.MaLop == nhom.MaLop).ToListAsync();
            foreach (var m in newMembers) m.MaNhom = maNhom;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật nhóm thành công" });
        }

        [HttpDelete("groups/{maNhom:int}")]
        public async Task<IActionResult> DisbandGroup(int maNhom)
        {
            var gvId = CurrentUserId();
            var nhom = await _context.Nhoms.Include(n => n.LopThucTap).FirstOrDefaultAsync(n => n.MaNhom == maNhom);
            if (nhom == null) return NotFound();
            if (nhom.LopThucTap.MaGiangVien != gvId) return Forbid();

            var members = await _context.GhiDanhSinhViens.Where(g => g.MaNhom == maNhom).ToListAsync();
            foreach (var m in members) m.MaNhom = null;
            _context.Nhoms.Remove(nhom);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã giải tán nhóm" });
        }

        // ============ E07.3 Chấm điểm ============
        [HttpGet("classes/{maLop:int}/score-columns")]
        public async Task<IActionResult> GetScoreColumns(int maLop)
        {
            var lop = await GetOwnedClassAsync(maLop);
            if (lop == null) return Forbid();

            var cols = await _context.CotDiems.Where(c => c.MaLop == maLop).OrderBy(c => c.ThuTu)
                .Select(c => new ScoreColumnDto { MaCotDiem = c.MaCotDiem, TenCot = c.TenCot, DiemToiDa = c.DiemToiDa, ThuTu = c.ThuTu })
                .ToListAsync();
            return Ok(cols);
        }

        [HttpPost("classes/{maLop:int}/score-columns")]
        public async Task<IActionResult> CreateScoreColumn(int maLop, [FromBody] CreateScoreColumnDto dto)
        {
            var lop = await GetOwnedClassAsync(maLop);
            if (lop == null) return Forbid();
            if (string.IsNullOrWhiteSpace(dto.TenCot))
                return BadRequest(new { message = "Vui lòng nhập tên cột điểm" });
            if (await _context.CotDiems.AnyAsync(c => c.MaLop == maLop && c.TenCot == dto.TenCot.Trim()))
                return BadRequest(new { message = "Tên cột điểm đã tồn tại trong lớp" });
            if (dto.DiemToiDa <= 0 || dto.DiemToiDa > 10)
                return BadRequest(new { message = "Điểm tối đa phải trong khoảng 0–10" });

            var thuTu = (await _context.CotDiems.Where(c => c.MaLop == maLop).MaxAsync(c => (int?)c.ThuTu) ?? 0) + 1;
            var col = new CotDiem { MaLop = maLop, TenCot = dto.TenCot.Trim(), DiemToiDa = dto.DiemToiDa, ThuTu = thuTu };
            _context.CotDiems.Add(col);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã thêm cột điểm", maCotDiem = col.MaCotDiem });
        }

        [HttpDelete("score-columns/{maCotDiem:int}")]
        public async Task<IActionResult> DeleteScoreColumn(int maCotDiem)
        {
            var gvId = CurrentUserId();
            var col = await _context.CotDiems.Include(c => c.LopThucTap).FirstOrDefaultAsync(c => c.MaCotDiem == maCotDiem);
            if (col == null) return NotFound();
            if (col.LopThucTap.MaGiangVien != gvId) return Forbid();

            var diems = await _context.DiemSinhViens.Where(d => d.MaCotDiem == maCotDiem).ToListAsync();
            _context.DiemSinhViens.RemoveRange(diems);
            _context.CotDiems.Remove(col);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã xóa cột điểm" });
        }

        [HttpGet("classes/{maLop:int}/gradebook")]
        public async Task<IActionResult> GetGradebook(int maLop)
        {
            var lop = await GetOwnedClassAsync(maLop);
            if (lop == null) return Forbid();

            var cols = await _context.CotDiems.Where(c => c.MaLop == maLop).OrderBy(c => c.ThuTu).ToListAsync();
            var students = await _context.GhiDanhSinhViens.Include(g => g.SinhVien)
                .Where(g => g.MaLop == maLop).OrderBy(g => g.SinhVien.MaDinhDanh).ToListAsync();
            var ghiDanhIds = students.Select(s => s.MaGhiDanh).ToList();
            var diems = await _context.DiemSinhViens.Where(d => ghiDanhIds.Contains(d.MaGhiDanh)).ToListAsync();

            var ky = await _context.KyThucTaps.FirstOrDefaultAsync(k => k.MaKy == lop.MaKy);

            var rows = students.Select(s =>
            {
                var map = new Dictionary<int, decimal?>();
                foreach (var c in cols)
                    map[c.MaCotDiem] = diems.FirstOrDefault(d => d.MaGhiDanh == s.MaGhiDanh && d.MaCotDiem == c.MaCotDiem)?.DiemSo;
                var vals = map.Values.Where(v => v.HasValue).Select(v => v!.Value).ToList();
                return new GradebookRowDto
                {
                    MaGhiDanh = s.MaGhiDanh,
                    MaSoSinhVien = s.SinhVien.MaDinhDanh,
                    HoTen = s.SinhVien.HoTen,
                    Diem = map,
                    DiemTongKet = vals.Count > 0 ? Math.Round(vals.Average(), 2) : (decimal?)null
                };
            }).ToList();

            return Ok(new GradebookDto
            {
                CotDiem = cols.Select(c => new ScoreColumnDto { MaCotDiem = c.MaCotDiem, TenCot = c.TenCot, DiemToiDa = c.DiemToiDa, ThuTu = c.ThuTu }).ToList(),
                DongDiem = rows,
                DaKhoaSoDiem = ky?.DaKhoaSoDiem ?? false
            });
        }

        [HttpPost("classes/{maLop:int}/scores")]
        public async Task<IActionResult> SaveScores(int maLop, [FromBody] SaveScoresDto dto)
        {
            var lop = await GetOwnedClassAsync(maLop);
            if (lop == null) return Forbid();

            var ky = await _context.KyThucTaps.FirstOrDefaultAsync(k => k.MaKy == lop.MaKy);
            if (ky?.DaKhoaSoDiem == true)
                return BadRequest(new { message = "Sổ điểm đã khóa. Không thể chỉnh sửa" });

            var validGhiDanh = await _context.GhiDanhSinhViens.Where(g => g.MaLop == maLop).Select(g => g.MaGhiDanh).ToListAsync();
            var validCols = await _context.CotDiems.Where(c => c.MaLop == maLop).ToDictionaryAsync(c => c.MaCotDiem, c => c.DiemToiDa);

            foreach (var item in dto.Diem)
            {
                if (!validGhiDanh.Contains(item.MaGhiDanh) || !validCols.ContainsKey(item.MaCotDiem))
                    continue;

                var existing = await _context.DiemSinhViens
                    .FirstOrDefaultAsync(d => d.MaGhiDanh == item.MaGhiDanh && d.MaCotDiem == item.MaCotDiem);

                if (item.DiemSo == null)
                {
                    if (existing != null) _context.DiemSinhViens.Remove(existing);
                    continue;
                }

                if (item.DiemSo < 0 || item.DiemSo > validCols[item.MaCotDiem])
                    return BadRequest(new { message = $"Điểm phải trong khoảng 0–{validCols[item.MaCotDiem]}" });

                if (existing == null)
                {
                    _context.DiemSinhViens.Add(new DiemSinhVien
                    {
                        MaGhiDanh = item.MaGhiDanh,
                        MaCotDiem = item.MaCotDiem,
                        DiemSo = item.DiemSo.Value,
                        NgayCapNhat = DateTimeOffset.UtcNow
                    });
                }
                else
                {
                    existing.DiemSo = item.DiemSo.Value;
                    existing.NgayCapNhat = DateTimeOffset.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Lưu điểm thành công" });
        }

        // E07.4 Xuất bảng điểm (CSV)
        [HttpGet("classes/{maLop:int}/scores/export")]
        public async Task<IActionResult> ExportScores(int maLop)
        {
            var lop = await GetOwnedClassAsync(maLop);
            if (lop == null) return Forbid();

            var cols = await _context.CotDiems.Where(c => c.MaLop == maLop).OrderBy(c => c.ThuTu).ToListAsync();
            var students = await _context.GhiDanhSinhViens.Include(g => g.SinhVien)
                .Where(g => g.MaLop == maLop).OrderBy(g => g.SinhVien.MaDinhDanh).ToListAsync();
            var ghiDanhIds = students.Select(s => s.MaGhiDanh).ToList();
            var diems = await _context.DiemSinhViens.Where(d => ghiDanhIds.Contains(d.MaGhiDanh)).ToListAsync();

            var sb = new StringBuilder();
            sb.Append("Mã số sinh viên,Họ tên");
            foreach (var c in cols) sb.Append(',').Append(EscapeCsv(c.TenCot));
            sb.Append(",Điểm tổng kết\n");

            foreach (var s in students)
            {
                sb.Append(EscapeCsv(s.SinhVien.MaDinhDanh)).Append(',').Append(EscapeCsv(s.SinhVien.HoTen));
                var vals = new List<decimal>();
                foreach (var c in cols)
                {
                    var d = diems.FirstOrDefault(x => x.MaGhiDanh == s.MaGhiDanh && x.MaCotDiem == c.MaCotDiem)?.DiemSo;
                    sb.Append(',').Append(d?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "");
                    if (d.HasValue) vals.Add(d.Value);
                }
                sb.Append(',').Append(vals.Count > 0 ? Math.Round(vals.Average(), 2).ToString(System.Globalization.CultureInfo.InvariantCulture) : "");
                sb.Append('\n');
            }

            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
            return File(bytes, "text/csv", $"bang-diem-{lop.TenLop}.csv");
        }

        // ============ E08.1/E08.5 Lịch hướng dẫn ============
        [HttpGet("schedule")]
        public async Task<IActionResult> GetSchedule()
        {
            var gvId = CurrentUserId();
            var items = await _context.LichHens
                .Include(l => l.SinhVien)
                .Where(l => l.GiangVienId == gvId)
                .OrderByDescending(l => l.ThoiGianHop)
                .Select(l => new ScheduleDto
                {
                    Id = l.Id,
                    TieuDe = l.TieuDe,
                    NoiDung = l.NoiDung,
                    ThoiGianHop = l.ThoiGianHop,
                    LinkMeeting = l.LinkMeeting,
                    TrangThai = l.TrangThai,
                    SinhVienId = l.SinhVienId,
                    TenSinhVien = l.SinhVien.HoTen,
                    MaSoSinhVien = l.SinhVien.MaDinhDanh
                })
                .ToListAsync();
            return Ok(items);
        }

        [HttpPost("schedule")]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleDto dto)
        {
            var gvId = CurrentUserId();
            if (string.IsNullOrWhiteSpace(dto.TieuDe))
                return BadRequest(new { message = "Vui lòng nhập tiêu đề" });

            // Sinh viên phải thuộc một lớp của giảng viên này
            var thuocLop = await _context.GhiDanhSinhViens.Include(g => g.LopThucTap)
                .AnyAsync(g => g.MaSinhVien == dto.SinhVienId && g.LopThucTap.MaGiangVien == gvId);
            if (!thuocLop)
                return BadRequest(new { message = "Sinh viên không thuộc lớp của bạn" });

            var lich = new LichHen
            {
                TieuDe = dto.TieuDe.Trim(),
                NoiDung = dto.NoiDung ?? "",
                ThoiGianHop = ToUtc(dto.ThoiGianHop),
                LinkMeeting = dto.LinkMeeting ?? "",
                TrangThai = "Chờ xác nhận",
                GiangVienId = gvId,
                SinhVienId = dto.SinhVienId
            };
            _context.LichHens.Add(lich);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Tạo lịch hẹn thành công", id = lich.Id });
        }

        [HttpPut("schedule/{id:int}")]
        public async Task<IActionResult> UpdateSchedule(int id, [FromBody] UpdateScheduleDto dto)
        {
            var gvId = CurrentUserId();
            var lich = await _context.LichHens.FirstOrDefaultAsync(l => l.Id == id);
            if (lich == null) return NotFound();
            if (lich.GiangVienId != gvId) return Forbid();

            if (!string.IsNullOrWhiteSpace(dto.TieuDe)) lich.TieuDe = dto.TieuDe.Trim();
            if (dto.NoiDung != null) lich.NoiDung = dto.NoiDung;
            if (dto.ThoiGianHop.HasValue) lich.ThoiGianHop = ToUtc(dto.ThoiGianHop.Value);
            if (dto.LinkMeeting != null) lich.LinkMeeting = dto.LinkMeeting;
            if (!string.IsNullOrWhiteSpace(dto.TrangThai)) lich.TrangThai = dto.TrangThai;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật lịch hẹn thành công" });
        }

        [HttpDelete("schedule/{id:int}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var gvId = CurrentUserId();
            var lich = await _context.LichHens.FirstOrDefaultAsync(l => l.Id == id);
            if (lich == null) return NotFound();
            if (lich.GiangVienId != gvId) return Forbid();
            _context.LichHens.Remove(lich);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã xóa lịch hẹn" });
        }

        // ============ Helpers ============
        private async Task<LopThucTap?> GetOwnedClassAsync(int maLop)
        {
            var gvId = CurrentUserId();
            var lop = await _context.LopThucTaps.FirstOrDefaultAsync(l => l.MaLop == maLop);
            if (lop == null || lop.MaGiangVien != gvId) return null;
            return lop;
        }

        private async Task<string> GenerateJoinCodeAsync()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var rnd = Random.Shared;
            string code;
            do
            {
                code = new string(Enumerable.Range(0, 6).Select(_ => chars[rnd.Next(chars.Length)]).ToArray());
            } while (await _context.LopThucTaps.AnyAsync(l => l.MaThamGia == code));
            return code;
        }

        private static DateTime ToUtc(DateTime value) =>
            value.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(value, DateTimeKind.Utc)
                : value.ToUniversalTime();

        private static string EscapeCsv(string? value)
        {
            value ??= "";
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            return value;
        }

        private int CurrentUserId()
        {
            var sub = User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                      ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(sub, out var id) ? id : 0;
        }
    }
}
