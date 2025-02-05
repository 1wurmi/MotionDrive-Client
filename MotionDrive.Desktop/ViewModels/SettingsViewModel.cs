using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Microsoft.VisualBasic.FileIO;
using MotionDrive.Desktop.Config;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Desktop.ViewModels;
public class SettingsViewModel : ReactiveObject
{
    private Window topLevel = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow;
    IScreen HostScreen { get; }
    ConfigManager configManager = new();
    private string OldConfigAsString;

    private ConfigModel _loadedConfig;
    public ConfigModel LoadedConfig
    {
        get => _loadedConfig;
        set => this.RaiseAndSetIfChanged(ref _loadedConfig, value);
    }

    public SettingsViewModel(IScreen screen)
    {
        HostScreen = screen;

        this.LoadedConfig = this.configManager.LoadConfig();
        this.configManager.SaveConfig(this.LoadedConfig);
        this.OldConfigAsString = this.configManager.GetConfigAsString();
    }

    public bool HasChanges()
    {
        return JsonSerializer.Serialize<ConfigModel>(this.LoadedConfig, new JsonSerializerOptions { WriteIndented = true }).ToString() != this.OldConfigAsString;
    }

    public async Task<string?> OpenFileExplorer(bool exeOnly = false)
    {
        IStorageFolder? startFolder = await topLevel.StorageProvider.TryGetFolderFromPathAsync(SpecialDirectories.MyDocuments);

        IReadOnlyList<IStorageFile> file = await topLevel.StorageProvider.OpenFilePickerAsync(new Avalonia.Platform.Storage.FilePickerOpenOptions
        {
            AllowMultiple = false,
            SuggestedStartLocation = startFolder,
            FileTypeFilter = exeOnly ? new List<FilePickerFileType> { new FilePickerFileType(".exe") { Patterns = new[] { "*.exe" } } } : null
        });

        if (file.Count == 0)
        {
            return null;
        }

        return file[0].Path.AbsolutePath.ToString();
    }

    public async void SelectACGamePathCommand()
    {
        this.LoadedConfig.Executables.ACPath = await OpenFileExplorer(true);
    }

    public async void SelectACCGamePathCommand()
    {
        this.LoadedConfig.Executables.ACCPath = await OpenFileExplorer(true);
    }

    public async void SelectDisplayExePathCommand()
    {
        this.LoadedConfig.DisplayExePath = await OpenFileExplorer(true);
    }

    public void Save()
    {
        this.configManager.SaveConfig(this.LoadedConfig);
        this.OldConfigAsString = this.configManager.GetConfigAsString();
    }

    public async void CopyConfigToClipboard()
    {
        await topLevel.Clipboard.SetTextAsync(this.configManager.GetConfigAsString());
    }
}
