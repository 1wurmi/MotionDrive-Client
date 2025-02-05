using System;

namespace MotionDrive.Desktop.Models;
public class TelemetryFile
{
    public DateTime Date { get; set; }
    public string Path { get; set; }
    public string SessionType { get; set; }
    public string CarName { get; set; }
    public string TrackName { get; set; }

}