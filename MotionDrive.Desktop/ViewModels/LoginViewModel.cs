using Avalonia;
using Desktop.Models;
using MotionDrive.Desktop.Config;
using MotionDrive.Desktop.Models;
using MotionDrive.Desktop.SecrectsConfig;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Desktop.ViewModels;
public class LoginViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    public IScreen HostScreen { get; }
    public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);

    public void GoToRegister() => HostScreen.Router.Navigate.Execute(new RegisterViewModel(HostScreen));

    private string _username;
    public string Username
    {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
    }

    private string _password;
    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public ViewModelActivator Activator { get; }

    SecretsManager secretsManager = new SecretsManager();
    ConfigManager configManager = new ConfigManager();
    ConfigModel loadedConfig;

    public LoginViewModel(IScreen screen, bool isUserLoggedIn)
    {
        HostScreen = screen;
        loadedConfig = configManager.LoadConfig();

        this.Activator = new ViewModelActivator();
        this.WhenActivated((CompositeDisposable disposables) =>
        {
            if (isUserLoggedIn)
                HostScreen.Router.Navigate.Execute(new HomeViewModel(HostScreen));
        });
    }

    public void Login()
    {
        using HttpClient http = new HttpClient();

        http.BaseAddress = new Uri(loadedConfig.APIUrl);
        HttpResponseMessage response = http.PostAsync("api/auth/login", new StringContent($"{{\"email\":\"{Username}\",\"password\":\"{Password}\"}}", Encoding.UTF8, "application/json")).Result;

        ResponseModel rm = JsonSerializer.Deserialize<ResponseModel>(response.Content.ReadAsStringAsync().Result);

        if (rm.successFull)
        {
            secretsManager.SaveSecrets(new SecretConfigModel()
            {
                JWT = rm.jwt,
                RefreshToken = rm.refresh,
                ExpiresAt = rm.expiresAt
            });

            HostScreen.Router.Navigate.Execute(new HomeViewModel(HostScreen));
        }
        else
        {
            string error = response.Content.ReadAsStringAsync().Result;
            Trace.WriteLine(error);
        }
    }

    public void Refresh()
    {
        using HttpClient http = new HttpClient();

        SecretConfigModel secrets = secretsManager.LoadSecrets();

        http.BaseAddress = new Uri(loadedConfig.APIUrl);
        HttpResponseMessage response = http.PostAsync("api/auth/refresh", new StringContent($"{{\"RefreshToken\":\"{secrets.RefreshToken}\",\"AccessToken\":\"{secrets.JWT}\"}}", Encoding.UTF8, "application/json")).Result;

        ResponseModel rm = JsonSerializer.Deserialize<ResponseModel>(response.Content.ReadAsStringAsync().Result);

        if (rm.successFull)
        {
            secretsManager.SaveSecrets(new SecretConfigModel()
            {
                JWT = rm.jwt,
                RefreshToken = rm.refresh,
                ExpiresAt = rm.expiresAt
            });

            HostScreen.Router.Navigate.Execute(new HomeViewModel(HostScreen));
        }
        else
        {
            string error = response.Content.ReadAsStringAsync().Result;
            Trace.WriteLine(error);
        }
    }
}
