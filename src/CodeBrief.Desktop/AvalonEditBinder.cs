using System.Windows;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;

namespace CodeBrief.Desktop;

public static class AvalonEditBinder
{
    public static readonly DependencyProperty BoundTextProperty = DependencyProperty.RegisterAttached(
        "BoundText",
        typeof(string),
        typeof(AvalonEditBinder),
        new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBoundTextChanged));

    public static readonly DependencyProperty SyntaxProperty = DependencyProperty.RegisterAttached(
        "Syntax",
        typeof(string),
        typeof(AvalonEditBinder),
        new PropertyMetadata(null, OnSyntaxChanged));

    public static string GetBoundText(DependencyObject obj) => (string)obj.GetValue(BoundTextProperty);

    public static void SetBoundText(DependencyObject obj, string value) => obj.SetValue(BoundTextProperty, value);

    public static string? GetSyntax(DependencyObject obj) => (string?)obj.GetValue(SyntaxProperty);

    public static void SetSyntax(DependencyObject obj, string? value) => obj.SetValue(SyntaxProperty, value);

    private static void OnBoundTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextEditor editor)
        {
            return;
        }

        var incoming = e.NewValue as string ?? "";
        if (editor.Text == incoming)
        {
            return;
        }

        editor.Text = incoming;
        editor.TextChanged -= EditorOnTextChanged;
        editor.TextChanged += EditorOnTextChanged;
    }

    private static void EditorOnTextChanged(object? sender, EventArgs e)
    {
        if (sender is TextEditor editor)
        {
            SetBoundText(editor, editor.Text);
        }
    }

    private static void OnSyntaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextEditor editor)
        {
            return;
        }

        var name = Map(e.NewValue as string);
        editor.SyntaxHighlighting = name is null ? null : HighlightingManager.Instance.GetDefinition(name);
    }

    private static string? Map(string? languageId) => languageId switch
    {
        "csharp" => "C#",
        "python" => "Python",
        "java" => "Java",
        "javascript" or "typescript" => "JavaScript",
        "cpp" or "c" => "C++",
        "html" => "HTML",
        "css" => "CSS",
        "xml" => "XML",
        "php" => "PHP",
        "sql" => "TSQL",
        "powershell" => "PowerShell",
        "ruby" => "Ruby",
        _ => null
    };
}
