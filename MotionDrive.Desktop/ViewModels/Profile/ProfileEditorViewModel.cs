using Desktop.ViewModels;
using MotionDrive.Desktop.Config;
using MotionDrive.Desktop.Models;
using MotionDrive.Desktop.SecrectsConfig;
using PSC.CSharp.Library.CountryData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace MotionDrive.Desktop.ViewModels.Profile;
public class ProfileEditorViewModel : ReactiveObject
{
    ProfileViewManager parent { get; }

    private ProfileEditModel _profile;
    public ProfileEditModel Profile
    {
        get => _profile;
        set => this.RaiseAndSetIfChanged(ref _profile, value);
    }

    ConfigManager configManager = new ConfigManager();
    SecretsManager secretsManager = new SecretsManager();
    ConfigModel loadedConfig;
    SecretConfigModel SecretConfigModel { get; }

    HttpClient client;

    IEnumerable<Country> Countries { get; }
    IScreen HostScreen { get; }

    private string _currentCountryImageURL;
    public string CurrentCountryImageURL
    {
        get => _currentCountryImageURL;
        set => this.RaiseAndSetIfChanged(ref _currentCountryImageURL, value);
    }

    public ProfileEditorViewModel(ProfileViewManager parent, IScreen HostScreen)
    {
        this.parent = parent;
        this.HostScreen = HostScreen;

        loadedConfig = configManager.LoadConfig();
        SecretConfigModel = secretsManager.LoadSecrets();

        client = new HttpClient() { BaseAddress = new Uri(loadedConfig.APIUrl) };
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SecretConfigModel.JWT);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        CountryHelper ch = new CountryHelper();

        this.Countries = ch.GetCountryData();

        this.GetProfileData();
    }

    public void GetProfileData()
    {
        HttpResponseMessage response = client.GetAsync($"api/profile/data").Result;

        Trace.WriteLine(response.Content.ReadAsStringAsync().Result);

        ProfileEditModel profile = JsonSerializer.Deserialize<ProfileEditModel>(response.Content.ReadAsStringAsync().Result);
        // TODO CASE IF PROFILE NOT FOUND ETC.
        this.Profile = profile;
    }

    public void Save()
    {
        HttpResponseMessage res = client.PostAsync("api/profile/data", new StringContent(JsonSerializer.Serialize(this.Profile), Encoding.UTF8, "application/json")).Result;
    }

    public void ResetPassword()
    {

    }

    public void LogOut()
    {
        // Secrets.dat löschen
        // auf LoginView wechseln

        secretsManager.DeleteSecrets();
        this.HostScreen.Router.Navigate.Execute(new LoginViewModel(this.HostScreen, false));
    }


    public void CountrySelectionChanged(Country selectedCountry)
    {
        this.CurrentCountryImageURL = "https://flagsapi.com/" + selectedCountry.CountryShortCode + "/flat/32.png";

        // Available Settings:
        // Edit Username
        // Edit Email
        // Edit Phone Number
        // Change password
        // Edit Profile Picture
        // Edit Profile Bio
        // Edit Country
        // Favourite Car
        // Favourite Track
        // Favourite Game
        // Favourite Car Class

    }


}
