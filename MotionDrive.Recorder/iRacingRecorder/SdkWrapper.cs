using MotionDrive.Recorder.iRacingRecorder.irsdksharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionDrive.Recorder.iRacingRecorder;
internal class SdkWrapper
{
    internal readonly iRacingSDK sdk;
    private int waitTime;
    private readonly SynchronizationContext context;

    public SdkWrapper()
    {
        this.context = SynchronizationContext.Current;
        sdk = new iRacingSDK();

        this.TelemetryUpdateFrequency = 10;

        _DriverId = -1;
    }

    #region Properties

    private bool _IsRunning;
    /// <summary>
    /// Is the main loop running?
    /// </summary>
    public bool IsRunning { get { return _IsRunning; } }

    private bool _IsConnected;
    /// <summary>
    /// Is the SDK connected to iRacing?
    /// </summary>
    public bool IsConnected { get { return _IsConnected; } }

    private int _TelemetryUpdateFrequency;
    /// <summary>
    /// Gets or sets the number of times the telemetry info is updated per second. The default and maximum is 60 times per second.
    /// </summary>
    public int TelemetryUpdateFrequency
    {
        get { return _TelemetryUpdateFrequency; }
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException("TelemetryUpdateFrequency must be at least 1.");
            if (value > 60)
                throw new ArgumentOutOfRangeException("TelemetryUpdateFrequency cannot be more than 60.");

            _TelemetryUpdateFrequency = value;

            waitTime = (int)Math.Floor(1000f / value) - 1;
        }
    }

    private int _DriverId;
    /// <summary>
    /// Gets the Id (CarIdx) of yourself (the driver running this application).
    /// </summary>
    public int DriverId { get { return _DriverId; } }

    #endregion

    public object GetData(string headerName)
    {

       return sdk.GetData(headerName);
    }
    
    public TelemetryValue<T> GetTelemetryValue<T>(string name)
    {
        return new TelemetryValue<T>(sdk, name);
    }
    public void Start()
    {
        _IsRunning = true;

        Thread t = new Thread(Loop);
        t.Start();
        //Thread lt = new Thread(LapTimingLoop);
        //lt.Start();
    }

    /// <summary>
    /// Stops the main loop
    /// </summary>
    public void Stop()
    {
        _IsRunning = false;
    }
    public void Loop()
    {
        int lastUpdate = -1;

        while (_IsRunning)
        {
            // Check if we can find the sim
            if (sdk.IsConnected())
            {
                if (!_IsConnected)
                {
                    // If this is the first time, raise the Connected event
                    this.RaiseEvent(OnConnected, EventArgs.Empty);
                }

                _IsConnected = true;

                // Get the session info string
                string sessionInfo = sdk.GetSessionInfo();

                // Parse out your own driver Id
                if (this.DriverId == -1) _DriverId = int.Parse(YamlParser.Parse(sessionInfo, "DriverInfo:DriverCarIdx:"));

                // Get the session time (in seconds) of this update
                var time = (double)sdk.GetData("SessionTime");

                // Is the session info updated?
                int newUpdate = sdk.Header.SessionInfoUpdate;
                if (newUpdate != lastUpdate)
                {
                    lastUpdate = newUpdate;

                    // Raise the SessionInfoUpdated event and pass along the session info and session time.
                    var sessionArgs = new SessionInfoUpdatedEventArgs(sessionInfo, time);
                    this.RaiseEvent(OnSessionInfoUpdated, sessionArgs);
                }

                // Raise the TelemetryUpdated event and pass along the lap info and session time
                var telArgs = new TelemetryUpdatedEventArgs(new TelemetryInfo(sdk), time);
                this.RaiseEvent(OnTelemetryUpdated, telArgs);

            }
            else if (sdk.IsInitialized)
            {
                // We have already been initialized before, so the sim is closing
                this.RaiseEvent(OnDisconnected, EventArgs.Empty);

                sdk.Shutdown();
                _DriverId = -1;
                lastUpdate = -1;
                _IsConnected = false;
            }
            else
            {
                _IsConnected = false;
                _DriverId = -1;

                //Try to find the sim
                sdk.Startup();
            }

            // Sleep for a short amount of time until the next update is available
            if (_IsConnected)
            {
                if (waitTime <= 0 || waitTime > 1000) waitTime = 15;
                Thread.Sleep(waitTime);
            }
            else
            {
                // Not connected yet, no need to check every 16 ms, let's try again in a second
                Thread.Sleep(1000);
            }
        }

        sdk.Shutdown();
        _DriverId = -1;
        _IsConnected = false;
    }

    #region Events

    /// <summary>
    /// Event raised when the sim outputs telemetry information (60 times per second).
    /// </summary>
    public event EventHandler<TelemetryUpdatedEventArgs> TelemetryUpdated;

    //public event EventHandler<LapTimingTelemetryUpdatedEventArgs> LapTimingTelemetryUpdated;

    /// <summary>
    /// Event raised when the sim refreshes the session info (few times per minute).
    /// </summary>
    public event EventHandler<SessionInfoUpdatedEventArgs> SessionInfoUpdated;

    /// <summary>
    /// Event raised when the SDK detects the sim for the first time.
    /// </summary>
    public event EventHandler Connected;

    /// <summary>
    /// Event raised when the SDK no longer detects the sim (sim closed).
    /// </summary>
    public event EventHandler Disconnected;

    private void RaiseEvent<T>(Action<T> del, T e)
        where T : EventArgs
    {
        var callback = new SendOrPostCallback(obj => del(obj as T));

        del(e);
    }

    private void OnSessionInfoUpdated(SessionInfoUpdatedEventArgs e)
    {
        var handler = this.SessionInfoUpdated;
        if (handler != null) handler(this, e);
    }

    private void OnTelemetryUpdated(TelemetryUpdatedEventArgs e)
    {
        var handler = this.TelemetryUpdated;
        if (handler != null) handler(this, e);
    }

    //private void OnLapTimingTelemetryUpdated(LapTimingTelemetryUpdatedEventArgs e)
    //{
    //    var handler = this.LapTimingTelemetryUpdated;
    //    if (handler != null) handler(this, e);
    //}

    private void OnConnected(EventArgs e)
    {
        var handler = this.Connected;
        if (handler != null) handler(this, e);
    }

    private void OnDisconnected(EventArgs e)
    {
        var handler = this.Disconnected;
        if (handler != null) handler(this, e);
    }

    #endregion

    public class SdkUpdateEventArgs : EventArgs
    {
        public SdkUpdateEventArgs(double time)
        {
            _UpdateTime = time;
        }

        private readonly double _UpdateTime;
        /// <summary>
        /// Gets the time (in seconds) when this update occured.
        /// </summary>
        public double UpdateTime { get { return _UpdateTime; } }
    }

    public class SessionInfoUpdatedEventArgs : SdkUpdateEventArgs
    {
        public SessionInfoUpdatedEventArgs(string sessionInfo, double time) : base(time)
        {
            _SessionInfo = sessionInfo;
        }

        private readonly string _SessionInfo;
        /// <summary>
        /// Gets the session info string.
        /// </summary>
        public string SessionInfo { get { return _SessionInfo; } }
    }

    public class TelemetryUpdatedEventArgs : SdkUpdateEventArgs
    {
        public TelemetryUpdatedEventArgs(TelemetryInfo info, double time) : base(time)
        {
            _TelemetryInfo = info;
        }

        private readonly TelemetryInfo _TelemetryInfo;
        /// <summary>
        /// Gets the telemetry info object.
        /// </summary>
        public TelemetryInfo TelemetryInfo { get { return _TelemetryInfo; } }
    }

}
