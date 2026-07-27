using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ttnndev.Server.Models;

namespace ttnndev.Server.Services
{
    public interface ITokenService
    {
        string CreateAccessToken(NguoiDung user);
        (string plainToken, string tokenHash, DateTimeOffset expiresAt) CreateRefreshToken();
        string HashToken(string plainToken);
    }

    public class TokenService : ITokenService
    {
        private readonly JwtSettings _settings;

        public TokenService(IOptions<JwtSettings> settings)
        {
            _settings = settings.Value;
        }

        public string CreateAccessToken(NguoiDung user)
        {
            var now = DateTimeOffset.UtcNow;
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.MaNguoiDung.ToString()),
                new Claim("maDinhDanh", user.MaDinhDanh ?? string.Empty),
                new Claim("hoTen", user.HoTen ?? string.Empty),
                new Claim(ClaimTypes.Role, user.VaiTro ?? string.Empty),
                new Claim("vaiTro", user.VaiTro ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                notBefore: now.UtcDateTime,
                expires: now.AddMinutes(_settings.AccessTokenMinutes).UtcDateTime,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public (string plainToken, string tokenHash, DateTimeOffset expiresAt) CreateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(48);
            var plain = Convert.ToBase64String(bytes);
            var hash = HashToken(plain);
            var expires = DateTimeOffset.UtcNow.AddMinutes(_settings.RefreshTokenMinutes);
            return (plain, hash, expires);
        }

        public string HashToken(string plainToken)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(plainToken));
            return Convert.ToHexString(hash);
        }
    }
}
