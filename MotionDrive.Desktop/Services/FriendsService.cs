using Microsoft.AspNetCore.SignalR.Client;
using MotionDrive.Desktop.Config;
using MotionDrive.Desktop.Models;
using MotionDrive.Desktop.SecrectsConfig;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MotionDrive.Desktop.Services;
public class FriendsService
{
    private static FriendsService? _instance;
    private ConfigManager cm = new ConfigManager();
    private ConfigModel loadedConfig;

    private SecretsManager sm = new SecretsManager();
    private SecretConfigModel loadedSecrets;
    public static FriendsService Instance => _instance ??= new FriendsService();
    
    public ObservableCollection<Friend> Friends = new ObservableCollection<Friend>();
    public ObservableCollection<FriendRequest> FriendRequests = new ObservableCollection<FriendRequest>();

    private HubConnection _hubConnection;

    private FriendsService()
    {
        loadedConfig = cm.LoadConfig();
        loadedSecrets = sm.LoadSecrets();

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(loadedConfig.APIUrl + "/friendsHub?access_token="+loadedSecrets.JWT)
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<Friend>("UserStatusChanged", friend =>
        {
            var existingFriend = Friends.FirstOrDefault(f => f.Id == friend.Id);
            if (existingFriend != null)
            {
                existingFriend.IsOnline = friend.IsOnline;
            }
        });
    }

    public async Task InitializeAsync()
    {
        await FetchFriendsAsync();  // Load friends from the API
        await FetchFriendRequestsAsync();
        await _hubConnection.StartAsync();  // Connect to the SignalR hub
    }

    private async Task FetchFriendsAsync()
    {
        using (var client = new System.Net.Http.HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loadedSecrets.JWT);
            var response = await client.GetStringAsync(loadedConfig.APIUrl + "/friends");
            var friendsList = JsonSerializer.Deserialize<List<Friend>>(response, new JsonSerializerOptions() { PropertyNameCaseInsensitive=true});

            Friends.Clear();
            foreach (var friend in friendsList)
            {
                Friends.Add(friend);  // Populate the friends list
            }
        }
    }

    private async Task FetchFriendRequestsAsync()
    {
        using (var client = new System.Net.Http.HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loadedSecrets.JWT);
            var response = await client.GetStringAsync(loadedConfig.APIUrl + "/friends/requests");
            var requestList = JsonSerializer.Deserialize<List<FriendRequest>>(response, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            FriendRequests.Clear();
            foreach (var friend in requestList)
            {
                FriendRequests.Add(friend); 
            }
        }
    }

    public async Task<bool> RequestFriendAsync(string friendCode)
    {
        using (var client = new System.Net.Http.HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loadedSecrets.JWT);
            var response = await client.PostAsync(loadedConfig.APIUrl  + "/friends/request", new StringContent($"{{\"friendCode\":\"{friendCode}\"}}", Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                await FetchFriendsAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public async Task AcceptFriendAsync(string requestId)
    {
        using (var client = new System.Net.Http.HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loadedSecrets.JWT);
            var response = await client.PutAsync(loadedConfig.APIUrl + "/friends/accept", new StringContent($"{{\"requestId\":\"{requestId}\"}}", Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                await InitializeAsync();
            }
        }
    }
    public async Task DeclineFriendAsync(string requestId)
    {
        using (var client = new System.Net.Http.HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loadedSecrets.JWT);
            var response = await client.PutAsync(loadedConfig.APIUrl + "/friends/decline", new StringContent($"{{\"requestId\":\"{requestId}\"}}", Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
                await InitializeAsync();
        }
    }

    public async Task RemoveFriendAsync(string friendId)
    {
        using (var client = new System.Net.Http.HttpClient())
        {
            var response = await client.DeleteAsync(loadedConfig.APIUrl + $"/api/friends/{friendId}");
            if (response.IsSuccessStatusCode)
            {
                await FetchFriendsAsync();
            }
        }
    }

}
