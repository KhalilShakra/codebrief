# CodeBrief

Windows-app som tar källkod, identifierar språket och skriver en pedagogisk rapport på svenska.

Klistra in kod eller släpp en fil. CodeBrief gissar språket direkt, kör analys och visar sex sektioner: översikt, pseudokod, flöde, buggar/säkerhet, förbättringar och rättad kod.

![CodeBrief](src/CodeBrief.Desktop/Assets/logo.png)

## Funktioner

- **Språkdetektion** offline: C#, Python, C/C++, Java, JavaScript/TypeScript, Go, Rust med flera
- **Lokal analys** som alltid fungerar, även utan internet
- **AI-analys** via din egen OpenAI- eller Anthropic-nyckel (lagras krypterad med Windows DPAPI)
- **Syntaxfärgning** i editorn
- **Export** till Markdown och PDF
- **Provperiod** på 20 analyser, därefter RSA-signerad licensnyckel
- **Installer** för en fristående 64-bitars `.exe`

## Krav

- Windows 10/11 x64
- För utveckling: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Kör från källkod

```powershell
dotnet restore
dotnet test
dotnet run --project src/CodeBrief.Desktop
```

## Användning

1. Klistra in kod eller öppna en fil (`Ctrl+O`).
2. Tryck **Analysera** (`Ctrl+Enter`).
3. Läs rapporten till höger. Exportera med **Markdown** (`Ctrl+S`) eller **PDF** (`Ctrl+Shift+P`).
4. Under **Inställningar** kan du välja OpenAI eller Anthropic och klistra in en API-nyckel. Utan nyckel används den lokala motorn.
5. Under **Licens** aktiverar du en `CB1.`-nyckel när provperioden är slut.

API-nyckeln lämnar aldrig din dator utom till den provider du valt. CodeBrief skickar inte nyckeln till någon egen server.

## Bygg installer

```powershell
powershell -ExecutionPolicy Bypass -File scripts/publish.ps1
```

Resultatet:

- `publish/win-x64/CodeBrief.exe` — self-contained, ingen .NET-installation krävs
- `dist/CodeBrief-Setup-1.0.0.exe` — Inno Setup-installer (om Inno Setup 6 är installerat)

## Skapa licensnycklar

Den privata nyckeln ligger i `tools/license-private.pem` och ska **inte** checkas in.

```powershell
dotnet run --project tools/CodeBrief.LicenseGen -- issue --name "Kundnamn" --email kund@example.com --days 365
```

Kommandot skriver ut en nyckel som börjar med `CB1.`. Klistra in den i appen under Licens.

För att byta nyckelpar (produktion):

```powershell
dotnet run --project tools/CodeBrief.LicenseGen -- keys
```

Kopiera sedan `tools/license-public.pem` till `src/CodeBrief.Licensing/LicensePublicKey.cs`.

## Projektstruktur

```
src/CodeBrief.Desktop      WPF-klient
src/CodeBrief.Core         språkdetektor, lokal analys, AI-klienter
src/CodeBrief.Contracts    JSON-rapportmodell
src/CodeBrief.Licensing    RSA-licens
tools/CodeBrief.LicenseGen skapa nycklar
tests/CodeBrief.Tests      enhetstester
installer/                 Inno Setup
```

## Kortkommandon

| Shortcut | Åtgärd |
|---|---|
| `Ctrl+Enter` | Analysera |
| `Ctrl+O` | Öppna fil |
| `Ctrl+N` | Rensa |
| `Ctrl+S` | Exportera Markdown |
| `Ctrl+Shift+P` | Exportera PDF |
| `Ctrl+,` | Inställningar |

## Website / GitHub Pages

Marknadssajten ligger i `docs/` (statisk HTML, ingen backend).

1. Skapa ett GitHub-repo och pusha `main` (det finns inget remote i den här kopian ännu).
2. På GitHub: **Settings → Pages**.
3. Source: **Deploy from a branch**.
4. Branch `main`, folder **`/docs`**. Save.
5. Sajten publiceras på `https://<användarnamn>.github.io/<repo>/`.

Ladda ner-knappen pekar mot GitHub Releases. När du har en installer, skapa en release och lägg upp `dist/CodeBrief-Setup-1.0.0.exe`.

**Egen domän:** köp den själv (Loopia, Namecheap, …). Lägg inte in en påhittad adress. Steg för DNS (CNAME `www` → `<användarnamn>.github.io`, eller A-poster för apex) och GitHub “Custom domain” + HTTPS finns i [docs/DOMAIN.md](docs/DOMAIN.md). Ingen `CNAME`-fil ska ligga i `docs/` förrän du faktiskt har en domän.

## Licens för källkoden

MIT. QuestPDF används under Community License.
