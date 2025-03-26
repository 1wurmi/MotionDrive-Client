using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.SKCharts;
using MotionDrive.Desktop.ViewModels.Telemetry;
using ReactiveUI;
using System.Linq;

namespace MotionDrive.Desktop;
public partial class TelemetryChartsView : ReactiveUserControl<TelemetryChartsViewModel>
{

    private Popup OptionsPopup;
    private Button DropdownButton;
    private CheckBox Option1, Option2, Option3;

    public TelemetryChartsView()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);

        OptionsPopup = this.FindControl<Popup>("optionsPopup");
        DropdownButton = this.FindControl<Button>("dropdownButton");
        Option1 = this.FindControl<CheckBox>("option1");
        Option2 = this.FindControl<CheckBox>("option2");
        Option3 = this.FindControl<CheckBox>("option3");
    }

    private void ComboBox_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        ComboBox cb = sender as ComboBox;
        ViewModel.LoadData(cb.SelectedIndex); 
    }

    private void CartesianChart_PointerMoved(object? sender, Avalonia.Input.PointerEventArgs e)
    {
        var chart = sender as LiveChartsCore.SkiaSharpView.Avalonia.CartesianChart;
        var position = e.GetPosition(chart);

        var lvc = new LvcPointD(position.X, position.Y);

        // Ermitteln des Datenpunkts anhand der Mausposition
        var chartPoints = chart.GetPointsAt(lvc);

        if (chartPoints.Count() > 0)
            ViewModel.UpdateTyreData(chartPoints.FirstOrDefault().Index);
    }

    private void TogglePopup(object? sender, RoutedEventArgs e)
    {
        OptionsPopup.IsOpen = !OptionsPopup.IsOpen;
    }
}