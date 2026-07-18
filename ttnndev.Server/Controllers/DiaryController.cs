using Microsoft.AspNetCore.Mvc;
using ttnndev.Server.Data;
using ttnndev.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace ttnndev.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiaryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DiaryController(AppDbContext context)
        {
            _context = context;
        }

        // GET api/Diary/my-diaries?maDinhDanh=SV001
        // Lấy danh sách nhật ký của sinh viên đang đăng nhập
        [HttpGet("my-diaries")]
        public async Task<IActionResult> GetMyDiaries([FromQuery] string maDinhDanh)
        {
            if (string.IsNullOrEmpty(maDinhDanh))
                return BadRequest("Thiếu maDinhDanh.");

            // Tìm sinh viên theo mã định danh
            var sinhVien = await _context.NguoiDungs
                .FirstOrDefaultAsync(u => u.MaDinhDanh == maDinhDanh && !u.DaXoa);

            if (sinhVien == null)
                return NotFound("Không tìm thấy sinh viên.");

            // Lấy danh sách nhật ký thông qua GhiDanh
            var diaries = await _context.NhatKys
                .Include(n => n.GhiDanh)
                .Where(n => n.GhiDanh.MaSinhVien == sinhVien.MaNguoiDung)
                .OrderByDescending(n => n.NgayTao)
                .Select(n => new
                {
                    n.MaNhatKy,
                    tuanThucTap = n.Tuan,
                    ngayTao = n.NgayTao,
                    noiDungCongViec = n.CongViecDaLam,
                    ketQuaDatDuoc = n.KetQua,
                    khoKhanVongMac = n.KhoKhan,
                    n.KeHoachTuanToi,
                    trangThaiDuyet = n.TrangThaiDuyet,
                    nhanXetGiangVien = n.NhanXetGiangVien
                })
                .ToListAsync();

            return Ok(diaries);
        }

        // POST api/Diary
        // Sinh viên nộp nhật ký mới
        [HttpPost]
        public async Task<IActionResult> SubmitDiary([FromBody] DiarySubmitDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Tìm sinh viên
            var sinhVien = await _context.NguoiDungs
                .FirstOrDefaultAsync(u => u.MaDinhDanh == dto.MaDinhDanh && !u.DaXoa);

            if (sinhVien == null)
                return NotFound("Không tìm thấy sinh viên.");

            // Tìm GhiDanh của sinh viên
            var ghiDanh = await _context.GhiDanhSinhViens
                .Include(g => g.LopThucTap)
                .ThenInclude(l => l.KyThucTap)
                .FirstOrDefaultAsync(g => g.MaSinhVien == sinhVien.MaNguoiDung);

            if (ghiDanh == null)
                return NotFound("Sinh viên chưa được ghi danh vào lớp thực tập nào.");

            var kyThucTap = ghiDanh.LopThucTap.KyThucTap;

            // Validate ngày thực hiện
            var ngayThucHien = dto.NgayThucHien ?? DateTime.UtcNow;
            if (ngayThucHien > DateTime.UtcNow)
                return BadRequest("Không thể nộp nhật ký cho ngày trong tương lai.");

            // Map DTO -> NhatKy model
            var nhatKy = new NhatKy
            {
                MaGhiDanh = ghiDanh.MaGhiDanh,
                Tuan = dto.TuanThucTap,
                NgayThucHien = ngayThucHien,
                CongViecDaLam = dto.NoiDungCongViec,
                KetQua = dto.KetQuaDatDuoc,
                KhoKhan = dto.KhoKhanVongMac,
                KeHoachTuanToi = dto.KeHoachTuanToi,
                TrangThaiDuyet = "ChoDuyet"
            };

            _context.NhatKys.Add(nhatKy);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Lưu nhật ký thành công!", maNhatKy = nhatKy.MaNhatKy });
        }

        // GET api/Diary/class/{maLop}/weekly/{weekNumber}
        // Giảng viên xem nhật ký cả lớp (Weekly Digest)
        [HttpGet("class/{maLop}/weekly/{weekNumber}")]
        public async Task<IActionResult> GetWeeklyDiary(int maLop, int weekNumber)
        {
            var diaries = await _context.NhatKys
                .Include(n => n.GhiDanh)
                .ThenInclude(g => g.SinhVien)
                .Where(n => n.GhiDanh.MaLop == maLop && n.Tuan == weekNumber)
                .Select(n => new
                {
                    n.MaNhatKy,
                    hoTenSinhVien = n.GhiDanh.SinhVien.HoTen,
                    maDinhDanh = n.GhiDanh.SinhVien.MaDinhDanh,
                    tuan = n.Tuan,
                    ngayThucHien = n.NgayThucHien,
                    congViecDaLam = n.CongViecDaLam,
                    ketQua = n.KetQua,
                    khoKhan = n.KhoKhan,
                    trangThaiDuyet = n.TrangThaiDuyet,
                    nhanXetGiangVien = n.NhanXetGiangVien
                })
                .ToListAsync();

            if (!diaries.Any()) return NotFound("Không tìm thấy nhật ký cho tuần này.");
            return Ok(diaries);
        }
    }

    // DTO để nhận dữ liệu nhật ký từ Frontend
    public class DiarySubmitDto
    {
        public string MaDinhDanh { get; set; } = "";
        public int TuanThucTap { get; set; }
        public DateTime? NgayThucHien { get; set; }
        public string NoiDungCongViec { get; set; } = "";
        public string? KetQuaDatDuoc { get; set; }
        public string? KhoKhanVongMac { get; set; }
        public string? KeHoachTuanToi { get; set; }
    }
}