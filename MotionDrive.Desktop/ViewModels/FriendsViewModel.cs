using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop.ViewModels;
public class FriendsViewModel : ReactiveObject
{
    IScreen HostScreen { get; }

    public FriendsViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}
