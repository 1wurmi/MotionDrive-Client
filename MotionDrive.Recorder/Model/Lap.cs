using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recorder.Model;
public class Lap
{
    public List<TelemetryPacket> TelemetryPackets { get; set; } = new List<TelemetryPacket>();
    public required string LapName { get; set; } = "";

    public int LapTime { get; set; } = 0;
    public int DeltaToFastestLap { get; set; } = 0;

    public bool isValid { get; set; } = false;
    public bool isFastestLap { get; set; } = false;

}
