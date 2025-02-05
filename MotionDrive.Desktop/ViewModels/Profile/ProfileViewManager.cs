using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionDrive.Desktop.ViewModels.Profile;
public class ProfileViewManager : ReactiveObject
{
    IScreen HostScreen { get; }
    private UserControl _currentView;
    UserControl CurrentView
    {
        get => _currentView;
        set => this.RaiseAndSetIfChanged(ref _currentView, value);
    }

    public ProfileViewManager(IScreen screen)
    {
        HostScreen = screen;
        CurrentView = new ProfileView() { DataContext = new ProfileViewModel(this) };
    }

    public void GoToEditProfilePage()
    {
        CurrentView = new ProfileEditView() { DataContext = new ProfileEditorViewModel(this, HostScreen) };
    }
}