using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Desktop.ViewModels;
using Desktop.Views;
using Microsoft.Extensions.DependencyInjection;
using MotionDrive.Desktop;
using MotionDrive.Desktop.Converter;
using MotionDrive.Desktop.Services;
using MotionDrive.Desktop.ViewModels;
using Recorder;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Desktop;
public partial class App : Application
{
    Recorder.Recorder recorder = new Recorder.Recorder()
    {
        SaveDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MotionDrive")
    };

    Thread recordThread;

    public override void Initialize()
    {
        this.recordThread = new Thread(new ThreadStart(recorder.StartRecorder));
        this.recordThread.Start();

        AvaloniaXamlLoader.Load(this);

        Resources["OnlineStatusConverter"] = new OnlineStatusConverter();

    }

    public override async void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var vmContext = new SplashViewModel();
            SplashWindow splashWindow = new SplashWindow()
            {
                DataContext = vmContext
            };
            desktop.MainWindow = splashWindow;
            desktop.MainWindow.Show();

            await vmContext.DoWork();

            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel(vmContext.IsLoggedIn)
            };
            desktop.MainWindow.Closing += MainWindow_Closing;
            desktop.MainWindow.Show();
            splashWindow.Close();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void MainWindow_Closing(object? sender, Avalonia.Controls.WindowClosingEventArgs e)
    {
        e.Cancel = true;

        var window = (MainWindow)sender;
        window.Hide();

    }

    private void NativeMenuItem_Click(object? sender, System.EventArgs e)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow.Show();
        }
    }

    private void NativeMenuItem_Close(object? sender, System.EventArgs e)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            recorder.StopRecording(false);
            desktop.Shutdown();
        }
    }
}
