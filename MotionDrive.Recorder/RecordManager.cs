using MotionDrive.Recorder.Enum;
using MotionDrive.Recorder.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Enumeration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MotionDrive.Recorder;
// THIS CLASS MANAGES EVERYTHING RELATED TO MANAGING RECORDED DATA AND WRITING TO FILE
// ALSO IDENTIFIES IF NEW LAP STARTED, BASED ON DATA FROM OTHER CLASS
public class RecordManager
{
    Lap[]? Laps { get; set; }
    public Session? CurrentSession { get; set; }
    public Session? LastSession { get; set; }
    public string? SaveDir { get; set; }

    // MANUALLY REGISTER A NEW SESSION
    public void NewSessionStarted(string carName, string trackName, SessionType sessionType)
    {
        if (CurrentSession != null)
        {
            LastSession = CurrentSession;

            WriteToFile().Start();
        };

        CurrentSession = new Session()
        {
            CarName = carName,
            TrackName = trackName,
            SessionType = sessionType
        };

        CurrentSession.Laps.Add(new Lap() { LapName = "Lap " + CurrentSession.Laps.Count });
    }

    private TelemetryPacket? oldPacket = null;
    public void AddNewTelemetryPacket(TelemetryPacket tp)
    {
        if (oldPacket != null)
        {
            if (oldPacket.Time == tp.Time)
                return;

            if (tp.Time < oldPacket.Time)
            {
                CurrentSession.Laps.Last().isValid = oldPacket.isValidLap;
                CurrentSession.Laps.Last().LapTime = tp.iLastTime;

                AddNewLap(new Lap() { LapName = "Lap " + tp.completedLaps });
            }
        }

        oldPacket = tp;

        CurrentSession.Laps.Last().TelemetryPackets.Add(tp);
    }

    private void AddNewLap(Lap lap)
    {
        CurrentSession.Laps.Add(lap);
    }

    public Task WriteToFile(bool manuallyStopped = false)
    {
        return new Task(() =>
        {
            string jsonString;
            string fileName;
            Session sessToUse;
            if (manuallyStopped)
                sessToUse = CurrentSession;
            else
                sessToUse = LastSession;


            int fastestLapTime = sessToUse.Laps.Last().TelemetryPackets.Last().iBestTime;
            foreach (Lap lap in sessToUse.Laps)
            {
                lap.DeltaToFastestLap = lap.LapTime - fastestLapTime;
                if (lap.LapTime == fastestLapTime)
                    lap.isFastestLap = true;
            }


            jsonString = JsonSerializer.Serialize(sessToUse);
            fileName = $"/{sessToUse.SessionType.ToString()}{DateTime.Now.ToString("MM_dd_yyyy_HH_mm")}.json";


            string filePath = SaveDir + fileName;
            File.WriteAllText(filePath, jsonString);

            File.WriteAllText(filePath + ":meta.json", JsonSerializer.Serialize(new { fastestLapTime }));

            return;
        });
    }
}
