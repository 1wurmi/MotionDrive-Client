using Recorder.ACC.SharedMemory;
using Recorder.ACC.SharedMemory.Models;
using Recorder.Enum;
using Recorder.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recorder.ACC;
internal class ACC : IGameRecorder
{
    ACCSharedMemoryReader accr;
    RecordManager rm = new RecordManager();
    public Task RunAsync(string saveDir, CancellationToken token)
    {
        rm.SaveDir = saveDir;
        Trace.WriteLine("ACC has been Started");
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

    public async Task StopAsync(CancellationTokenSource source, bool hasToWrite = true)
    {
        Trace.WriteLine("STOPPING ACC");

        this.StopReading();
        source.Cancel();

        if (hasToWrite)
            this.rm.WriteToFile(true).Start();

        Trace.WriteLine("STOPPED");
    }

    public void Read()
    {
        this.accr = new ACCSharedMemoryReader(1000, 100, 100, 100);

        this.accr.EverythingUpdated += OnEverythingUpdated;

        this.accr.Start();
    }

    public void StopReading()
    {
        this.accr.Stop();
    }

    public int? CurrentSessionIndex;
    public AC_SESSION_TYPE? CurrentSessionType;
    private void OnEverythingUpdated(object sender, EverythingEventArgs e)
    {
        if (CurrentSessionIndex == null || e.Graphics.SessionIndex != CurrentSessionIndex || CurrentSessionType != e.Graphics.Session)
        {
            CurrentSessionIndex = e.Graphics.SessionIndex;
            CurrentSessionType = e.Graphics.Session;
            SessionType st;

            switch (e.Graphics.Session)
            {
                case AC_SESSION_TYPE.AC_PRACTICE:
                    st = SessionType.PRACTICE;
                    break;

                case AC_SESSION_TYPE.AC_QUALIFY:
                    st = SessionType.QUALIFYING;
                    break;

                case AC_SESSION_TYPE.AC_HOTLAP:
                    st = SessionType.HOTLAP;
                    break;

                default:
                    st = SessionType.RACE;
                    break;
            }

            rm.NewSessionStarted(e.StaticInfo.CarModel, e.StaticInfo.Track, st);
        }

        if (e.Graphics.IsInPit == 0)
        {
            rm.AddNewTelemetryPacket(
                new TelemetryPacket()
                {
                    Throttle = e.Physics.Gas,
                    Brake = e.Physics.Brake,
                    Clutch = e.Physics.Clutch,
                    Steering = e.Physics.SteerAngle,
                    RPM = e.Physics.Rpms,
                    Time = new TimeSpan(0, 0, 0, 0, e.Graphics.iCurrentTime),
                    NormalizedCarPosition = e.Graphics.NormalizedCarPosition,
                    TyreWear = e.Physics.TyreWear,
                    TyrePressure = e.Physics.WheelPressure,
                    BrakeTemp = e.Physics.BrakeTemp,
                    iBestTime = e.Graphics.iBestTime,
                    iLastTime = e.Graphics.iLastTime,
                    completedLaps = e.Graphics.CompletedLaps,
                    isValidLap = e.Graphics.IsValidLap != 0,
                }
            );
        }
    }
}
