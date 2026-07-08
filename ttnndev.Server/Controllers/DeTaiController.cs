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

        // Sinh viên đăng ký đề tài
        [HttpPost("register")]
        public async Task<IActionResult> RegisterTopic([FromBody] DeTai deTai)
        {
            _context.DeTais.Add(deTai);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // Giảng viên duyệt đề tài
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
}