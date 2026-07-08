using Microsoft.AspNetCore.Mvc;
using ttnndev.Server.Data;
using ttnndev.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace ttnndev.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiemController : ControllerBase
    {
        private readonly AppDbContext _context;
        public DiemController(AppDbContext context) => _context = context;

        // Lấy bảng điểm lớp
        [HttpGet("class/{maLop}/scores")]
        public async Task<IActionResult> GetScores(int maLop)
        {
            var scores = await _context.DiemSinhViens
                .Include(d => d.GhiDanh)
                .ThenInclude(g => g.LopThucTap)
                .Where(d => d.GhiDanh.MaLop == maLop)
                .ToListAsync();
            return Ok(scores);
        }
    }
}