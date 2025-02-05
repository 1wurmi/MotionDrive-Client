using Desktop.ViewModels;
using MotionDrive.Desktop.Config;
using MotionDrive.Desktop.Models;
using MotionDrive.Desktop.SecrectsConfig;
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

namespace MotionDrive.Desktop.ViewModels;
public class SplashViewModel : ReactiveObject
{
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

    public BackgroundWorker backgroundWorker = new BackgroundWorker();

    public SplashViewModel()
    {
        backgroundWorker.WorkerReportsProgress = true;
        backgroundWorker.DoWork += new DoWorkEventHandler(BackgroundWorker_DoWork);
        backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(BackgroundWorker_ProgressChanged);
    }

    private void BackgroundWorker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
    {
        this.ProgressValue = e.ProgressPercentage;
    }

    private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
    {
        BackgroundWorker worker = sender as BackgroundWorker;
        this.TryLogIn(worker);
    }


    public async void TryLogIn(BackgroundWorker worker)
    {
        this.StatusText = "TRYING TO LOG IN ...";
        Thread.Sleep(500); // Has to be, because server is not starting fast enough

        worker.ReportProgress(33);

        Thread.Sleep(500); // Has to be, because server is not starting fast enough

        using HttpClient http = new HttpClient();

        SecretConfigModel secrets = secretsManager.LoadSecrets();
        ConfigModel loadedConfig = configManager.LoadConfig();

        http.BaseAddress = new Uri(loadedConfig.APIUrl);
        HttpResponseMessage response = http.PostAsync("api/auth/refresh", new StringContent($"{{\"RefreshToken\":\"{secrets.RefreshToken}\",\"AccessToken\":\"{secrets.JWT}\"}}", Encoding.UTF8, "application/json")).Result;

        worker.ReportProgress(66);

        Thread.Sleep(500); // Has to be, because server is not starting fast enough

        ResponseModel rm = JsonSerializer.Deserialize<ResponseModel>(response.Content.ReadAsStringAsync().Result);

        if (rm.successFull)
        {
            worker.ReportProgress(100);

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
