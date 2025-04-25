using ReactiveUI;
using System;
using System.Collections.Generic;

namespace MotionDrive.DisplayApplication.ViewModels;
public class StandardDisplayViewModel : ReactiveObject
{
    private string _position;
    private string _lap;
    private string _lapTime;
    private string _predTime;
    private string[] _tyres;
    private string _delta;
    private string _gear;
    private string _speed;
    private string _fuelIn;
    private string _fuelPerLap;
    public string Position
    {
        get => _position;
        set => this.RaiseAndSetIfChanged(ref _position, value);
    }
    public string Lap
    {
        get => _lap;
        set => this.RaiseAndSetIfChanged(ref _lap, value);
    }
    public string LapTime
    {
        get => _lapTime;
        set => this.RaiseAndSetIfChanged(ref _lapTime, value);
    }
    public string PredTime
    {
        get => _predTime;
        set => this.RaiseAndSetIfChanged(ref _predTime, value);
    }
    public string[] Tyres
    {
        get => _tyres;
        set => this.RaiseAndSetIfChanged(ref _tyres, value);
    }
    public string Delta
    {
        get => _delta;
        set => this.RaiseAndSetIfChanged(ref _delta, value);
    }
    public string Gear
    {
        get => _gear;
        set => this.RaiseAndSetIfChanged(ref _gear, value);
    }
    public string Speed
    {
        get => _speed;
        set => this.RaiseAndSetIfChanged(ref _speed, value);
    }
    public string FuelIn
    {
        get => _fuelIn;
        set => this.RaiseAndSetIfChanged(ref _fuelIn, value);
    }
    public string FuelPerLap
    {
        get => _fuelPerLap;
        set => this.RaiseAndSetIfChanged(ref _fuelPerLap, value);
    }

    public StandardDisplayViewModel()
    {
        Position = "0/0";
        Lap = "0";
        LapTime = "2:16.333";
        PredTime = "2:15:999";
        this.Tyres = new string[] { "26.4", "26.9", "25.9", "27.2" };
        Delta = "+0.334";
        Gear = "5";
        Speed = "180 km/h";
        FuelIn = "44.3 L";
        FuelPerLap = "2.4 L";
    }
}