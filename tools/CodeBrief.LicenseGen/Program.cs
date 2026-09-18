using CodeBrief.Licensing;

var command = args.Length > 0 ? args[0] : "help";
var keysDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
Directory.CreateDirectory(keysDir);
var privatePath = Path.Combine(keysDir, "license-private.pem");
var publicPath = Path.Combine(keysDir, "license-public.pem");

switch (command)
{
    case "keys":
        var pair = LicenseCrypt.CreateKeyPair();
        await File.WriteAllTextAsync(privatePath, pair.PrivatePem);
        await File.WriteAllTextAsync(publicPath, pair.PublicPem);
        Console.WriteLine("Nyckelpar skapat:");
        Console.WriteLine(privatePath);
        Console.WriteLine(publicPath);
        Console.WriteLine("Klistra in public PEM i LicensePublicKey.cs och håll private PEM hemlig.");
        break;

    case "issue":
        if (!File.Exists(privatePath))
        {
            Console.Error.WriteLine("Saknar license-private.pem. Kör först: keys");
            return 1;
        }

        var name = Get(args, "--name") ?? "CodeBrief-användare";
        var email = Get(args, "--email") ?? "";
        var days = int.TryParse(Get(args, "--days"), out var d) ? d : 365;
        var payload = new LicensePayload
        {
            Name = name,
            Email = email,
            Edition = "pro",
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(days)
        };
        var key = LicenseCrypt.Issue(payload, await File.ReadAllTextAsync(privatePath));
        Console.WriteLine(key);
        break;

    default:
        Console.WriteLine("CodeBrief.LicenseGen");
        Console.WriteLine("  keys");
        Console.WriteLine("  issue --name Namn --email mail --days 365");
        break;
}

return 0;

static string? Get(string[] args, string name)
{
    var i = Array.IndexOf(args, name);
    return i >= 0 && i + 1 < args.Length ? args[i + 1] : null;
}
