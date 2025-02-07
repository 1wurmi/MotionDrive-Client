using MotionDrive.Recorder.ACCRecorder;
using MotionDrive.Recorder.ACCRecorder.SharedMemory;
using MotionDrive.Recorder.ACCRecorder.SharedMemory.Models;
using MotionDrive.Recorder.Enum;
using MotionDrive.Recorder.iRacingRecorder.irsdksharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
    public string currentCar = "";
    public string currentTrack = "";
    public List<object> Sessions;

    private void OnSessionInfoUpdated(object sender, SessionInfoUpdatedEventArgs e)
    {
        // e.UpdateTime => check if data is not coming twice
        // Trace.WriteLine(e.SessionInfo);

        var deserializer = new Deserializer();
        Dictionary<object, object> result = deserializer.Deserialize<Dictionary<object, object>>(new StringReader(e.SessionInfo));

        var weekendInfo = result["WeekendInfo"] as Dictionary<object, object>;
        var driverInfo = result["DriverInfo"] as Dictionary<object, object>;

        Sessions = (result["SessionInfo"] as Dictionary<object, object>)["Sessions"] as List<object>;

        int driverIdx = int.Parse(driverInfo["DriverCarIdx"].ToString());
        Dictionary<object, object> driver = ((List<object>)driverInfo["Drivers"])[driverIdx] as Dictionary<object, object>;

        currentCar = driver["CarScreenName"] as string;
        currentTrack = weekendInfo["TrackName"] as string;
    }

    private void OnTelemetryUpdated(object sender, TelemetryUpdatedEventArgs e)
    {
        //e.TelemetryInfo.SessionNum;

        Trace.WriteLine(e.TelemetryInfo.Throttle.Value);

        if (LastUpdateTime == e.UpdateTime)
            return;

        if (CurrentSessionIndex == null || e.TelemetryInfo.SessionNum.Value != CurrentSessionIndex)
        {
            CurrentSessionIndex = e.TelemetryInfo.SessionNum.Value;
            SessionType st;
            switch ((Sessions[e.TelemetryInfo.SessionNum.Value] as Dictionary<object, object>)["SessionType"].ToString().ToLower()) {
                case "offline testing":
                    st = SessionType.OFFLINE_TESTING;
                    break;

                case "practice":
                    st = SessionType.PRACTICE;
                    break;

                case "qualifying":
                    st = SessionType.QUALIFYING;
                    break;

                default: 
                    st = SessionType.RACE;
                    break;
            }
            rm.NewSessionStarted(currentCar, currentTrack, st, Game.IRACING);
        }

        if (e.TelemetryInfo.IsOnTrack.Value)
        {
            Trace.WriteLine("ON TRACK");
            rm.AddNewTelemetryPacket(
                new Model.TelemetryPacket
                {
                    Throttle = e.TelemetryInfo.Throttle.Value,
                    Brake = e.TelemetryInfo.Brake.Value,
                    Clutch = e.TelemetryInfo.Clutch.Value,
                    Steering = e.TelemetryInfo.SteeringWheelAngle.Value,
                    RPM = ((int)e.TelemetryInfo.RPM.Value),
                    Time = new TimeSpan(0, 0, 0, 0, (int)e.TelemetryInfo.LapCurrentLapTime.Value),
                    NormalizedCarPosition = e.TelemetryInfo.LapDistPct.Value,
                    TyreWear = [0f, 0f, 0f, 0f], // NEED TO FIND A SOLUTION BECAUSE ONLY AVAILABLE IF CAR HAS THIS SENSOR
                    TyrePressure = [0f, 0f, 0f, 0f], // NEED TO FIND A SOLUTION BECAUSE ONLY AVAILABLE IF CAR HAS THIS SENSOR
                    BrakeTemp = [0f, 0f, 0f, 0f], // NEED TO FIND A SOLUTION BECAUSE ONLY AVAILABLE IF CAR HAS THIS SENSOR
                    iBestTime = (int)e.TelemetryInfo.LapBestLapTime.Value,
                    iLastTime = (int)e.TelemetryInfo.LapLastLapTime.Value,
                    completedLaps = e.TelemetryInfo.Lap.Value,
                    isValidLap = e.TelemetryInfo.LapDeltaToBestLap_OK.Value,
                }
            );
        }
    }

}
