using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MotionDrive.Desktop.ViewModels.Profile;
using ReactiveUI;

namespace MotionDrive.Desktop;
public partial class ProfileViewManagerView : ReactiveUserControl<ProfileViewManager>
{
    public ProfileViewManagerView()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }
}