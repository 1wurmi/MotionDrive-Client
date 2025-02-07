using System.Diagnostics;
using MotionDrive.Recorder.ACCRecorder;
using MotionDrive.Recorder.Enum;
using MotionDrive.Recorder.iRacingRecorder;

namespace MotionDrive.Recorder;
public class Recorder
{
    private string _saveDir = "";
    public required string SaveDir
    {
        get { return _saveDir; }
        set { _saveDir = value; }
    }

    private Game? _currentGame;
    public Game? CurrentGame
    {
        get { return _currentGame; }
        set { _currentGame = value; }
    }

    public string _currentProcessName = "";
    public string CurrentProcessName
    {
        get { return _currentProcessName; }
        set { _currentProcessName = value; }
    }

    public IGameRecorder? CurrentGameRecorder = null;

    private CancellationTokenSource cancellationTokenSource;
    private CancellationToken cancellationToken;

    public void StartRecorder()
    {
        ResetCancellationToken();

        while (true)
        {

            if (CurrentGameRecorder != null)
            {
                GameStillRunning();
            }
            else
            {
                Process[] processes = Process.GetProcesses();
                foreach (Process process in processes)
                {
                    if (process.ProcessName == "acs")
                    {
                        CurrentGame = Game.AC;
                        CurrentGameRecorder = new ACC();
                        CurrentProcessName = "acs";
                    }
                    else if (process.ProcessName == "acc")
                    {
                        CurrentGame = Game.ACC;
                        CurrentGameRecorder = new ACC();
                        CurrentProcessName = "acc";
                    }
                    else if (process.ProcessName == "iRacingSim64DX11")
                    {
                        CurrentGame = Game.IRACING;
                        CurrentGameRecorder = new iRacing();
                        CurrentProcessName = "iRacingSim64DX11";
                    }
                }

                if (CurrentGameRecorder != null)
                    CurrentGameRecorder.RunAsync(SaveDir, cancellationToken);
            }
        }
    }

    public void StopRecording(bool hasToWrite)
    {
        if (CurrentGameRecorder != null)
            CurrentGameRecorder.StopAsync(cancellationTokenSource, hasToWrite);
        cancellationTokenSource.Cancel();
    }
    public void GameStillRunning()
    {
        if (CurrentGame == null || CurrentGameRecorder == null) return;

        if (Process.GetProcessesByName(CurrentProcessName).Length == 0)
        {
            CurrentGameRecorder.StopAsync(cancellationTokenSource);
            CurrentGameRecorder = null;
            CurrentGame = null;
            ResetCancellationToken();
        }
    }

    public void ResetCancellationToken()
    {
        cancellationTokenSource = new CancellationTokenSource();
        cancellationToken = cancellationTokenSource.Token;
    }
}
