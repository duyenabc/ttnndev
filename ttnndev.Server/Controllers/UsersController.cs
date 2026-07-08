using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ttnndev.Server.Data;
using ttnndev.Server.DTOs; // Import DTO

namespace ttnndev.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllUsers()
        {
            var users = await _context.NguoiDungs
                .Where(u => !u.DaXoa)
                .Select(u => new UserResponseDto // Chuyển đổi từ Entity sang DTO
                {
                    MaNguoiDung = u.MaNguoiDung,
                    MaDinhDanh = u.MaDinhDanh,
                    HoTen = u.HoTen,
                    Email = u.Email,
                    VaiTro = u.VaiTro
                })
                .ToListAsync();

            return Ok(users);
        }
    }
}