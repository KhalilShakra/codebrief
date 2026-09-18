using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CodeBrief.Licensing;

public sealed class LicensePayload
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTimeOffset ExpiresUtc { get; set; }
    public string Edition { get; set; } = "pro";
}

public sealed class LicenseValidation
{
    public bool IsValid { get; init; }
    public string Message { get; init; } = "";
    public LicensePayload? Payload { get; init; }
}

public static class LicenseCrypt
{
    public const string Prefix = "CB1";
    public const int TrialAnalyses = 20;

    public static (string PublicPem, string PrivatePem) CreateKeyPair()
    {
        using var rsa = RSA.Create(2048);
        return (rsa.ExportSubjectPublicKeyInfoPem(), rsa.ExportPkcs8PrivateKeyPem());
    }

    public static string Issue(LicensePayload payload, string privatePem)
    {
        var json = JsonSerializer.Serialize(payload);
        var data = Encoding.UTF8.GetBytes(json);
        using var rsa = RSA.Create();
        rsa.ImportFromPem(privatePem);
        var signature = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return $"{Prefix}.{ToUrl(data)}.{ToUrl(signature)}";
    }

    public static LicenseValidation Verify(string? licenseKey, string publicPem)
    {
        if (string.IsNullOrWhiteSpace(licenseKey))
        {
            return new LicenseValidation { IsValid = false, Message = "Ingen licensnyckel." };
        }

        var parts = licenseKey.Trim().Split('.');
        if (parts.Length != 3 || parts[0] != Prefix)
        {
            return new LicenseValidation { IsValid = false, Message = "Ogiltigt nyckelformat." };
        }

        try
        {
            var data = FromUrl(parts[1]);
            var signature = FromUrl(parts[2]);
            using var rsa = RSA.Create();
            rsa.ImportFromPem(publicPem);
            if (!rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
            {
                return new LicenseValidation { IsValid = false, Message = "Signaturen stämmer inte." };
            }

            var payload = JsonSerializer.Deserialize<LicensePayload>(Encoding.UTF8.GetString(data));
            if (payload is null)
            {
                return new LicenseValidation { IsValid = false, Message = "Nyckeln kunde inte läsas." };
            }

            if (payload.ExpiresUtc < DateTimeOffset.UtcNow)
            {
                return new LicenseValidation { IsValid = false, Message = "Licensen har gått ut.", Payload = payload };
            }

            return new LicenseValidation { IsValid = true, Message = "Licens aktiv.", Payload = payload };
        }
        catch (Exception ex)
        {
            return new LicenseValidation { IsValid = false, Message = "Kunde inte validera nyckeln: " + ex.Message };
        }
    }

    private static string ToUrl(byte[] data) =>
        Convert.ToBase64String(data).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    private static byte[] FromUrl(string value)
    {
        var padded = value.Replace('-', '+').Replace('_', '/');
        switch (padded.Length % 4)
        {
            case 2: padded += "=="; break;
            case 3: padded += "="; break;
        }

        return Convert.FromBase64String(padded);
    }
}
