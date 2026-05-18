using System.Security.Cryptography;

namespace Sprint3.Security
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100_000;

        public static string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] key = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                KeySize);

            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
        }

        public static bool Verify(string password, string storedHash)
        {
            if (string.IsNullOrWhiteSpace(storedHash))
            {
                return false;
            }

            string[] parts = storedHash.Split('.');
            if (parts.Length != 2)
            {
                return false;
            }

            byte[] salt;
            byte[] expectedKey;

            try
            {
                salt = Convert.FromBase64String(parts[0]);
                expectedKey = Convert.FromBase64String(parts[1]);
            }
            catch
            {
                return false;
            }

            byte[] actualKey = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                expectedKey.Length);

            return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
        }
    }
}
