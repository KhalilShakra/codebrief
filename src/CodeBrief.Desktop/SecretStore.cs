using System.Security.Cryptography;
using System.Text;

namespace CodeBrief.Desktop;

internal static class SecretStore
{
    private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("CodeBrief.v1.api");

    public static string Protect(string plain)
    {
        if (string.IsNullOrEmpty(plain))
        {
            return "";
        }

        var bytes = ProtectedData.Protect(Encoding.UTF8.GetBytes(plain), Entropy, DataProtectionScope.CurrentUser);
        return Convert.ToBase64String(bytes);
    }

    public static string Unprotect(string? cipher)
    {
        if (string.IsNullOrWhiteSpace(cipher))
        {
            return "";
        }

        try
        {
            var bytes = ProtectedData.Unprotect(Convert.FromBase64String(cipher), Entropy, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return "";
        }
    }
}
