using Microsoft.Extensions.Logging;
using MotionDrive.Recorder.ACCRecorder;
using MotionDrive.Recorder.ACCRecorder.SharedMemory;
using MotionDrive.Recorder.iRacingRecorder.SharedMemory;
using SVappsLAB.iRacingTelemetrySDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionDrive.Recorder.iRacingRecorder;
internal class iRacing : IGameRecorder
{
    iRacingSharedMemoryReader irsmr;
    RecordManager rm = new RecordManager();
    public void Read()
    {

    }
    public void StopReading()
    {
    }

    public Task RunAsync(string saveDir, CancellationToken token)
    {
        rm.SaveDir = saveDir;
        Trace.WriteLine("iRacing has been Started");
        return Task.Run(() =>
        {
            Read();
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
            }

        }, token);
    }

    public async Task StopAsync(CancellationTokenSource token, bool hasToWrite = true)
    {
        Trace.WriteLine("STOPPING iRacing");

        StopReading();
        token.Cancel();

        if (hasToWrite)
            rm.WriteToFile(true).Start();

        Trace.WriteLine("iRacing STOPPED");
    }

}
