using System;
using System.Collections.Generic;

namespace ttnndev.Server.DTOs
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class AccountSummaryDto
    {
        public int TongSo { get; set; }
        public int ChoKichHoat { get; set; }
        public int DangHoatDong { get; set; }
        public int BiKhoa { get; set; }
    }

    public class AccountListItemDto
    {
        public int MaNguoiDung { get; set; }
        public string MaDinhDanh { get; set; } = null!;
        public string HoTen { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? SoDienThoai { get; set; }
        public string VaiTro { get; set; } = null!;
        public string TrangThaiTaiKhoan { get; set; } = null!;
        public bool QuyenQuanLyNguoiDung { get; set; }
        public DateTimeOffset NgayTao { get; set; }
    }

    public class AccountDetailDto : AccountListItemDto
    {
        public string? TenKhoa { get; set; }
        public string? TenBoMon { get; set; }
        public string? LopSinhHoat { get; set; }
        public DateTimeOffset NgayCapNhat { get; set; }
    }

    public class CreateUserDto
    {
        public string MaDinhDanh { get; set; } = null!;
        public string HoTen { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? SoDienThoai { get; set; }
        public string VaiTro { get; set; } = null!;
        public int? MaKhoa { get; set; }
        public int? MaBoMon { get; set; }
        public string? LopSinhHoat { get; set; }
        public bool CapTaiKhoanNgay { get; set; } = false;
        public bool QuyenQuanLyNguoiDung { get; set; } = false;
    }

    public class LockDto
    {
        public string LyDo { get; set; } = null!;
    }

    public class RejectDto
    {
        public string LyDo { get; set; } = null!;
    }

    public class BulkIdsDto
    {
        public List<int> Ids { get; set; } = new();
        public string? LyDo { get; set; }
    }

    public class PermissionToggleDto
    {
        public bool QuyenQuanLyNguoiDung { get; set; }
    }

    public class TempPasswordResultDto
    {
        public string MatKhauTam { get; set; } = null!;
        public string Message { get; set; } = null!;
    }

    public class AccountRequestDto
    {
        public int MaYeuCau { get; set; }
        public string LoaiYeuCau { get; set; } = null!;
        public int MaNguoiYeuCau { get; set; }
        public string? TenNguoiYeuCau { get; set; }
        public int? MaDoiTuong { get; set; }
        public string? TenDoiTuong { get; set; }
        public string? MaDinhDanhDoiTuong { get; set; }
        public string? EmailDoiTuong { get; set; }
        public string? LyDoYeuCau { get; set; }
        public string TrangThai { get; set; } = null!;
        public string? LyDoTuChoi { get; set; }
        public DateTimeOffset NgayTao { get; set; }
        public DateTimeOffset? NgayXuLy { get; set; }
    }

    public class CreateAccountRequestDto
    {
        public string LoaiYeuCau { get; set; } = null!; // CapTaiKhoan/KhoaTaiKhoan/MoKhoaTaiKhoan
        public int? MaDoiTuong { get; set; }
        public string? LyDo { get; set; }
        // Dùng cho LoaiYeuCau = CapTaiKhoan (tạo user Nhap rồi gửi yêu cầu)
        public CreateUserDto? NguoiDungMoi { get; set; }
    }

    public class AuditLogDto
    {
        public long MaLog { get; set; }
        public string HanhDong { get; set; } = null!;
        public int MaNguoiThucHien { get; set; }
        public string? TenNguoiThucHien { get; set; }
        public int? MaDoiTuong { get; set; }
        public string? TenDoiTuong { get; set; }
        public DateTimeOffset ThoiDiem { get; set; }
    }
}
