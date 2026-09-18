using System.IO;
using System.Reflection;
using System.Windows;
using CodeBrief.Contracts;
using CodeBrief.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace CodeBrief.Desktop.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private const string SampleCode =
        """
        using System;

        class Program
        {
            static string password = "admin123";

            static void Main()
            {
                var numbers = new[] { 10, 0, 5 };
                foreach (var n in numbers)
                {
                    Console.WriteLine(100 / n);
                }
            }
        }
        """;

    private readonly UiState _ui;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AnalyzeCommand))]
    [NotifyPropertyChangedFor(nameof(LineCount))]
    [NotifyPropertyChangedFor(nameof(CharacterCount))]
    private string _sourceCode = SampleCode;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(WindowTitle))]
    private string? _fileName;

    [ObservableProperty]
    private LanguageGuess _language = LanguageGuess.Unknown;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CopyReportCommand))]
    [NotifyCanExecuteChangedFor(nameof(CopyFixCommand))]
    [NotifyCanExecuteChangedFor(nameof(ExportReportCommand))]
    [NotifyCanExecuteChangedFor(nameof(ExportPdfCommand))]
    private AnalysisReport? _report;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AnalyzeCommand))]
    private bool _isBusy;

    [ObservableProperty]
    private string _status = "Klistra in kod eller öppna en fil. Ctrl+Enter analyserar.";

    [ObservableProperty]
    private string _licenseStatus = "";

    public MainViewModel(UiState ui)
    {
        _ui = ui;
        RefreshLanguage();
        RefreshLicenseStatus();
    }

    public string WindowTitle => string.IsNullOrWhiteSpace(FileName) ? "CodeBrief" : $"{FileName} — CodeBrief";

    public int LineCount => string.IsNullOrEmpty(SourceCode)
        ? 0
        : SourceCode.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n').Length;

    public int CharacterCount => SourceCode.Length;

    public string VersionLabel { get; } = "v" + (Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.0.0");

    partial void OnSourceCodeChanged(string value) => RefreshLanguage();

    partial void OnFileNameChanged(string? value) => RefreshLanguage();

    public void LoadFromPath(string path)
    {
        if (!File.Exists(path))
        {
            Status = "Filen finns inte.";
            return;
        }

        SourceCode = File.ReadAllText(path);
        FileName = Path.GetFileName(path);
        RememberFolder(path);
        Report = null;
        Status = $"Öppnade {FileName}.";
    }

    [RelayCommand]
    private void OpenFile()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Öppna källkod",
            Filter =
                "Kodfiler|*.cs;*.py;*.js;*.ts;*.tsx;*.java;*.cpp;*.h;*.hpp;*.go;*.rs;*.php;*.rb;*.sql;*.kt;*.swift;*.ps1;*.sh;*.html;*.css|" +
                "Alla filer|*.*"
        };

        if (!string.IsNullOrWhiteSpace(_ui.LastFolder) && Directory.Exists(_ui.LastFolder))
        {
            dialog.InitialDirectory = _ui.LastFolder;
        }

        if (dialog.ShowDialog() == true)
        {
            LoadFromPath(dialog.FileName);
        }
    }

    [RelayCommand]
    private void Clear()
    {
        SourceCode = "";
        FileName = null;
        Report = null;
        Status = "Arbetsytan är tom.";
    }

    [RelayCommand]
    private void OpenSettings()
    {
        var window = new SettingsWindow(_ui) { Owner = Application.Current.MainWindow };
        window.ShowDialog();
        RefreshLicenseStatus();
        Status = _ui.Provider == "Local" ? "Lokal analysmotor aktiv." : $"{_ui.Provider} är valt som analysmotor.";
    }

    [RelayCommand]
    private void OpenLicense()
    {
        var window = new LicenseWindow(_ui) { Owner = Application.Current.MainWindow };
        window.ShowDialog();
        RefreshLicenseStatus();
    }

    private bool CanAnalyze() => !IsBusy && !string.IsNullOrWhiteSpace(SourceCode);

    [RelayCommand(CanExecute = nameof(CanAnalyze))]
    private async Task Analyze(CancellationToken cancellationToken)
    {
        if (!LicenseGate.CanAnalyze(_ui, out var licenseMessage))
        {
            Status = licenseMessage;
            OpenLicense();
            return;
        }

        IsBusy = true;
        Status = "Analyserar…";
        try
        {
            var request = new AnalysisRequest
            {
                Source = SourceCode,
                FileName = FileName,
                ClientGuess = Language
            };

            var analyzer = AnalyzerFactory.Create(_ui);
            Report = await analyzer.AnalyzeAsync(request, cancellationToken).ConfigureAwait(true);
            LicenseGate.ConsumeTrialIfNeeded(_ui);
            AppSettings.Save(_ui);
            RefreshLicenseStatus();
            Status = $"Klar · {Report.Language.Name} · {Report.Analyzer}";
        }
        catch (OperationCanceledException)
        {
            Status = "Analysen avbröts.";
        }
        catch (Exception ex)
        {
            Status = "Analysen misslyckades: " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanCopyReport() => Report is not null;

    [RelayCommand(CanExecute = nameof(CanCopyReport))]
    private void CopyReport()
    {
        if (Report is null) return;
        Clipboard.SetText(ReportFormatter.ToMarkdown(Report));
        Status = "Rapporten kopierades som Markdown.";
    }

    private bool CanCopyFix() => !string.IsNullOrWhiteSpace(Report?.Fix.CorrectedCode);

    [RelayCommand(CanExecute = nameof(CanCopyFix))]
    private void CopyFix()
    {
        if (string.IsNullOrWhiteSpace(Report?.Fix.CorrectedCode)) return;
        Clipboard.SetText(Report.Fix.CorrectedCode);
        Status = "Rättad kod kopierades.";
    }

    [RelayCommand(CanExecute = nameof(CanCopyReport))]
    private void ExportReport()
    {
        if (Report is null) return;
        SaveOutput("Markdown|*.md|Text|*.txt", Path.GetFileNameWithoutExtension(FileName ?? "rapport") + "-codebrief.md",
            path => File.WriteAllText(path, ReportFormatter.ToMarkdown(Report)));
    }

    [RelayCommand(CanExecute = nameof(CanCopyReport))]
    private void ExportPdf()
    {
        if (Report is null) return;
        SaveOutput("PDF|*.pdf", Path.GetFileNameWithoutExtension(FileName ?? "rapport") + "-codebrief.pdf",
            path => PdfReportWriter.Write(Report, path));
    }

    private void SaveOutput(string filter, string fileName, Action<string> write)
    {
        var dialog = new SaveFileDialog
        {
            Title = "Exportera rapport",
            Filter = filter,
            FileName = fileName
        };

        if (!string.IsNullOrWhiteSpace(_ui.LastFolder) && Directory.Exists(_ui.LastFolder))
        {
            dialog.InitialDirectory = _ui.LastFolder;
        }

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        write(dialog.FileName);
        RememberFolder(dialog.FileName);
        Status = "Rapporten sparades: " + Path.GetFileName(dialog.FileName);
    }

    private void RememberFolder(string path)
    {
        var folder = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(folder))
        {
            _ui.LastFolder = folder;
        }
    }

    private void RefreshLanguage() => Language = LanguageDetector.Detect(SourceCode, FileName);

    private void RefreshLicenseStatus()
    {
        LicenseGate.CanAnalyze(_ui, out var message);
        LicenseStatus = message;
    }
}
