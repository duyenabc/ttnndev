namespace ttnndev.Server.DTOs
{
    // ---- Lớp / Kỳ ----
    public class CycleDto
    {
        public int MaKy { get; set; }
        public string TenKy { get; set; } = "";
        public string NamHoc { get; set; } = "";
        public string HocKy { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }

    public class TeacherClassDto
    {
        public int MaLop { get; set; }
        public int MaKy { get; set; }
        public string TenKy { get; set; } = "";
        public string TenLop { get; set; } = "";
        public int SoThuTuLop { get; set; }
        public string MaThamGia { get; set; } = "";
        public bool GhiDanhMo { get; set; }
        public DateTimeOffset? HanGhiDanh { get; set; }
        public string? LinkHopMacDinh { get; set; }
        public int SoSinhVien { get; set; }
        public DateTimeOffset NgayTao { get; set; }
    }

    public class CreateClassDto
    {
        public int MaKy { get; set; }
        public int SoThuTuLop { get; set; }
        public string? MaHocPhan { get; set; }
    }

    public class UpdateEnrollmentDto
    {
        public bool GhiDanhMo { get; set; }
        public DateTimeOffset? HanGhiDanh { get; set; }
    }

    // ---- Sinh viên ----
    public class AddStudentsDto
    {
        public List<string> MaSoSinhViens { get; set; } = new();
    }

    public class AddStudentResultDto
    {
        public string MaSoSinhVien { get; set; } = "";
        public bool ThanhCong { get; set; }
        public string? HoTen { get; set; }
        public string? LyDo { get; set; }
    }

    public class TeacherStudentDto
    {
        public int MaGhiDanh { get; set; }
        public int MaSinhVien { get; set; }
        public string MaSoSinhVien { get; set; } = "";
        public string HoTen { get; set; } = "";
        public string? Email { get; set; }
        public string? SoDienThoai { get; set; }
        public string? LopSinhHoat { get; set; }
        public int? MaNhom { get; set; }
        public string? TenNhom { get; set; }
        public string? ViTriThucTap { get; set; }
        public string? DonViThucTap { get; set; }
        public string TrangThaiThucTap { get; set; } = "";
        public string? TinhTrangTienDo { get; set; }
        public string? TenDeTai { get; set; }
        public string? TrangThaiDeTai { get; set; }
    }

    public class StopStudentDto
    {
        public string LyDo { get; set; } = "";
        public DateTime? NgayDung { get; set; }
    }

    // ---- Nhóm ----
    public class GroupDto
    {
        public int MaNhom { get; set; }
        public string TenNhom { get; set; } = "";
        public int SoThuTu { get; set; }
        public List<GroupMemberDto> ThanhVien { get; set; } = new();
    }

    public class GroupMemberDto
    {
        public int MaGhiDanh { get; set; }
        public string MaSoSinhVien { get; set; } = "";
        public string HoTen { get; set; } = "";
    }

    public class CreateGroupDto
    {
        public List<int> MaGhiDanhs { get; set; } = new();
    }

    public class UpdateGroupDto
    {
        public string? TenNhom { get; set; }
        public List<int> MaGhiDanhs { get; set; } = new();
    }

    // ---- Chấm điểm ----
    public class ScoreColumnDto
    {
        public int MaCotDiem { get; set; }
        public string TenCot { get; set; } = "";
        public decimal DiemToiDa { get; set; }
        public int ThuTu { get; set; }
    }

    public class CreateScoreColumnDto
    {
        public string TenCot { get; set; } = "";
        public decimal DiemToiDa { get; set; } = 10m;
    }

    public class GradebookRowDto
    {
        public int MaGhiDanh { get; set; }
        public string MaSoSinhVien { get; set; } = "";
        public string HoTen { get; set; } = "";
        public Dictionary<int, decimal?> Diem { get; set; } = new();
        public decimal? DiemTongKet { get; set; }
    }

    public class GradebookDto
    {
        public List<ScoreColumnDto> CotDiem { get; set; } = new();
        public List<GradebookRowDto> DongDiem { get; set; } = new();
        public bool DaKhoaSoDiem { get; set; }
    }

    public class SaveScoreItemDto
    {
        public int MaGhiDanh { get; set; }
        public int MaCotDiem { get; set; }
        public decimal? DiemSo { get; set; }
    }

    public class SaveScoresDto
    {
        public List<SaveScoreItemDto> Diem { get; set; } = new();
    }

    // ---- Lịch hướng dẫn ----
    public class ScheduleDto
    {
        public int Id { get; set; }
        public string TieuDe { get; set; } = "";
        public string? NoiDung { get; set; }
        public DateTime ThoiGianHop { get; set; }
        public string? LinkMeeting { get; set; }
        public string TrangThai { get; set; } = "";
        public int SinhVienId { get; set; }
        public string? TenSinhVien { get; set; }
        public string? MaSoSinhVien { get; set; }
    }

    public class CreateScheduleDto
    {
        public string TieuDe { get; set; } = "";
        public string? NoiDung { get; set; }
        public DateTime ThoiGianHop { get; set; }
        public string? LinkMeeting { get; set; }
        public int SinhVienId { get; set; }
    }

    public class UpdateScheduleDto
    {
        public string? TieuDe { get; set; }
        public string? NoiDung { get; set; }
        public DateTime? ThoiGianHop { get; set; }
        public string? LinkMeeting { get; set; }
        public string? TrangThai { get; set; }
    }
}
