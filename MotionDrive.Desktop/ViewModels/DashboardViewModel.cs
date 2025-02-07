using Avalonia.Media;
using MotionDrive.Desktop.Config;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;

namespace Desktop.ViewModels;
public class DashboardViewModel : ReactiveObject
{
    IScreen HostScreen { get; }

    ConfigManager configManager = new();

    public ObservableCollection<ShortCut> ShortCutList { get; set; }

    public DashboardViewModel(IScreen screen)
    {
        HostScreen = screen;
        this.LoadShortCuts();
    }

    public void LoadShortCuts()
    {
        Executables executables = configManager.LoadConfig().Executables;

        ShortCutList = new ObservableCollection<ShortCut>();

        if (executables.ACPath != null && executables.ACPath != "")
        {
            ShortCutList.Add(new ShortCut() { Name = "Assetto Corsa", Icon = "ac.png", Path = executables.ACPath, ProcessName = "acs" });
        }
        if (executables.ACCPath != null && executables.ACCPath != "")
        {
            ShortCutList.Add(new ShortCut() { Name = "Assetto Corsa Competizione", Icon = "acc.png", Path = executables.ACCPath, ProcessName = "acc" });
        }
        if (executables.IRacingPath != null && executables.IRacingPath != "")
        {
            // INFO: Might not work on all computers ... (not sure)
           ShortCutList.Add(new ShortCut() { Name = "iRacing", Icon = "iracing.png", Path = executables.IRacingPath, ProcessName = "iRacingSim64DX11" });
        }
    }
}

public class ShortCut : ReactiveObject
{
    public string Name { get; set; }
    public string Icon { get; set; }
    public string Path { get; set; }
    public string ProcessName { get; set; }
    private bool _isRunning = false;
    public bool IsRunning
    {
        get => this._isRunning;
        set => this.RaiseAndSetIfChanged(ref this._isRunning, value);
    }

    private Color _ellipseColor;
    public Color EllipseColor
    {
        get => this._ellipseColor;
        set => this.RaiseAndSetIfChanged(ref this._ellipseColor, value);
    }


    private Color _runningColor = Color.Parse("#10f500");
    private Color _notRunningColor = Color.Parse("#141414");

    Thread processCheckerThread;

    public ShortCut()
    {
        this.EllipseColor = this._runningColor;
        this.processCheckerThread = new Thread(new ThreadStart(this.CheckProcess));
        this.processCheckerThread.Start();
    }

    public void ExecuteExecutable()
    {
        Process.Start(Path);
    }

    private async void CheckProcess()
    {
        Trace.WriteLine(Process.GetProcesses());
        while (true)
        {
            var processes = Process.GetProcessesByName(this.ProcessName);
            this.IsRunning = processes.Any() ? true : false;
            this.EllipseColor = this.IsRunning ? this._runningColor : this._notRunningColor;
            Thread.Sleep(5000);
        }
    }

}