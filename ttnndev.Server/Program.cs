using Microsoft.EntityFrameworkCore;
using ttnndev.Server.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Đăng ký kết nối DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Cấu hình CORS (Phải để trước các dịch vụ khác)
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 3. Cấu hình HTTP Pipeline (Thứ tự rất quan trọng!)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Phải gọi UseCors trước UseAuthorization và MapControllers
app.UseCors("AllowAll");

app.UseAuthentication(); // Nếu sau này bạn thêm JWT, hãy giữ dòng này
app.UseAuthorization();

app.MapControllers();

app.Run();