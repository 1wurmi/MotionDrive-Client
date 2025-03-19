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
using System.Windows.Input;

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

    public ICommand AcceptFriendAsync { get; }
    public ICommand DeclineFriendAsync { get; }

    public FriendsViewModel(IScreen screen)
    {
        HostScreen = screen;
        this.FriendsService.InitializeAsync();

        AcceptFriendAsync = ReactiveCommand.CreateFromTask<string>(this.AcceptFriendRequestAsync);
        DeclineFriendAsync = ReactiveCommand.CreateFromTask<string>(this.DeclineFriendRequestAsync);
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

    public async Task AcceptFriendRequestAsync(string requestId)
    {
        await this.FriendsService.AcceptFriendAsync(requestId);
    }

    public async Task DeclineFriendRequestAsync(string requestId)
    {
        await this.FriendsService.DeclineFriendAsync(requestId);
    }
}
