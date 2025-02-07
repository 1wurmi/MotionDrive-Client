using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionDrive.Recorder.iRacingRecorder.BitFields;
public class EngineWarning : BitfieldBase<EngineWarnings>
{
    public EngineWarning(int value)
        : base(value)
    {
    }
}

[Flags]
public enum EngineWarnings : uint
{
    WaterTemperatureWarning = 0x01,
    FuelPressureWarning = 0x02,
    OilPressureWarning = 0x04,
    EngineStalled = 0x08,
    PitSpeedLimiter = 0x10,
    RevLimiterActive = 0x20
}
