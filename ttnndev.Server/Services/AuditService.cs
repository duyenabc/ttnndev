using System;
using System.Threading.Tasks;
using ttnndev.Server.Data;
using ttnndev.Server.Models;

namespace ttnndev.Server.Services
{
    public interface IAuditService
    {
        Task LogAsync(int maNguoiThucHien, string hanhDong, int? maDoiTuong = null,
            string? giaTriCu = null, string? giaTriMoi = null, int? maBatch = null);
    }

    public class AuditService : IAuditService
    {
        private readonly AppDbContext _context;

        public AuditService(AppDbContext context)
        {
            _context = context;
        }

        // Ghi log thao tác quản lý tài khoản (E01.2). Không tự SaveChanges nếu muốn gom;
        // ở đây SaveChanges ngay để đơn giản hóa và đảm bảo log luôn được ghi.
        public async Task LogAsync(int maNguoiThucHien, string hanhDong, int? maDoiTuong = null,
            string? giaTriCu = null, string? giaTriMoi = null, int? maBatch = null)
        {
            _context.AuditLogs.Add(new AuditLog
            {
                MaNguoiThucHien = maNguoiThucHien,
                HanhDong = hanhDong,
                MaDoiTuong = maDoiTuong,
                MaBatch = maBatch,
                GiaTriCu = ToJson(giaTriCu),
                GiaTriMoi = ToJson(giaTriMoi),
                ThoiDiem = DateTimeOffset.UtcNow
            });
            await _context.SaveChangesAsync();
        }

        // Đảm bảo giá trị lưu vào cột jsonb luôn là JSON hợp lệ
        private static string? ToJson(string? value)
        {
            if (value == null) return null;
            try
            {
                using var _ = System.Text.Json.JsonDocument.Parse(value);
                return value; // đã là JSON hợp lệ
            }
            catch (System.Text.Json.JsonException)
            {
                return System.Text.Json.JsonSerializer.Serialize(value); // bọc thành chuỗi JSON
            }
        }
    }
}
