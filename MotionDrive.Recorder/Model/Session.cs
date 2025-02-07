using MotionDrive.Recorder.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionDrive.Recorder.Model;
public class Session
{
    public Game gameName { get; set; }
    public List<Lap> Laps { get; set; } = new List<Lap>();
    public string? CarName { get; set; }
    public string? TrackName { get; set; }
    public SessionType? SessionType { get; set; }
}
