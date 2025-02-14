using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MotionDrive.Desktop.Config;
public class ConfigModel
{
    public Executables Executables { get; set; } = new Executables();
    public string APIUrl { get; set; } = "https://motiondriveapiservice.azure-api.net/api/api";
    public string DisplayExePath { get; set; } = "../MotionDrive.DesktopApplication/bin/Debug/net8.0/MotionDrive.DisplayApplication.Exe";
}

public class Executables : INotifyPropertyChanged
{
    private string? _acPath;
    private string? _accPath;

    public string? ACPath
    {
        get { return _acPath; }
        set
        {
            this._acPath = value;
            OnPropertyChanged();
        }
    }
    public string? ACCPath
    {
        get { return _accPath; }
        set
        {
            this._accPath = value;
            OnPropertyChanged();
        }
    }


    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
