using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MotionDrive.Desktop.Views.Popup;
using ReactiveUI;

namespace MotionDrive.Desktop;
public partial class PopupView : ReactiveUserControl<PopupViewModel>
{
    public PopupView()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }
}