using Microsoft.VisualBasic.FileIO;
using MotionDrive.Desktop.Config;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop.ViewModels;
public class MotionSysViewModel : ReactiveObject
{
    IScreen HostScreen { get; }
    ConfigManager configManager = new ConfigManager();
    ConfigModel loadedConfig;

    Process displayProcess;

    public MotionSysViewModel(IScreen screen)
    {
        HostScreen = screen;
        loadedConfig = configManager.LoadConfig();
    }

    public void Start()
    {
        displayProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = loadedConfig.DisplayExePath,
                Arguments = "-d 0"
            }
        };

        displayProcess.Start();
    }

    public void Stop()
    {
        displayProcess.Kill();
    }
}
