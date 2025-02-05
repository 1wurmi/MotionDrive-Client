using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop.ViewModels;
public class SetupsViewModel : ReactiveObject
{
    IScreen HostScreen { get; }

    public SetupsViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}
