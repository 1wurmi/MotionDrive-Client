using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Desktop.ViewModels;
using ReactiveUI;

namespace Desktop;
public partial class SettingsView : ReactiveUserControl<SettingsViewModel>
{
    public SettingsView()
    {

        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }
}