using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Desktop.ViewModels;
using ReactiveUI;

namespace Desktop;
public partial class SetupsView : ReactiveUserControl<SetupsViewModel>
{
    public SetupsView()
    {

        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }
}