using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ttnndev.Server.Data;
using ttnndev.Server.Services;

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

// 3. Cấu hình JWT + các service tài khoản
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>() ?? new JwtSettings();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuditService, AuditService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Giữ nguyên tên claim gốc (sub, iat) thay vì remap sang URI dài
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ClockSkew = TimeSpan.FromSeconds(30),
            NameClaimType = "sub",
            RoleClaimType = ClaimTypes.Role
        };

        // E15.7.1: token phát hành trước TokenValidFrom bị coi là vô hiệu (đổi mật khẩu / khóa tài khoản / reset)
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async ctx =>
            {
                var db = ctx.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
                var sub = ctx.Principal?.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
                var iat = ctx.Principal?.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Iat);
                if (!int.TryParse(sub, out var userId))
                {
                    ctx.Fail("invalid_subject");
                    return;
                }
                var user = await db.NguoiDungs.FindAsync(userId);
                if (user == null || user.DaXoa || user.TrangThaiTaiKhoan == "BiKhoa")
                {
                    ctx.Fail("account_unavailable");
                    return;
                }
                if (long.TryParse(iat, out var iatUnix))
                {
                    var issuedAt = DateTimeOffset.FromUnixTimeSeconds(iatUnix);
                    // cho phép lệch 30s để tránh false-positive do làm tròn giây
                    if (issuedAt < user.TokenValidFrom.AddSeconds(-30))
                    {
                        ctx.Fail("token_revoked");
                    }
                }
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 4. Cấu hình HTTP Pipeline (Thứ tự rất quan trọng!)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Phải gọi UseCors trước UseAuthentication/UseAuthorization và MapControllers
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
