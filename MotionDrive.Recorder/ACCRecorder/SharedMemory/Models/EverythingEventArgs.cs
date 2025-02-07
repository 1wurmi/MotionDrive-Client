using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionDrive.Recorder.ACCRecorder.SharedMemory.Models;
public class EverythingEventArgs : EventArgs
{
    public EverythingEventArgs(Graphics graphics, Physics physics, StaticInfo staticInfo)
    {
        Graphics = graphics;
        Physics = physics;
        StaticInfo = staticInfo;
    }

    public Graphics Graphics { get; private set; }
    public Physics Physics { get; private set; }
    public StaticInfo StaticInfo { get; private set; }
}
