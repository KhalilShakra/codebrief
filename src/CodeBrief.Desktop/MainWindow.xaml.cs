using System.Windows;
using CodeBrief.Core;
using CodeBrief.Desktop.ViewModels;

namespace CodeBrief.Desktop;

public partial class MainWindow : Window
{
    private readonly UiState _ui;

    public MainWindow()
    {
        _ui = AppSettings.Load();
        InitializeComponent();
        DataContext = new MainViewModel(_ui);
        ApplySavedPlacement();
        Closing += (_, _) => PersistWindowState();
    }

    private void ApplySavedPlacement()
    {
        if (!double.IsNaN(_ui.Left) &&
            !double.IsNaN(_ui.Top) &&
            IsOnScreen(_ui.Left, _ui.Top, _ui.Width, _ui.Height))
        {
            WindowStartupLocation = WindowStartupLocation.Manual;
            Left = _ui.Left;
            Top = _ui.Top;
            Width = Math.Max(MinWidth, _ui.Width);
            Height = Math.Max(MinHeight, _ui.Height);
        }

        if (_ui.Maximized)
        {
            WindowState = WindowState.Maximized;
        }
    }

    private void PersistWindowState()
    {
        if (WindowState == WindowState.Maximized)
        {
            _ui.Maximized = true;
        }
        else if (WindowState == WindowState.Normal)
        {
            _ui.Maximized = false;
            _ui.Left = Left;
            _ui.Top = Top;
            _ui.Width = Width;
            _ui.Height = Height;
        }

        AppSettings.Save(_ui);
    }

    private static bool IsOnScreen(double left, double top, double width, double height)
    {
        var bounds = new Rect(left, top, Math.Max(width, 100), Math.Max(height, 100));
        return bounds.IntersectsWith(new Rect(
            SystemParameters.VirtualScreenLeft,
            SystemParameters.VirtualScreenTop,
            SystemParameters.VirtualScreenWidth,
            SystemParameters.VirtualScreenHeight));
    }

    private void OnFileDragOver(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    private void OnFileDrop(object sender, DragEventArgs e)
    {
        if (DataContext is not MainViewModel viewModel)
        {
            return;
        }

        if (e.Data.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0)
        {
            viewModel.LoadFromPath(files[0]);
        }
    }
}
