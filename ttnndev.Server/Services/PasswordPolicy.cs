using System.Linq;

namespace ttnndev.Server.Services
{
    public static class PasswordPolicy
    {
        // E15.3/E15.6: tối thiểu 8 ký tự, gồm chữ hoa, chữ thường và số
        public static bool IsValid(string? password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                return false;
            return password.Any(char.IsUpper)
                && password.Any(char.IsLower)
                && password.Any(char.IsDigit);
        }

        // Sinh mật khẩu tạm hợp lệ theo chính sách (E00.9)
        public static string GenerateTemporary()
        {
            const string uppers = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            const string lowers = "abcdefghijkmnpqrstuvwxyz";
            const string digits = "23456789";
            const string all = uppers + lowers + digits;
            var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            var chars = new char[12];
            chars[0] = Pick(rng, uppers);
            chars[1] = Pick(rng, lowers);
            chars[2] = Pick(rng, digits);
            for (int i = 3; i < chars.Length; i++)
                chars[i] = Pick(rng, all);
            // xáo trộn
            for (int i = chars.Length - 1; i > 0; i--)
            {
                int j = Next(rng, i + 1);
                (chars[i], chars[j]) = (chars[j], chars[i]);
            }
            return new string(chars);
        }

        private static char Pick(System.Security.Cryptography.RandomNumberGenerator rng, string set)
            => set[Next(rng, set.Length)];

        private static int Next(System.Security.Cryptography.RandomNumberGenerator rng, int max)
        {
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            return (int)(System.BitConverter.ToUInt32(bytes, 0) % (uint)max);
        }
    }
}
