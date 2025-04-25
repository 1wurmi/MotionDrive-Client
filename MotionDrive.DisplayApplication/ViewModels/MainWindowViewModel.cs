using Avalonia.Controls;
using ReactiveUI;

namespace MotionDrive.DisplayApplication.ViewModels;
public partial class MainWindowViewModel : ReactiveObject
{
    private UserControl _currentView = new StandardDisplayView()
    {
        DataContext = new StandardDisplayViewModel()
    };

    public UserControl CurrentView
    {
        get => _currentView;
        set => this.RaiseAndSetIfChanged(ref _currentView, value);
    }

    public MainWindowViewModel()
    {
        CurrentView = new StandardDisplayView()
        {
            DataContext = new StandardDisplayViewModel()
        };


    }
}
