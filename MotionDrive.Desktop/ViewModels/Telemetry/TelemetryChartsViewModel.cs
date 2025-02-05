using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Geo;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MotionDrive.Desktop.Models;
using ReactiveUI;
using Recorder.Model;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionDrive.Desktop.ViewModels.Telemetry;
public class TelemetryChartsViewModel : ReactiveObject
{
    private int _currentLapIndex = -1;
    public int CurrentLapIndex
    {
        get => _currentLapIndex;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentLapIndex, value);

            if (value == 0)
                PreviousLapDisabled = true;
            else
                PreviousLapDisabled = false;

            if (value == this.session.Laps.Count - 1)
                NextLapDisabled = true;
            else
                NextLapDisabled = false;
        }
    }

    private bool _previousLapDisabled = true, _nextLapDisabled = false;
    public bool PreviousLapDisabled
    {
        get => _previousLapDisabled;
        set => this.RaiseAndSetIfChanged(ref _previousLapDisabled, value);
    }
    public bool NextLapDisabled
    {
        get => _nextLapDisabled;
        set => this.RaiseAndSetIfChanged(ref _nextLapDisabled, value);
    }
    private Axis[] ThrottleBrakeClutchYAxis { get; set; } = [
        new Axis
        {
            MaxLimit=1,
            MinLimit=0,
            LabelsDensity=0,
            CrosshairLabelsBackground=SKColors.Gray.AsLvcColor(),
            CrosshairLabelsPaint = new SolidColorPaint(SKColors.DarkRed, 1),
            CrosshairPaint = new SolidColorPaint(SKColors.Red, 1),
            Labeler= value => value.ToString("N2"),
        }
    ];
    private Axis[] ThrottleBrakeClutchXAxis { get; set; } = [
        new TimeSpanAxis (TimeSpan.FromMilliseconds(20), date => date.ToString(@"mm\:ss\:fff"))
        {
            MinLimit=0,
            CrosshairPaint = new SolidColorPaint(SKColors.Red, 1),
        }
    ];
    private Axis[] SteeringYAxis { get; set; } = [
        new Axis
        {
            MinLimit=-1,
            MaxLimit=1,
            CrosshairPaint = new SolidColorPaint(SKColors.Red, 1),
        }
    ];

    private Axis[] RPMYAxis { get; set; } = [
        new Axis
        {
            MaxLimit=10000, MinLimit=0,
            CrosshairPaint = new SolidColorPaint(SKColors.Red, 1),
        }
    ];

    private ISeries[] ThrottleBrakeClutchSeries { get; set; } = [
        new LineSeries<TimeSpanPoint> { Name="T:", Stroke=new SolidColorPaint(SKColors.Lime), Fill=null, LineSmoothness=0, GeometryStroke=null, GeometrySize=0, GeometryFill=null, ZIndex=10 },
        new LineSeries<TimeSpanPoint> { Name="B:", Stroke=new SolidColorPaint(SKColors.Red), Fill=null, LineSmoothness=0, GeometryStroke=null, GeometrySize=0, GeometryFill=null, ZIndex=9 },
        new LineSeries<TimeSpanPoint> { Name="C:", Stroke=new SolidColorPaint(SKColors.Gray), Fill=null, LineSmoothness=0, GeometryStroke=null, GeometrySize=0, GeometryFill=null, ZIndex=8 }
    ];
    private ISeries[] SteeringSeries { get; set; } = [
        new LineSeries<TimeSpanPoint> { Stroke=new SolidColorPaint(SKColors.Blue), Fill=null, LineSmoothness=0, GeometryStroke=null, GeometrySize=0, GeometryFill=null }
    ];

    private ISeries[] RPMSeries { get; set; } = [
        new LineSeries<TimeSpanPoint> { Stroke=new SolidColorPaint(SKColors.Aqua), Fill=null, LineSmoothness=0, GeometryStroke=null, GeometrySize=0, GeometryFill=null }
    ];

    private SolidColorPaint TooltipBackground { get; set; } = new SolidColorPaint(new SKColor(0, 0, 0, 90));

    private Tyres _currentTyres;
    public Tyres CurrentTyres
    {
        get => _currentTyres;
        set => this.RaiseAndSetIfChanged(ref _currentTyres, value);
    }

    public Session session { get; }

    public ObservableCollection<LapModel> Laps { get; set; }

    public TelemetryViewModel parentTelemetryViewModel;
    public TelemetryChartsViewModel(Session session, TelemetryViewModel parentTelemetryViewModel)
    {
        this.parentTelemetryViewModel = parentTelemetryViewModel;
        this.session = session;
        this.Initialize();
    }

    public void GoToChooseSession()
    {
        this.parentTelemetryViewModel.GoToChooseSession();
    }

    public void Initialize()
    {
        this.Laps = new ObservableCollection<LapModel>();
        foreach (Lap lap in session.Laps)
        {
            string color = "white";
            if (!lap.isValid)
                color = "red";
            else if (lap.isValid && lap.isFastestLap)
                color = "#de08ff";

            string name = "Lap " + Laps.Count;
            string time_as_str;

            if (lap.isValid && lap.isFastestLap)
                time_as_str = TimeSpan.FromMilliseconds(lap.LapTime).ToString(@"mm\:ss\:fff");
            else
                time_as_str = (lap.DeltaToFastestLap < 0 ? "-" : "+") + TimeSpan.FromMilliseconds(lap.DeltaToFastestLap).ToString(@"ss\:fff");

            Laps.Add(new LapModel { LapName = name + " (" + time_as_str + ")", Color = color });
        }

        LoadData(0);
        CurrentLapIndex = 0;
    }


    List<float[]> tyreWearValues = new List<float[]>();
    List<float[]> brakeTempValues = new List<float[]>();
    List<float[]> tyrePressureValues = new List<float[]>();
    public void LoadData(int lapIndex)
    {
        CurrentLapIndex = lapIndex;

        List<TimeSpanPoint> throttleValues = new();
        List<TimeSpanPoint> brakeValues = new();
        List<TimeSpanPoint> clutchValues = new();
        List<TimeSpanPoint> steeringValues = new();
        List<TimeSpanPoint> rpmValues = new();

        List<float[]> tyreWearValues = new();
        List<float[]> brakeTempValues = new();
        List<float[]> tyrePressureValues = new();

        for (int i = 0; i < session.Laps[lapIndex].TelemetryPackets.Count; i++)
        {
            TelemetryPacket telemetry = session.Laps[lapIndex].TelemetryPackets[i];
            throttleValues.Add(new TimeSpanPoint(telemetry.Time, telemetry.Throttle));
            brakeValues.Add(new TimeSpanPoint(telemetry.Time, telemetry.Brake));
            clutchValues.Add(new TimeSpanPoint(telemetry.Time, telemetry.Clutch));
            steeringValues.Add(new TimeSpanPoint(telemetry.Time, telemetry.Steering));
            rpmValues.Add(new TimeSpanPoint(telemetry.Time, telemetry.RPM));

            tyreWearValues.Add(telemetry.TyreWear);
            brakeTempValues.Add(telemetry.BrakeTemp);
            tyrePressureValues.Add(telemetry.TyrePressure);
        }

        ThrottleBrakeClutchSeries[0].Values = throttleValues;
        ThrottleBrakeClutchSeries[1].Values = brakeValues;
        ThrottleBrakeClutchSeries[2].Values = clutchValues;
        SteeringSeries[0].Values = steeringValues;
        RPMSeries[0].Values = rpmValues;

        this.tyreWearValues = tyreWearValues;
        this.brakeTempValues = brakeTempValues;
        this.tyrePressureValues = tyrePressureValues;
    }

    public void UpdateTyreData(int index)
    {
        CurrentTyres = new Tyres(
            Math.Round(tyrePressureValues[index][0], 1),
            Math.Round(tyrePressureValues[index][1], 1),
            Math.Round(tyrePressureValues[index][2], 1),
            Math.Round(tyrePressureValues[index][3], 1),
            Math.Round(brakeTempValues[index][0], 1),
            Math.Round(brakeTempValues[index][1], 1),
            Math.Round(brakeTempValues[index][2], 1),
            Math.Round(brakeTempValues[index][3], 1),
            Math.Round(tyreWearValues[index][0], 1),
            Math.Round(tyreWearValues[index][1], 1),
            Math.Round(tyreWearValues[index][2], 1),
            Math.Round(tyreWearValues[index][3], 1)
        );
    }

    public async void ShowPreviousLap()
    {
        LoadData(CurrentLapIndex - 1);
    }

    public async void ShowNextLap()
    {
        LoadData(CurrentLapIndex + 1);
    }
}
public class LapModel
{
    public string LapName { get; set; }
    public string Color { get; set; }
}

public class FolderCardModel
{
    public string FolderPath { get; set; }
    public string FolderName { get; set; }

}

