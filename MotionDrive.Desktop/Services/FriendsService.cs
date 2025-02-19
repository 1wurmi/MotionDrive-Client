using Microsoft.AspNetCore.SignalR.Client;
using MotionDrive.Desktop.Config;
using MotionDrive.Desktop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace MotionDrive.Desktop.Services;
public class FriendsService
{
    private static FriendsService? _instance;
    private ConfigManager cm = new ConfigManager();
    private ConfigModel loadedConfig;
    public static FriendsService Instance => _instance ??= new FriendsService();
    
    public ObservableCollection<Friend> Friends = new ObservableCollection<Friend>();

    private HubConnection _hubConnection;

    private FriendsService()
    {
        loadedConfig = cm.LoadConfig();
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(loadedConfig.APIUrl + "/friendshub")
            .WithAutomaticReconnect()
            .Build();
    }

    public async Task InitializeAsync()
    {
        await FetchFriendsAsync();  // Load friends from the API
        await _hubConnection.StartAsync();  // Connect to the SignalR hub
    }

    private async Task FetchFriendsAsync()
    {
        using (var client = new System.Net.Http.HttpClient())
        {
            var response = await client.GetStringAsync(loadedConfig.APIUrl + "api/friends");
            var friendsList = System.Text.Json.JsonSerializer.Deserialize<List<Friend>>(response);

            Friends.Clear();
            foreach (var friend in friendsList)
            {
                Friends.Add(friend);  // Populate the friends list
            }
        }
    }

    // Add a friend (can be triggered from the UI or other services)
    public async Task AddFriendAsync(string friendCode)
    {
        using (var client = new System.Net.Http.HttpClient())
        {
            var response = await client.PostAsJsonAsync(loadedConfig.APIUrl  + "/api/friends", friendCode);
            if (response.IsSuccessStatusCode)
            {
                await FetchFriendsAsync();
            }
        }
    }

    // Remove a friend (can be triggered from the UI or other services)
    public async Task RemoveFriendAsync(string friendId)
    {
        using (var client = new System.Net.Http.HttpClient())
        {
            var response = await client.DeleteAsync($"https://yourserver.com/api/friends/{friendId}");
            if (response.IsSuccessStatusCode)
            {
                await FetchFriendsAsync();
            }
        }
    }

}
