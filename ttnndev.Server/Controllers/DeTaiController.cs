using Microsoft.AspNetCore.Mvc;
using ttnndev.Server.Data;
using ttnndev.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace ttnndev.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeTaiController : ControllerBase
    {
        private readonly AppDbContext _context;
        public DeTaiController(AppDbContext context) => _context = context;

        // GET api/DeTai/teacher/{maDinhDanhGV}/topics
        // Giảng viên xem danh sách sinh viên & đề tài thuộc các lớp mình phụ trách
        [HttpGet("teacher/{maDinhDanhGV}/topics")]
        public async Task<IActionResult> GetTopicsForTeacher(string maDinhDanhGV)
        {
            var giangVien = await _context.NguoiDungs
                .FirstOrDefaultAsync(u => u.MaDinhDanh == maDinhDanhGV && !u.DaXoa);

            if (giangVien == null) return NotFound("Không tìm thấy giảng viên.");

            // Join DeTai -> GhiDanhSinhVien -> LopThucTap để lọc theo giảng viên
            var topics = await (
                from dt in _context.DeTais
                join gd in _context.GhiDanhSinhViens on dt.MaGhiDanh equals gd.MaGhiDanh
                join lop in _context.LopThucTaps on gd.MaLop equals lop.MaLop
                join sv in _context.NguoiDungs on gd.MaSinhVien equals sv.MaNguoiDung
                where lop.MaGiangVien == giangVien.MaNguoiDung
                select new
                {
                    id = dt.MaDeTai,
                    mssv = sv.MaDinhDanh,
                    hoTen = sv.HoTen,
                    tenLop = lop.TenLop,
                    tenDeTai = dt.TenDeTai,
                    moTa = dt.MoTa,
                    trangThaiDeTai = dt.TrangThai,
                    ngayTao = dt.NgayTao
                }
            ).ToListAsync();

            return Ok(topics);
        }

        // POST api/DeTai/register
        // Sinh viên đăng ký đề tài
        [HttpPost("register")]
        public async Task<IActionResult> RegisterTopic([FromBody] DeTaiSubmitDto dto)
        {
            // Tìm sinh viên
            var sinhVien = await _context.NguoiDungs
                .FirstOrDefaultAsync(u => u.MaDinhDanh == dto.MaDinhDanh && !u.DaXoa);

            if (sinhVien == null) return NotFound("Không tìm thấy sinh viên.");

            // Tìm GhiDanh
            var ghiDanh = await _context.GhiDanhSinhViens
                .FirstOrDefaultAsync(g => g.MaSinhVien == sinhVien.MaNguoiDung);

            if (ghiDanh == null) return NotFound("Sinh viên chưa ghi danh vào lớp thực tập.");

            var deTai = new DeTai
            {
                MaGhiDanh = ghiDanh.MaGhiDanh,
                TenDeTai = dto.TenDeTai,
                MoTa = dto.MoTa,
                TrangThai = "ChoDuyet",
                KhoaChinhSua = false
            };
            _context.DeTais.Add(deTai);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Đăng ký đề tài thành công!", maDeTai = deTai.MaDeTai });
        }

        // PUT api/DeTai/evaluate/{maDeTai}
        // Giảng viên duyệt / từ chối / yêu cầu sửa đề tài
        [HttpPut("evaluate/{maDeTai}")]
        public async Task<IActionResult> EvaluateTopic(int maDeTai, [FromBody] EvaluateDto dto)
        {
            var deTai = await _context.DeTais.FindAsync(maDeTai);
            if (deTai == null) return NotFound("Không tìm thấy đề tài.");

            // Map trạng thái từ FE sang BE
            var trangThaiMap = new Dictionary<string, string>
            {
                { "Đã duyệt", "DaDuyet" },
                { "Từ chối", "TuChoi" },
                { "Yêu cầu sửa", "YeuCauSua" },
                // Cũng hỗ trợ tiếng Anh nếu FE gửi
                { "DaDuyet", "DaDuyet" },
                { "TuChoi", "TuChoi" },
                { "YeuCauSua", "YeuCauSua" }
            };

            if (!trangThaiMap.TryGetValue(dto.TrangThai, out var trangThaiDB))
                return BadRequest($"Trạng thái không hợp lệ: {dto.TrangThai}");

            deTai.TrangThai = trangThaiDB;
            // Khoá chỉnh sửa nếu đã duyệt hoặc từ chối
            deTai.KhoaChinhSua = trangThaiDB == "DaDuyet" || trangThaiDB == "TuChoi";

            await _context.SaveChangesAsync();
            return Ok(new { message = $"Đã cập nhật trạng thái đề tài: {trangThaiDB}", maDeTai });
        }

        // PUT api/DeTai/approve/{maDeTai} (legacy)
        [HttpPut("approve/{maDeTai}")]
        public async Task<IActionResult> ApproveTopic(int maDeTai)
        {
            var deTai = await _context.DeTais.FindAsync(maDeTai);
            if (deTai == null) return NotFound();
            deTai.TrangThai = "DaDuyet";
            await _context.SaveChangesAsync();
            return Ok();
        }
    }

    public class DeTaiSubmitDto
    {
        public string MaDinhDanh { get; set; } = "";
        public string TenDeTai { get; set; } = "";
        public string MoTa { get; set; } = "";
    }

    public class EvaluateDto
    {
        public string TrangThai { get; set; } = "";
        public string? NhanXet { get; set; }
    }
}