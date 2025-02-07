using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionDrive.Recorder.iRacingRecorder;
public enum TrackSurfaces
{
    NotInWorld = -1,
    OffTrack,
    InPitStall,
    AproachingPits,
    OnTrack
}

public enum SessionStates
{
    Invalid,
    GetInCar,
    Warmup,
    ParadeLaps,
    Racing,
    Checkered,
    CoolDown
}
