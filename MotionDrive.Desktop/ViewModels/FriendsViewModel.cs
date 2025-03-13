using Avalonia.Controls.Documents;
using MotionDrive.Desktop.Models;
using MotionDrive.Desktop.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop.ViewModels;
public class FriendsViewModel : ReactiveObject
{
    IScreen HostScreen { get; }

    private string _friendCode;
    public string FriendCode
    {
        get => _friendCode ?? "";
        set => this.RaiseAndSetIfChanged(ref _friendCode, value);
    }

    private bool _hasError = false;
    public bool HasError
    {
        get => _hasError;
        set => this.RaiseAndSetIfChanged(ref _hasError, value);
    }

    public FriendsService FriendsService => FriendsService.Instance;
    public ObservableCollection<Friend> Friends => FriendsService.Instance.Friends;
    public ObservableCollection<FriendRequest> FriendRequests => FriendsService.Instance.FriendRequests;

    public FriendsViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
    public async Task RequestFriendAsync()
    {
        if (await this.FriendsService.RequestFriendAsync(FriendCode))
        {
            FriendCode = "";
            HasError = false;
        }
        else
        {
            HasError = true;
        }
    }
}
