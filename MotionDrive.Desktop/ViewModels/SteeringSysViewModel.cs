using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop.ViewModels;
public class SteeringSysViewModel : ReactiveObject
{
    IScreen HostScreen { get; }

    public SteeringSysViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}
