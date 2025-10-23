namespace KubePizza.Core.Security;

/// <summary>
/// Provides data protection services for encrypting and decrypting sensitive data.
/// </summary>
public interface IProtectedDataProvider
{
    /// <summary>
    /// Asynchronously encrypts the specified data using platform-specific data protection mechanisms.
    /// </summary>
    /// <param name="data">The byte array containing the data to be protected.</param>
    /// <param name="optionalEntropy">An optional additional byte array used to increase the complexity of the encryption. This parameter can be null.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the encrypted data as a byte array.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is null.</exception>
    /// <exception cref="System.Security.Cryptography.CryptographicException">Thrown when the data protection operation fails.</exception>
    Task<byte[]> ProtectAsync(byte[] data, string? optionalEntropy = null);

    /// <summary>
    /// Asynchronously decrypts the specified encrypted data using platform-specific data protection mechanisms.
    /// </summary>
    /// <param name="encryptedData">The byte array containing the encrypted data to be unprotected.</param>
    /// <param name="optionalEntropy">An optional additional byte array that was used during encryption. This must match the entropy used during protection. This parameter can be null.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the decrypted data as a byte array.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="encryptedData"/> is null.</exception>
    /// <exception cref="System.Security.Cryptography.CryptographicException">Thrown when the data unprotection operation fails or the data cannot be decrypted.</exception>
    Task<byte[]> UnprotectAsync(byte[] encryptedData, string? optionalEntropy = null);
}