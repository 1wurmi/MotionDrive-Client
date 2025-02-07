using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionDrive.Recorder.iRacingRecorder;
internal class iRacing : IGameRecorder
{
    public void Read()
    {
        throw new NotImplementedException();
    }

    public Task RunAsync(string saveDir, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task StopAsync(CancellationTokenSource token, bool hasToWrite = true)
    {
        throw new NotImplementedException();
    }
}
