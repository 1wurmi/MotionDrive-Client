using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Avalonia;
using LiveChartsCore.SkiaSharpView.SKCharts;
using MotionDrive.Desktop.ViewModels.Telemetry;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Desktop;
public partial class TelemtryView : ReactiveUserControl<TelemetryViewModel>
{
    public TelemtryView()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }
}