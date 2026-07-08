using Microsoft.AspNetCore.Mvc;
using ttnndev.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace ttnndev.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PracticumController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PracticumController(AppDbContext context)
        {
            _context = context;
        }

        // E03.1 - Lấy danh sách lớp của giảng viên
        [HttpGet("my-classes/{giangVienId}")]
        public async Task<IActionResult> GetMyClasses(int giangVienId)
        {
            var classes = await _context.LopThucTaps
                                        .Include(l => l.KyThucTap)
                                        .Where(l => l.MaGiangVien == giangVienId)
                                        .ToListAsync();
            return Ok(classes);
        }

        // E03.5 - Xem danh sách sinh viên trong lớp
        [HttpGet("class/{maLop}/students")]
        public async Task<IActionResult> GetStudentsInClass(int maLop)
        {
            var students = await _context.GhiDanhSinhViens
                                         .Include(g => g.SinhVien)
                                         .Where(g => g.MaLop == maLop)
                                         .ToListAsync();
            return Ok(students);
        }
    }
}