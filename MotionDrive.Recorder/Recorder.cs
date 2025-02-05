using Recorder.ACC;
using Recorder.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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
                    }
                    else if (process.ProcessName == "acc")
                    {
                        CurrentGame = Game.ACC;
                        this.CurrentGameRecorder = new ACC.ACC();
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
        if (this.CurrentGame == null) return;

        switch (this.CurrentGame)
        {
            case Game.AC:
                if (Process.GetProcessesByName("acs").Length == 0)
                {
                    this.CurrentGameRecorder.StopAsync(cancellationTokenSource);
                    this.CurrentGameRecorder = null;
                    this.CurrentGame = null;
                    this.ResetCancellationToken();
                }
                break;
        }
    }

    public void ResetCancellationToken()
    {
        this.cancellationTokenSource = new CancellationTokenSource();
        this.cancellationToken = this.cancellationTokenSource.Token;
    }
}
