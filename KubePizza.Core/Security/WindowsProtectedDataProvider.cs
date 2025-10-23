using System.Security.Cryptography;

namespace KubePizza.Core.Security;

/// <summary>
/// Provides data protection services using the Windows Data Protection API (DPAPI).
/// This implementation encrypts data for the current user context only.
/// </summary>
public class WindowsProtectedDataProvider : IProtectedDataProvider
{
    /// <summary>
    /// Asynchronously encrypts the specified data using the Windows Data Protection API.
    /// </summary>
    /// <param name="data">The data to encrypt. Cannot be null.</param>
    /// <param name="optionalEntropy">Optional additional entropy to make the encryption more secure. 
    /// If provided, the same entropy must be used during decryption.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the encrypted data as a byte array.</returns>
    /// <exception cref="NotSupportedException">Thrown when the Windows Data Protection API is not supported on the current platform.</exception>
    /// <exception cref="CryptographicException">Thrown when the encryption operation fails.</exception>
    public Task<byte[]> ProtectAsync(byte[] data, string? optionalEntropy = null)
    {
        try
        {
            byte[]? entropy = optionalEntropy != null ? 
                System.Text.Encoding.UTF8.GetBytes(optionalEntropy) : null;

#pragma warning disable CA1416 // Validate platform compatibility
            var protectedData = ProtectedData.Protect(data, entropy, DataProtectionScope.CurrentUser);
#pragma warning restore CA1416 // Validate platform compatibility
            return Task.FromResult(protectedData);
        }
        catch (PlatformNotSupportedException)
        {
            throw new NotSupportedException("Windows Data Protection API is not supported on this platform");
        }
    }

    /// <summary>
    /// Asynchronously decrypts the specified encrypted data using the Windows Data Protection API.
    /// </summary>
    /// <param name="encryptedData">The encrypted data to decrypt. Cannot be null.</param>
    /// <param name="optionalEntropy">Optional entropy that was used during encryption. 
    /// Must match the entropy used during the encryption process if one was provided.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the decrypted data as a byte array.</returns>
    /// <exception cref="NotSupportedException">Thrown when the Windows Data Protection API is not supported on the current platform.</exception>
    /// <exception cref="CryptographicException">Thrown when the decryption operation fails, such as when the data was encrypted with different entropy or by a different user.</exception>
    public Task<byte[]> UnprotectAsync(byte[] encryptedData, string? optionalEntropy = null)
    {
        try
        {
            byte[]? entropy = optionalEntropy != null ? 
                System.Text.Encoding.UTF8.GetBytes(optionalEntropy) : null;

#pragma warning disable CA1416 // Validate platform compatibility
            var unprotectedData = ProtectedData.Unprotect(encryptedData, entropy, DataProtectionScope.CurrentUser);
#pragma warning restore CA1416 // Validate platform compatibility
            return Task.FromResult(unprotectedData);
        }
        catch (PlatformNotSupportedException)
        {
            throw new NotSupportedException("Windows Data Protection API is not supported on this platform");
        }
    }
}