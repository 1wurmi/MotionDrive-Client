using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using CommandLine;
using MotionDrive.DisplayApplication.ViewModels;
using MotionDrive.DisplayApplication.Views;
using System.Diagnostics;
using System.Linq;

namespace MotionDrive.DisplayApplication;
internal class Options
{
    [Option('d', "display", Required = true, HelpText = "Display to use")]
    public int DisplayIndex { get; set; }
}


public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();

            var options = new Options();

            Options opts = Parser.Default.ParseArguments<Options>(desktop.Args).WithParsed<Options>(o => Trace.WriteLine("...")).Value;

            desktop.MainWindow = CreateWindowForMonitor(opts.DisplayIndex);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    private Window CreateWindowForMonitor(int monitorIndex)
    {
        var mainWindow = new MainWindow()
        {
            DataContext = new MainWindowViewModel()
        };

        // Bildschirminformationen abrufen
        var primaryScreens = mainWindow.Screens.Primary;
        if (primaryScreens != null)
        {
            var allScreens = mainWindow.Screens.All;

            if (monitorIndex >= 0 && monitorIndex < allScreens.Count)
            {
                var targetScreen = allScreens[monitorIndex];

                // Fenstergröße und Position basierend auf dem Zielbildschirm einstellen
                mainWindow.Position = new PixelPoint(
                    targetScreen.Bounds.X,
                    targetScreen.Bounds.Y
                );
                mainWindow.Width = 800;
                mainWindow.Height = 400;
            }
            else
            {
                // Standard: Falls der Monitor-Index nicht gültig ist
                mainWindow.Position = new PixelPoint(0, 0);
            }
        }

        return mainWindow;
    }
}