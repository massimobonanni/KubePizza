using System.Security.Cryptography;
using System.Text;

namespace KubePizza.Core.Security;

/// <summary>
/// Provides data protection services for Linux platforms using AES encryption with PBKDF2 key derivation.
/// This implementation creates deterministic keys based on user identity and machine characteristics.
/// </summary>
public class LinuxProtectedDataProvider : IProtectedDataProvider
{
    /// <summary>
    /// The size of the encryption key in bytes (256 bits).
    /// </summary>
    private const int KeySize = 32; // 256 bits
    
    /// <summary>
    /// The size of the initialization vector in bytes (128 bits).
    /// </summary>
    private const int IvSize = 16;  // 128 bits

    /// <summary>
    /// Asynchronously encrypts the specified data using AES encryption with a deterministic key.
    /// </summary>
    /// <param name="data">The byte array containing the data to be protected.</param>
    /// <param name="optionalEntropy">An optional additional string used to increase the complexity of the encryption. This parameter can be null.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the encrypted data as a byte array, with the IV prepended.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is null.</exception>
    /// <exception cref="CryptographicException">Thrown when the data protection operation fails.</exception>
    public async Task<byte[]> ProtectAsync(byte[] data, string? optionalEntropy = null)
    {
        using var aes = Aes.Create();
        var key = await GenerateKeyAsync(optionalEntropy);
        aes.Key = key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        using var msEncrypt = new MemoryStream();
        
        // Write IV first
        await msEncrypt.WriteAsync(aes.IV, 0, aes.IV.Length);
        
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        {
            await csEncrypt.WriteAsync(data, 0, data.Length);
        }
        
        return msEncrypt.ToArray();
    }

    /// <summary>
    /// Asynchronously decrypts the specified encrypted data using AES decryption with a deterministic key.
    /// </summary>
    /// <param name="encryptedData">The byte array containing the encrypted data to be unprotected, with the IV at the beginning.</param>
    /// <param name="optionalEntropy">An optional additional string that was used during encryption. This must match the entropy used during protection. This parameter can be null.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the decrypted data as a byte array.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="encryptedData"/> is null.</exception>
    /// <exception cref="CryptographicException">Thrown when the data unprotection operation fails or the data cannot be decrypted.</exception>
    public async Task<byte[]> UnprotectAsync(byte[] encryptedData, string? optionalEntropy = null)
    {
        using var aes = Aes.Create();
        var key = await GenerateKeyAsync(optionalEntropy);
        aes.Key = key;

        using var msDecrypt = new MemoryStream(encryptedData);
        
        // Read IV
        var iv = new byte[IvSize];
        await msDecrypt.ReadAsync(iv, 0, iv.Length);
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var msPlain = new MemoryStream();
        
        await csDecrypt.CopyToAsync(msPlain);
        return msPlain.ToArray();
    }

    /// <summary>
    /// Generates a deterministic encryption key based on user identity, machine characteristics, and optional entropy using PBKDF2.
    /// </summary>
    /// <param name="optionalEntropy">An optional additional string to include in the key material for increased entropy.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a 256-bit encryption key as a byte array.</returns>
    private Task<byte[]> GenerateKeyAsync(string? optionalEntropy)
    {
        // Create a deterministic key based on user identity and machine
        var keyMaterial = new StringBuilder();
        keyMaterial.Append(Environment.UserName);
        keyMaterial.Append(Environment.MachineName);
        keyMaterial.Append(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
        
        if (!string.IsNullOrEmpty(optionalEntropy))
        {
            keyMaterial.Append(optionalEntropy);
        }

        // Use PBKDF2 to derive a key using the static method to avoid SYSLIB0060
        var passwordBytes = Encoding.UTF8.GetBytes(keyMaterial.ToString());
        var saltBytes = Encoding.UTF8.GetBytes("KubePizza.Salt"); // Static salt

        var key = Rfc2898DeriveBytes.Pbkdf2(
            passwordBytes,
            saltBytes,
            100000, // Iterations
            HashAlgorithmName.SHA256,
            KeySize);

        return Task.FromResult(key);
    }
}