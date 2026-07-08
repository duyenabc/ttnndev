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

        // 1.1 Sinh viên nộp nhật ký (Có validate ngày tháng)
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitDiary([FromBody] NhatKy nhatKy)
        {
            // Kiểm tra dữ liệu đầu vào cơ bản
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // 1. Lấy thông tin lớp và kỳ thực tập để kiểm tra ngày tháng
            var ghiDanh = await _context.GhiDanhSinhViens
                .Include(g => g.LopThucTap)
                .ThenInclude(l => l.KyThucTap)
                .FirstOrDefaultAsync(g => g.MaGhiDanh == nhatKy.MaGhiDanh);

            if (ghiDanh == null) return NotFound("Không tìm thấy thông tin ghi danh.");

            var kyThucTap = ghiDanh.LopThucTap.KyThucTap;

            // 2. Validate: Nhật ký phải nằm trong khoảng thời gian của Kỳ thực tập
            if (nhatKy.NgayThucHien < kyThucTap.NgayBatDau || nhatKy.NgayThucHien > kyThucTap.NgayKetThuc)
            {
                return BadRequest("Ngày nộp nhật ký không thuộc phạm vi kỳ thực tập.");
            }

            // 3. Validate: Không được nộp cho tương lai
            if (nhatKy.NgayThucHien > DateTime.Now)
            {
                return BadRequest("Không thể nộp nhật ký cho ngày trong tương lai.");
            }

            // 4. Lưu vào database
            nhatKy.TrangThaiDuyet = "ChoDuyet"; // Thiết lập trạng thái mặc định
            _context.NhatKys.Add(nhatKy);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Lưu nhật ký thành công!" });
        }

        // 1.2 Giảng viên xem nhật ký cả lớp (Weekly Digest)
        [HttpGet("class/{maLop}/weekly/{weekNumber}")]
        public async Task<IActionResult> GetWeeklyDiary(int maLop, int weekNumber)
        {
            var diaries = await _context.NhatKys
                .Include(n => n.GhiDanh)
                .ThenInclude(g => g.SinhVien) // Lấy thêm thông tin sinh viên để hiển thị tên
                .Where(n => n.GhiDanh.MaLop == maLop && n.Tuan == weekNumber)
                .ToListAsync();

            if (!diaries.Any()) return NotFound("Không tìm thấy nhật ký cho tuần này.");

            return Ok(diaries);
        }
    }
}