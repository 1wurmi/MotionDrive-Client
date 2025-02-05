using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Desktop.ViewModels;
using ReactiveUI;

namespace Desktop;
public partial class FriendsView : ReactiveUserControl<FriendsViewModel>
{
    public FriendsView()
    {

        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }
}