using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using MotionDrive.Desktop;
using MotionDrive.Desktop.Config;
using MotionDrive.Desktop.ViewModels;
using MotionDrive.Desktop.ViewModels.Profile;
using MotionDrive.Desktop.ViewModels.Telemetry;
using MotionDrive.Desktop.Views.Popup;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia.Models;
using ReactiveUI;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Desktop.ViewModels;
public class HomeViewModel : ReactiveObject, IRoutableViewModel
{
    public IScreen HostScreen { get; }
    public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);

    private object _currentView;
    public object CurrentView
    {
        get => _currentView;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentView, value);
        }
    }

    private bool _isPaneOpen = false;

    public bool IsPaneOpen
    {
        get => _isPaneOpen;
        set => this.RaiseAndSetIfChanged(ref _isPaneOpen, value);
    }

    private bool _showSaveReminderPopup = true;
    public bool ShowSaveReminderPopup
    {
        get => _showSaveReminderPopup;
        set => this.RaiseAndSetIfChanged(ref _showSaveReminderPopup, value);
    }

    private object _saveReminderPopup = new ContentControl();
    public object SaveReminderPopup
    {
        get => _saveReminderPopup;
        set => this.RaiseAndSetIfChanged(ref _saveReminderPopup, value);
    }

    private Geometry _openPaneIcon = Geometry.Parse("M6 8a.5.5 0 0 0 .5.5h5.793l-2.147 2.146a.5.5 0 0 0 .708.708l3-3a.5.5 0 0 0 0-.708l-3-3a.5.5 0 0 0-.708.708L12.293 7.5H6.5A.5.5 0 0 0 6 8m-2.5 7a.5.5 0 0 1-.5-.5v-13a.5.5 0 0 1 1 0v13a.5.5 0 0 1-.5.5");
    private Geometry _closePaneIcon = Geometry.Parse("M12.5 15a.5.5 0 0 1-.5-.5v-13a.5.5 0 0 1 1 0v13a.5.5 0 0 1-.5.5M10 8a.5.5 0 0 1-.5.5H3.707l2.147 2.146a.5.5 0 0 1-.708.708l-3-3a.5.5 0 0 1 0-.708l3-3a.5.5 0 1 1 .708.708L3.707 7.5H9.5a.5.5 0 0 1 .5.5");

    private Geometry _paneIcon;

    public Geometry PaneIcon
    {
        get => _paneIcon;
        set => this.RaiseAndSetIfChanged(ref _paneIcon, value);
    }

    private SolidColorBrush _activeColor = new SolidColorBrush(Color.Parse("#750E21"));
    private SolidColorBrush _defaultColor = new SolidColorBrush(Color.Parse("#333333"));

    private SolidColorBrush? _dashboardBackground;
    private SolidColorBrush? _telemetryBackground;
    private SolidColorBrush? _friendsBackground;
    private SolidColorBrush? _teamsBackground;
    private SolidColorBrush? _setupsBackground;
    private SolidColorBrush? _strategyBackground;
    private SolidColorBrush? _motionSysBackground;
    private SolidColorBrush? _steeringBackground;
    private SolidColorBrush? _settingsBackground;
    private SolidColorBrush? _profileBackground;

    public SolidColorBrush DashboardBackground { get => _dashboardBackground!; set => this.RaiseAndSetIfChanged(ref _dashboardBackground, value); }
    public SolidColorBrush TelemetryBackground { get => _telemetryBackground!; set => this.RaiseAndSetIfChanged(ref _telemetryBackground, value); }
    public SolidColorBrush FriendsBackground { get => _friendsBackground!; set => this.RaiseAndSetIfChanged(ref _friendsBackground, value); }
    public SolidColorBrush TeamsBackground { get => _teamsBackground!; set => this.RaiseAndSetIfChanged(ref _teamsBackground, value); }
    public SolidColorBrush SetupsBackground { get => _setupsBackground!; set => this.RaiseAndSetIfChanged(ref _setupsBackground, value); }
    public SolidColorBrush StrategyBackground { get => _strategyBackground!; set => this.RaiseAndSetIfChanged(ref _strategyBackground, value); }
    public SolidColorBrush MotionSysBackground { get => _motionSysBackground!; set => this.RaiseAndSetIfChanged(ref _motionSysBackground, value); }
    public SolidColorBrush SteeringBackground { get => _steeringBackground!; set => this.RaiseAndSetIfChanged(ref _steeringBackground, value); }
    public SolidColorBrush SettingsBackground { get => _settingsBackground!; set => this.RaiseAndSetIfChanged(ref _settingsBackground, value); }
    public SolidColorBrush ProfileBackground { get => _profileBackground!; set => this.RaiseAndSetIfChanged(ref _profileBackground, value); }

    public HomeViewModel(IScreen screen)
    {
        HostScreen = screen;

        _currentView = new DashboardView() { DataContext = new DashboardViewModel(HostScreen) };
        _paneIcon = _openPaneIcon;

        this.SetDefaultBackgrounds();
    }


    public void TogglePane()
    {
        this.IsPaneOpen = !this.IsPaneOpen;

        if (this.IsPaneOpen)
        {
            this.PaneIcon = _closePaneIcon;
        }
        else
        {
            this.PaneIcon = _openPaneIcon;
        }
    }

    private void SetDefaultBackgrounds()
    {
        DashboardBackground = this._defaultColor;
        TelemetryBackground = this._defaultColor;
        FriendsBackground = this._defaultColor;
        TeamsBackground = this._defaultColor;
        SetupsBackground = this._defaultColor;
        StrategyBackground = this._defaultColor;
        MotionSysBackground = this._defaultColor;
        SteeringBackground = this._defaultColor;
        SettingsBackground = this._defaultColor;
        ProfileBackground = this._defaultColor;
    }
    public async Task<bool> CheckIfCanNavigate()
    {
        bool canNavigate = true;

        if (CurrentView is SettingsView)
        {
            if (((SettingsView)CurrentView).ViewModel.HasChanges())
            {

                var tcs = new TaskCompletionSource<bool>();

                this.SaveReminderPopup = new PopupView
                {
                    DataContext = new PopupViewModel("Unsaved Changes!",
                    "Do you want to save your changes?",
                    new ObservableCollection<PopupButton>() {
                        new PopupButton("Yes", new SolidColorBrush(Color.Parse("#750E21")), () =>
                        {
                            Trace.WriteLine("Yes");
                            ((SettingsView)CurrentView).ViewModel.Save();

                            this.SaveReminderPopup = new ContentControl();
                            tcs.SetResult(true);
                        }),
                        new PopupButton("No",new SolidColorBrush(Color.Parse("#333333")), () =>
                        {
                            this.SaveReminderPopup = new ContentControl();
                            tcs.SetResult(true);
                        })
                    },
                    onClosePopup: () =>
                    {
                        this.SaveReminderPopup = new ContentControl();
                        tcs.SetResult(false);
                    }
                    )
                };

                await tcs.Task;
                canNavigate = tcs.Task.Result;
            }
        }

        return canNavigate;
    }

    public async Task NavigateToDashboard()
    {
        if (!await this.CheckIfCanNavigate())
            return;
        CurrentView = new DashboardView() { DataContext = new DashboardViewModel(HostScreen) };
        SetDefaultBackgrounds();
        DashboardBackground = this._activeColor;
    }

    public async Task NavigateToTelemetry()
    {
        if (!await this.CheckIfCanNavigate())
            return;
        CurrentView = new TelemtryView() { DataContext = new TelemetryViewModel(HostScreen) };
        SetDefaultBackgrounds();
        TelemetryBackground = this._activeColor;
    }

    public async Task NavigateToFriends()
    {
        if (!await this.CheckIfCanNavigate())
            return;
        CurrentView = new FriendsView() { DataContext = new FriendsViewModel(HostScreen) };
        SetDefaultBackgrounds();
        FriendsBackground = this._activeColor;
    }

    public async Task NavigateToTeams()
    {
        if (!await this.CheckIfCanNavigate())
            return;
        CurrentView = new TeamsView() { DataContext = new TeamsViewModel(HostScreen) };
        SetDefaultBackgrounds();
        TeamsBackground = this._activeColor;
    }

    public async Task NavigateToSetups()
    {
        if (!await this.CheckIfCanNavigate())
            return;
        CurrentView = new SetupsView() { DataContext = new SetupsViewModel(HostScreen) };
        SetDefaultBackgrounds();
        SetupsBackground = this._activeColor;
    }

    public async Task NavigateToStrategy()
    {
        if (!await this.CheckIfCanNavigate())
            return;
        CurrentView = new StrategyView() { DataContext = new StrategyViewModel(HostScreen) };
        SetDefaultBackgrounds();
        StrategyBackground = this._activeColor;
    }

    public async Task NavigateToMotionSys()
    {
        if (!await this.CheckIfCanNavigate())
            return;
        CurrentView = new MotionSysView() { DataContext = new MotionSysViewModel(HostScreen) };
        SetDefaultBackgrounds();
        MotionSysBackground = this._activeColor;
    }

    public async Task NavigateToSteeringSys()
    {
        if (!await this.CheckIfCanNavigate())
            return;
        CurrentView = new SteeringSysView() { DataContext = new SteeringSysViewModel(HostScreen) };
        SetDefaultBackgrounds();
        SteeringBackground = this._activeColor;
    }

    public async Task NavigateToSettings()
    {
        if (!await this.CheckIfCanNavigate())
            return;
        CurrentView = new SettingsView() { DataContext = new SettingsViewModel(HostScreen) };
        SetDefaultBackgrounds();
        SettingsBackground = this._activeColor;
    }

    public async Task NavigateToProfile()
    {
        if (!await this.CheckIfCanNavigate())
            return;
        CurrentView = new ProfileViewManagerView() { DataContext = new ProfileViewManager(HostScreen) };
        SetDefaultBackgrounds();
        ProfileBackground = this._activeColor;
    }
}
