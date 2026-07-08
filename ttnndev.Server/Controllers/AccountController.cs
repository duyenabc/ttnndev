using Microsoft.AspNetCore.Mvc;
using ttnndev.Server.Data;
using ttnndev.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace ttnndev.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // E00.5 - Lấy danh sách tài khoản theo vai trò
        [HttpGet("users/{role}")]
        public async Task<IActionResult> GetUsersByRole(string role)
        {
            var users = await _context.NguoiDungs
                                      .Where(u => u.VaiTro == role && !u.DaXoa)
                                      .ToListAsync();
            return Ok(users);
        }

        // E00.2 - Import danh sách (Mô phỏng logic, bạn cần thêm thư viện EPPlus để đọc Excel)
        [HttpPost("import")]
        public async Task<IActionResult> ImportUsers([FromBody] List<NguoiDung> users)
        {
            foreach (var user in users)
            {
                // Logic kiểm tra trùng lặp mã định danh
                if (await _context.NguoiDungs.AnyAsync(u => u.MaDinhDanh == user.MaDinhDanh))
                    continue;
                _context.NguoiDungs.Add(user);
            }
            await _context.SaveChangesAsync();
            return Ok("Import thành công!");
        }
    }
}