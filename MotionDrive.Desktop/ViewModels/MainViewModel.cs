using Avalonia.Animation;
using MotionDrive.Desktop.Config;
using MotionDrive.Desktop.Models;
using MotionDrive.Desktop.SecrectsConfig;
using MotionDrive.Desktop.Services;
using ReactiveUI;
using System.Collections;

namespace Desktop.ViewModels;
public class MainViewModel : ReactiveObject, IScreen
{
    public RoutingState Router { get; } = new();

    public MainViewModel(bool isUserLoggedIn)
    {
        Router.Navigate.Execute(new LoginViewModel(this, isUserLoggedIn));
    }

}

