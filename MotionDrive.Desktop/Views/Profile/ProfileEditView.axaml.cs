using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MotionDrive.Desktop.ViewModels.Profile;
using PSC.CSharp.Library.CountryData;
using ReactiveUI;
using System.Text.RegularExpressions;

namespace MotionDrive.Desktop;
public partial class ProfileEditView : ReactiveUserControl<ProfileEditorViewModel>
{
    public ProfileEditView()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);

        InitializeComponent();
    }

    private void TextBox_TextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            // Remove all non-numeric characters except the leading '+'
            string digits = Regex.Replace(textBox.Text, @"[^\d+]", "");

            // Ensure the first character is '+'
            if (!digits.StartsWith("+"))
            {
                digits = "+" + digits.TrimStart('+');
            }

            // Apply formatting
            if (digits.Length <= 3)
            {
                textBox.Text = digits;
            }
            else if (digits.Length <= 7)
            {
                textBox.Text = $"{digits.Substring(0, 3)} {digits.Substring(3)}";
            }
            else
            {
                textBox.Text = $"{digits.Substring(0, 3)} {digits.Substring(3, 3)} {digits.Substring(6)}";
            }

            // Move the caret to the end of the text
            textBox.CaretIndex = textBox.Text.Length;
        }
    }

    private void AboutMeTextBox_TextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {

            // Limit the number of characters
            if (textBox.Text.Length > 150)
            {
                textBox.Text = textBox.Text.Substring(0, 150);
                textBox.CaretIndex = textBox.Text.Length;
            }

            // Update the character count
            CharCounter.Text = $"{textBox.Text.Length}/150";
        }
    }

    private void ComboBox_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        var comboBox = sender as ComboBox;
        ViewModel.CountrySelectionChanged(comboBox.SelectedValue as Country);

    }
}