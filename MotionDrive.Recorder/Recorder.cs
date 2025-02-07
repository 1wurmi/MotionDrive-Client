using MotionDrive.Recorder.iRacing;
using Recorder.Enum;
using System.Diagnostics;

namespace Recorder;
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
        this.ResetCancellationToken();

        while (true)
        {

            if (this.CurrentGameRecorder != null)
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
                        this.CurrentGameRecorder = new ACC.ACC();
                        this.CurrentProcessName = "acs";
                    }
                    else if (process.ProcessName == "acc")
                    {
                        CurrentGame = Game.ACC;
                        this.CurrentGameRecorder = new ACC.ACC();
                        this.CurrentProcessName = "acc";
                    }
                    else if (process.ProcessName == "iRacingSim64DX11")
                    {
                        CurrentGame = Game.IRACING;
                        this.CurrentGameRecorder = new iRacing();
                        this.CurrentProcessName = "iRacingSim64DX11";
                    }
                }

                if (this.CurrentGameRecorder != null)
                    this.CurrentGameRecorder.RunAsync(this.SaveDir, cancellationToken);
            }
        }
    }

    public void StopRecording(bool hasToWrite)
    {
        if (this.CurrentGameRecorder != null)
            this.CurrentGameRecorder.StopAsync(cancellationTokenSource, hasToWrite);
        cancellationTokenSource.Cancel();
    }
    public void GameStillRunning()
    {
        if (this.CurrentGame == null || this.CurrentGameRecorder == null) return;

        if (Process.GetProcessesByName(this.CurrentProcessName).Length == 0)
        {
            this.CurrentGameRecorder.StopAsync(cancellationTokenSource);
            this.CurrentGameRecorder = null;
            this.CurrentGame = null;
            this.ResetCancellationToken();
        }
    }

    public void ResetCancellationToken()
    {
        this.cancellationTokenSource = new CancellationTokenSource();
        this.cancellationToken = this.cancellationTokenSource.Token;
    }
}
