using CodeBrief.Licensing;

namespace CodeBrief.Desktop;

internal static class LicenseGate
{
    public static LicenseValidation Current(UiState ui) =>
        LicenseCrypt.Verify(ui.LicenseKey, LicensePublicKey.Pem);

    public static bool CanAnalyze(UiState ui, out string message)
    {
        var license = Current(ui);
        if (license.IsValid)
        {
            message = $"Pro · {license.Payload?.Name}";
            return true;
        }

        var left = Math.Max(0, LicenseCrypt.TrialAnalyses - ui.TrialUsed);
        if (left > 0)
        {
            message = $"Provperiod · {left} analyser kvar";
            return true;
        }

        message = "Provperioden är slut. Aktivera en licens under Licens.";
        return false;
    }

    public static void ConsumeTrialIfNeeded(UiState ui)
    {
        if (!Current(ui).IsValid)
        {
            ui.TrialUsed++;
        }
    }
}
