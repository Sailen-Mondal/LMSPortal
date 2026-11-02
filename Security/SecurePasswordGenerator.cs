using System;
using System.Security.Cryptography;

namespace LMSPortal.Security
{
    public static class SecurePasswordGenerator
    {
        private const string Upper = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
        private const string Lower = "abcdefghijkmnopqrstuvwxyz";
        private const string Digits = "0123456789";
        private const string Specials = "!@#$%^&*()-_=+";

        public static string Generate(int length = 16)
        {
            if (length < 12) length = 12;

            var chars = new char[length];
            var all = Upper + Lower + Digits + Specials;

            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] randomBytes = new byte[length];
                rng.GetBytes(randomBytes);

                // Guarantee at least one from each character class
                chars[0] = Upper[randomBytes[0] % Upper.Length];
                chars[1] = Lower[randomBytes[1] % Lower.Length];
                chars[2] = Digits[randomBytes[2] % Digits.Length];
                chars[3] = Specials[randomBytes[3] % Specials.Length];

                for (int i = 4; i < length; i++)
                {
                    chars[i] = all[randomBytes[i] % all.Length];
                }

                // Cryptographic Fisher-Yates shuffle
                byte[] shuffleBytes = new byte[length];
                rng.GetBytes(shuffleBytes);
                for (int i = length - 1; i > 0; i--)
                {
                    int j = shuffleBytes[i] % (i + 1);
                    char temp = chars[i];
                    chars[i] = chars[j];
                    chars[j] = temp;
                }
            }

            return new string(chars);
        }
    }
}
