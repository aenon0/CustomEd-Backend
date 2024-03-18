// Purpose: This file contains the interface for hashing and verifying passwords.

namespace CustomEd.User.Service.PasswordService.Interfaces;

/// <summary>
/// Represents an interface for hashing and verifying passwords.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes the specified password.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>The hashed password.</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifies whether the specified password matches the hashed password.
    /// </summary>
    /// <param name="password">The password to verify.</param>
    /// <param name="passwordHash">The hashed password to compare against.</param>
    /// <returns>True if the password matches the hashed password; otherwise, false.</returns>
    bool VerifyPassword(string password, string passwordHash);
}