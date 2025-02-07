using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionDrive.Recorder;
public interface IGameRecorder
{
    Task RunAsync(string saveDir, CancellationToken token);
    Task StopAsync(CancellationTokenSource token, bool hasToWrite = true);
    public void Read();
}
