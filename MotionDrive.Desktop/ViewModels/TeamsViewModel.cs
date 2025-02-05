using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop.ViewModels;
public class TeamsViewModel : ReactiveObject
{
    IScreen HostScreen { get; }

    public TeamsViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}
