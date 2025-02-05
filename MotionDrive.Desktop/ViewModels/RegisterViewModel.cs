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
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Desktop.ViewModels;
public class RegisterViewModel : ReactiveObject, IRoutableViewModel
{
    public IScreen HostScreen { get; }
    public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);



    SecretsManager secretsManager = new SecretsManager();
    ConfigManager configManager = new ConfigManager();
    ConfigModel loadedConfig;

    public RegisterViewModel(IScreen screen)
    {
        HostScreen = screen;

        this.loadedConfig = configManager.LoadConfig();
    }

    public string _email;
    public string _username;
    public string _password;
    public string _repeatPassword;

    public string Email
    {
        get => _email;
        set => this.RaiseAndSetIfChanged(ref _email, value);
    }

    public string Username
    {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
    }

    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public string RepeatPassword
    {
        get => _repeatPassword;
        set => this.RaiseAndSetIfChanged(ref _repeatPassword, value);
    }

    public void GoToLogin() => HostScreen.Router.Navigate.Execute(new LoginViewModel(HostScreen, false));

    public void Register()
    {
        using HttpClient http = new HttpClient();

        http.BaseAddress = new Uri(loadedConfig.APIUrl);
        HttpResponseMessage response = http.PostAsync("api/auth/register", new StringContent($"{{\"userName\":\"{Username}\",\"email\":\"{Email}\",\"password\":\"{Password}\",\"confirmPassword\":\"{RepeatPassword}\"}}", Encoding.UTF8, "application/json")).Result;
        Trace.WriteLine(response.Content.ReadAsStringAsync().Result);
        ResponseModel rm = JsonSerializer.Deserialize<ResponseModel>(response.Content.ReadAsStringAsync().Result);

        if (!rm.successFull)
        {
            string error = response.Content.ReadAsStringAsync().Result;
            return;
        }

        response = http.PostAsync("api/auth/login", new StringContent($"{{\"email\":\"{Email}\",\"password\":\"{Password}\"}}", Encoding.UTF8, "application/json")).Result;

        rm = JsonSerializer.Deserialize<ResponseModel>(response.Content.ReadAsStringAsync().Result);

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
    }
}