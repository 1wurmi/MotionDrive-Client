using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionDrive.Recorder.Model;
public class TelemetryPacket
{
    // CURRENT LAP TIME IN MILLISECONDS
    public TimeSpan Time { get; set; }

    // CURRENT CAR POSITION ON TRACK
    public float NormalizedCarPosition { get; set; }
    public float Throttle { get; set; }
    public float Brake { get; set; }
    public float Clutch { get; set; }
    public float Steering { get; set; }
    public int RPM { get; set; }
    public float[] TyreWear { get; set; } // Tyre wear in percent
    public float[] BrakeTemp { get; set; }
    public float[] TyrePressure { get; set; }

    public int iLastTime { get; set; } // Last time in milliseconds
    public int iBestTime { get; set; } // Last time in milliseconds
    public int completedLaps { get; set; } // Number of completed laps
    public bool isValidLap { get; set; } // If the current lap is valid


    public override string ToString()
    {
        return $"Time: {Time}, \n" +
            $"Position: {NormalizedCarPosition}, \n" +
            $"Throttle: {Throttle}, \n" +
            $"Brake: {Brake}, \n" +
            $"Clutch: {Clutch}, \n" +
            $"Steering: {Steering}, \n" +
            $"RPM: {RPM}, \n" +
            $"Tyre Wear: {TyreWear.ToString()}, \n" +
            $"Brake Temp: {BrakeTemp.ToString()}, \n" +
            $"Tyre Pressure: {TyrePressure.ToString()}, \n";
    }
}
