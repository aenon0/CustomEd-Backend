// Purpose : The PasswordHasher class is responsible for hashing and verifying passwords using the Rfc2898DeriveBytes algorithm. It provides methods to hash a password and to verify whether a password matches a hashed password.
using System.Security.Cryptography;
using CustomEd.User.Service.PasswordService.Interfaces;
using CustomEd.User.Service.Settings;
using Microsoft.Extensions.Options;

namespace CustomEd.User.Service.Password
{
    /// <summary>
    /// Provides functionality to hash and verify passwords using the Rfc2898DeriveBytes algorithm.
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        private readonly IOptions<PasswordHasherSettings> _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordHasher"/> class.
        /// </summary>
        /// <param name="options">The options for the password hasher.</param>
        public PasswordHasher(IOptions<PasswordHasherSettings> options)
        {
            _options = options;
        }

        /// <summary>
        /// Hashes the specified password.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>The hashed password.</returns>
        public string HashPassword(string password)
        {
            using var algorithm = new Rfc2898DeriveBytes(
                password,
                _options.Value.SaltSize,
                _options.Value.Iterations,
                HashAlgorithmName.SHA512
            );

            string key = Convert.ToBase64String(algorithm.GetBytes(_options.Value.KeySize));
            string salt = Convert.ToBase64String(algorithm.Salt);

            return $"{_options.Value.Iterations}.{salt}.{key}";
        }

        /// <summary>
        /// Verifies whether the specified password matches the hashed password.
        /// </summary>
        /// <param name="password">The password to verify.</param>
        /// <param name="hashedPassword">The hashed password to compare against.</param>
        /// <returns>True if the password is verified, false otherwise.</returns>
        public bool VerifyPassword(string password, string hashedPassword)
        {
            string[] parts = hashedPassword.Split('.', 3);

            if (parts.Length != 3)
            {
                return false;
            }

            int iterations = Convert.ToInt32(parts[0]);
            byte[] salt = Convert.FromBase64String(parts[1]);
            byte[] key = Convert.FromBase64String(parts[2]);

            using var algorithm = new Rfc2898DeriveBytes(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA512
            );

            byte[] keyToCheck = algorithm.GetBytes(_options.Value.KeySize);

            return keyToCheck.SequenceEqual(key);
        }
    }
}
