using MotionDrive.Recorder.ACCRecorder;
using MotionDrive.Recorder.ACCRecorder.SharedMemory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MotionDrive.Recorder.iRacingRecorder.SdkWrapper;

namespace MotionDrive.Recorder.iRacingRecorder;
internal class iRacing : IGameRecorder
{
    RecordManager rm = new RecordManager();
    SdkWrapper wrapper = new SdkWrapper();
    public void Read()
    {
        this.wrapper.TelemetryUpdated += this.OnTelemetryUpdated;

        this.wrapper.Start();
    }
    public void StopReading()
    {
        this.wrapper.Stop();
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

    private void OnTelemetryUpdated(object sender, TelemetryUpdatedEventArgs e)
    {
        Trace.WriteLine(e.TelemetryInfo.Throttle);   
    }
}
