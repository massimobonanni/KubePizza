using System.Runtime.InteropServices;

namespace KubePizza.Core.Security;

/// <summary>
/// Factory class for creating platform-specific protected data providers.
/// </summary>
/// <remarks>
/// This factory automatically detects the current operating system platform and returns
/// the appropriate implementation of <see cref="IProtectedDataProvider"/> for data encryption
/// and decryption operations.
/// </remarks>
public static class ProtectedDataProviderFactory
{
    /// <summary>
    /// Creates a platform-specific protected data provider based on the current operating system.
    /// </summary>
    /// <returns>
    /// An <see cref="IProtectedDataProvider"/> implementation suitable for the current platform:
    /// <list type="bullet">
    /// <item><description><see cref="WindowsProtectedDataProvider"/> for Windows platforms</description></item>
    /// <item><description><see cref="LinuxProtectedDataProvider"/> for Linux and macOS platforms</description></item>
    /// </list>
    /// </returns>
    /// <exception cref="PlatformNotSupportedException">
    /// Thrown when the current platform is not Windows, Linux, or macOS.
    /// </exception>
    public static IProtectedDataProvider Create()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new WindowsProtectedDataProvider();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || 
                 RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return new LinuxProtectedDataProvider();
        }
        else
        {
            throw new PlatformNotSupportedException($"Platform {RuntimeInformation.OSDescription} is not supported");
        }
    }
}