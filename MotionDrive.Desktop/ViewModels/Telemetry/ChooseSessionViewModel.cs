using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Microsoft.VisualBasic.FileIO;
using MotionDrive.Desktop.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace MotionDrive.Desktop.ViewModels.Telemetry;
public class ChooseSessionViewModel : ReactiveObject
{

    IScreen HostScreen { get; }

    private Window topLevel = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow;


    public ObservableCollection<TelemetryFile> _files = new ObservableCollection<TelemetryFile>();
    public ObservableCollection<TelemetryFile> Files
    {
        get => _files;
        set => this.RaiseAndSetIfChanged(ref _files, value);
    }

    private bool _canShowFiles = false;
    public bool CanShowFiles
    {
        get => _canShowFiles;
        set => this.RaiseAndSetIfChanged(ref _canShowFiles, value);
    }

    private ObservableCollection<FolderCardModel> _folderCards = new ObservableCollection<FolderCardModel>();
    public ObservableCollection<FolderCardModel> FolderCards
    {
        get => _folderCards;
        set => this.RaiseAndSetIfChanged(ref _folderCards, value);
    }

    private IStorageFile? _selectedFile = null;
    public IStorageFile? SelectedFile
    {
        get => _selectedFile;
        set => this.RaiseAndSetIfChanged(ref _selectedFile, value);
    }

    public ICommand GetTelemetryFilesCommand { get; }
    public ICommand GoToTelemetryCommand { get; }



    public ObservableCollection<string> TelemetryFiles = new ObservableCollection<string>();

    private TelemetryViewModel parentTelemetryViewModel;

    public ChooseSessionViewModel(TelemetryViewModel parentTelemetryViewModel)
    {
        this.parentTelemetryViewModel = parentTelemetryViewModel;

        FolderCards = GetFolders();

        GetTelemetryFilesCommand = ReactiveCommand.Create<FolderCardModel>(GetTelemetryFiles);
        GoToTelemetryCommand = ReactiveCommand.Create<string>(GoToTelemetry);
    }

    public ObservableCollection<FolderCardModel> GetFolders()
    {

        var folders = new ObservableCollection<FolderCardModel>();
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MotionDrive", "Telemetry");

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        foreach (string folder in Directory.GetDirectories(path))
        {
            folders.Add(new FolderCardModel { FolderName = folder.Split('\\').Last(), FolderPath = folder });
        }

        return folders;
    }


    public void GoToChooseFolderPage()
    {
        Files.Clear();
        CanShowFiles = false;
    }

    public void GetTelemetryFiles(FolderCardModel gameCard)
    {
        Files.Clear();

        string[] files = Directory.GetFiles(gameCard.FolderPath, "*.json");
        foreach (string file in files)
        {
            // read ADS (:meta.json)
            string metaFile = file + ":meta.json";

            if (System.IO.File.Exists(metaFile))
            {
                string json = System.IO.File.ReadAllText(metaFile);
                TelemetryFile telemetryFile = System.Text.Json.JsonSerializer.Deserialize<TelemetryFile>(json);
                telemetryFile.Path = file;
                Files.Add(telemetryFile);
            }
        }
        CanShowFiles = true;
    }

    public void GoToTelemetry(string path)
    {
        Console.WriteLine(path);
        parentTelemetryViewModel.GoToTelemetry(path);
    }
    public async void OpenFileDialog()
    {

        IStorageFolder? startFolder = await topLevel.StorageProvider.TryGetFolderFromPathAsync(SpecialDirectories.MyDocuments);

        IReadOnlyList<IStorageFile> file = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            AllowMultiple = false,
            SuggestedStartLocation = startFolder
        });

        if (file.Count == 0)
        {
            return;
        }

        SelectedFile = file[0];
    }

}
