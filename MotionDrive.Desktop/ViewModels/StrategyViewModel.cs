﻿using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop.ViewModels;
public class StrategyViewModel : ReactiveObject
{
    IScreen HostScreen { get; }

    public StrategyViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}
