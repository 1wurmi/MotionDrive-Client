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

    public FriendsService FriendsService => FriendsService.Instance;
    public ObservableCollection<Friend> Friends => FriendsService.Instance.Friends;

    public FriendsViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
    public void RequestFriendAsync()
    {
        this.FriendsService.RequestFriendAsync(FriendCode);
    }
}
