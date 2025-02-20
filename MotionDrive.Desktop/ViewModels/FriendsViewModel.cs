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

    public ObservableCollection<Friend> Friends => FriendsService.Instance.Friends;

    public FriendsViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}
