using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using SanalPos.Application.Common.Interfaces;
using System.Security.Cryptography;

namespace SanalPos.Infrastructure.Authentication
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            // PBKDF2 ile şifre hashleme
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            // Salt ve hash'i birleştir
            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            // Salt ve hash'i ayır
            var parts = passwordHash.Split('.');
            if (parts.Length != 2)
            {
                return false;
            }

            var salt = Convert.FromBase64String(parts[0]);
            var hash = parts[1];

            // Aynı salt ile şifreyi hashle
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            // Hash'leri karşılaştır
            return hash == hashed;
        }
    }
}