using MotionDrive.Recorder.ACCRecorder;
using MotionDrive.Recorder.ACCRecorder.SharedMemory;
using MotionDrive.Recorder.ACCRecorder.SharedMemory.Models;
using MotionDrive.Recorder.Enum;
using MotionDrive.Recorder.iRacingRecorder.irsdksharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using static MotionDrive.Recorder.iRacingRecorder.SdkWrapper;

namespace MotionDrive.Recorder.iRacingRecorder;
internal class iRacing : IGameRecorder
{
    RecordManager rm = new RecordManager();
    SdkWrapper wrapper = new SdkWrapper();
    public void Read()
    {
        this.wrapper.SessionInfoUpdated += this.OnSessionInfoUpdated;
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

    public int? CurrentSessionIndex;
    public double LastUpdateTime = 0;
    

    private void OnSessionInfoUpdated(object sender, SessionInfoUpdatedEventArgs e)
    {
        // e.UpdateTime => check if data is not coming twice
        // Trace.WriteLine(e.SessionInfo);

        var deserializer = new Deserializer();
        Dictionary<object, object> result = deserializer.Deserialize<Dictionary<object, object>>(new StringReader(e.SessionInfo));

        var weekendInfo = result["WeekendInfo"] as Dictionary<object, object>;
        var driverInfo = result["DriverInfo"] as Dictionary<object, object>;

        int driverIdx = int.Parse(driverInfo["DriverCarIdx"].ToString());
        Dictionary<object, object> driver = ((List<object>)driverInfo["Drivers"])[driverIdx] as Dictionary<object, object>;

        rm.NewSessionStarted(driver["CarScreenName"] as string, weekendInfo["TrackName"] as string, SessionType.PRACTICE, Game.IRACING);
    }

    private void OnTelemetryUpdated(object sender, TelemetryUpdatedEventArgs e)
    {
        //e.TelemetryInfo.SessionNum;

        if (LastUpdateTime == e.UpdateTime)
            return;

        if (CurrentSessionIndex == null || e.TelemetryInfo.SessionNum.Value != CurrentSessionIndex )
        {
            CurrentSessionIndex = e.TelemetryInfo.SessionNum.Value;
            SessionType st;

        }
    }

}
