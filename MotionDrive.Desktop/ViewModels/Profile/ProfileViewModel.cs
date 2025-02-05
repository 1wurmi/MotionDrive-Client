using MotionDrive.Desktop.Config;
using MotionDrive.Desktop.Models;
using MotionDrive.Desktop.SecrectsConfig;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MotionDrive.Desktop.ViewModels.Profile;
public class ProfileViewModel : ReactiveObject
{
    ProfileViewManager parent { get; }
    ConfigManager configManager = new ConfigManager();
    SecretsManager secretsManager = new SecretsManager();
    ConfigModel loadedConfig;
    SecretConfigModel SecretConfigModel { get; }

    HttpClient client;

    private ProfileModel _profile;
    public ProfileModel Profile
    {
        get => _profile;
        set => this.RaiseAndSetIfChanged(ref _profile, value);
    }

    public ProfileViewModel(ProfileViewManager parent, string profileName = "")
    {
        this.parent = parent;
        loadedConfig = configManager.LoadConfig();
        SecretConfigModel = secretsManager.LoadSecrets();

        client = new HttpClient() { BaseAddress = new Uri(loadedConfig.APIUrl) };
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SecretConfigModel.JWT);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        this.GetProfile(profileName);

    }

    private void GetProfile(string profileName)
    {
        HttpResponseMessage response = client.GetAsync($"api/profile/{profileName}").Result;

        Trace.WriteLine(response.Content.ReadAsStringAsync().Result);

        ProfileModel profile = JsonSerializer.Deserialize<ProfileModel>(response.Content.ReadAsStringAsync().Result);

        // TODO CASE IF PROFILE NOT FOUND ETC.
        this.Profile = profile;
    }

    public void EditProfile()
    {
        parent.GoToEditProfilePage();
    }
}