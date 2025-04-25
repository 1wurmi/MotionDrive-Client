using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MotionDrive.DisplayApplication.ViewModels;
using ReactiveUI;
using System;

namespace MotionDrive.DisplayApplication;
public partial class StandardDisplayView : ReactiveUserControl<StandardDisplayViewModel>
{
    public StandardDisplayView()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }
}