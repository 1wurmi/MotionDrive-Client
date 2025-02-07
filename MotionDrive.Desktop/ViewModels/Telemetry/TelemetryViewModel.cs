using Avalonia.Controls;
using Desktop.Models;
using ReactiveUI;
using System.Diagnostics;

namespace MotionDrive.Desktop.ViewModels.Telemetry;
public class TelemetryViewModel : ReactiveObject
{
    private UserControl _currentView;
    public UserControl CurrentView
    {
        get => _currentView;
        set => this.RaiseAndSetIfChanged(ref _currentView, value);
    }

    ChooseSessionView chooseSessionView;

    public TelemetryViewModel(IScreen screen)
    {
        this.chooseSessionView = new ChooseSessionView() { DataContext = new ChooseSessionViewModel(this) };

        this.CurrentView = chooseSessionView;
    }
    public void GoToTelemetry(string fileName)
    {
        this.CurrentView = new TelemetryChartsView { DataContext = new TelemetryChartsViewModel(DataLoader.LoadSessionFromFile(fileName), this) };
    }

    public void GoToChooseSession()
    {
        this.CurrentView = chooseSessionView;
    }
}
