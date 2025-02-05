using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MotionDrive.Desktop.ViewModels.Telemetry;
using ReactiveUI;

namespace MotionDrive.Desktop;
public partial class ChooseSessionView : ReactiveUserControl<ChooseSessionViewModel>
{
    public ChooseSessionView()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }
}