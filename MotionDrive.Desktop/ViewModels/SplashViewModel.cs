using Avalonia.Threading;
using Chaos.NaCl;
using Desktop.ViewModels;
using MotionDrive.Desktop.Config;
using MotionDrive.Desktop.Models;
using MotionDrive.Desktop.SecrectsConfig;
using MotionDrive.Desktop.Services;
using NetSparkleUpdater;
using NetSparkleUpdater.Enums;
using NetSparkleUpdater.SignatureVerifiers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace MotionDrive.Desktop.ViewModels;
public class SplashViewModel : ReactiveObject
{
    private static readonly string updateURL = "/update/appcast";
    public bool IsLoggedIn { get; set; } = false;

    public string _statusText;
    public string StatusText
    {
        get => _statusText;
        set => this.RaiseAndSetIfChanged(ref _statusText, value);
    }
    private int _progressValue = 0;
    public int ProgressValue
    {
        get => _progressValue;
        set => this.RaiseAndSetIfChanged(ref _progressValue, value);
    }

    private SecretsManager secretsManager = new SecretsManager();
    private ConfigManager configManager = new ConfigManager();


    public SplashViewModel()
    {

    }

    private void SetProgessValue(int value)
    {
        this.ProgressValue = value;
    }

    public async Task DoWork()
    {
        var updateCheck = await CheckForUpdates();

        // Erst Updates prüfen
        if (!updateCheck)
        {
            // Falls kein Update, weiter mit Login
            TryLogIn();


            // TODO GET FRIENDS IF LOGIN WAS SUCESSFULL
            await FriendsService.Instance.InitializeAsync();
        }
    }

    public async Task<bool> CheckForUpdates()
    {

#if DEBUG
        this.StatusText = "DEVELOPMENT MODE - SKIPPING UPDATE CHECK";
        Thread.Sleep(1000);
        this.SetProgessValue(10);
        return false; // Skip updates in development
#endif

        this.StatusText = "CHECKING FOR UPDATES ...";
        Thread.Sleep(100);
        this.SetProgessValue(10);

        var sparkle = new SparkleUpdater(this.configManager.LoadConfig().APIUrl + updateURL, new Ed25519Checker(SecurityMode.Unsafe))
        {
            UIFactory = new NetSparkleUpdater.UI.Avalonia.UIFactory(),
            RelaunchAfterUpdate = true,
            LogWriter = new LogWriter(),
            UserInteractionMode = UserInteractionMode.DownloadAndInstall,
        };

        // Update-Status abrufen
        var updateInfo = await sparkle.CheckForUpdatesQuietly();

        if (updateInfo.Status == UpdateStatus.UpdateAvailable)
        {
            this.StatusText = "UPDATE FOUND. INSTALLING...";
            this.SetProgessValue(20);
            Thread.Sleep(500);

            // Update-Installation starten
            await sparkle.InitAndBeginDownload(updateInfo.Updates.First());
            //await sparkle.InstallUpdate(updateInfo.Updates.First());

            Environment.Exit(0);
            return true; // Update wurde ausgeführt
        }

        this.StatusText = "NO UPDATE FOUND. CONTINUING...";
        this.SetProgessValue(30);
        return false; // Kein Update -> weiter mit Login
    }

    public void TryLogIn()
    {
        this.StatusText = "TRYING TO LOG IN ...";
        Thread.Sleep(500); // Has to be, because server is not starting fast enough

        this.SetProgessValue(33);

        Thread.Sleep(500); // Has to be, because server is not starting fast enough

        using HttpClient http = new HttpClient();

        SecretConfigModel secrets = secretsManager.LoadSecrets();
        ConfigModel loadedConfig = configManager.LoadConfig();

        http.BaseAddress = new Uri(loadedConfig.APIUrl);
        HttpResponseMessage response = http.PostAsync("api/auth/refresh", new StringContent($"{{\"RefreshToken\":\"{secrets.RefreshToken}\",\"AccessToken\":\"{secrets.JWT}\"}}", Encoding.UTF8, "application/json")).Result;

        this.SetProgessValue(66);

        Thread.Sleep(500); // Has to be, because server is not starting fast enough

        ResponseModel rm = JsonSerializer.Deserialize<ResponseModel>(response.Content.ReadAsStringAsync().Result);

        if (rm.successFull)
        {
            this.SetProgessValue(100);

            secretsManager.SaveSecrets(new SecretConfigModel()
            {
                JWT = rm.jwt,
                RefreshToken = rm.refresh,
                ExpiresAt = rm.expiresAt
            });
            this.IsLoggedIn = true;

        }
        else
        {
            string error = response.Content.ReadAsStringAsync().Result;
            Trace.WriteLine(error);
            this.IsLoggedIn = false;
        }

    }
}
