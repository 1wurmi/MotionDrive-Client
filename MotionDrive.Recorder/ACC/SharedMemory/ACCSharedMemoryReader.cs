using Recorder.ACC.SharedMemory.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Recorder.ACC.SharedMemory;
internal class ACCSharedMemoryReader
{


    public delegate void PhysicsUpdatedHandler(object sender, PhysicsEventArgs e);
    public delegate void GraphicsUpdatedHandler(object sender, GraphicsEventArgs e);
    public delegate void StaticUpdatedHandler(object sender, StaticInfoEventArgs e);
    public delegate void EverythingUpdatedHandler(object sender, EverythingEventArgs e);



    private Timer sharedMemoryRetry;
    private MemoryMappedFile physicsMMF;
    private MemoryMappedFile graphicsMMF;
    private MemoryMappedFile staticMMF;

    private Timer physicsTimer;
    private Timer graphicsTimer;
    private Timer staticTimer;
    private Timer EverythingTimer;

    public int physicsInterval;
    public int graphicsInterval;
    public int staticInterval;
    public int everythingInterval;


    public ACCSharedMemoryReader(int physicsInterval, int graphicsInterval, int staticInterval, int everythingInterval)
    {
        sharedMemoryRetry = new Timer(2000);
        sharedMemoryRetry.AutoReset = true;
        sharedMemoryRetry.Elapsed += SharedMemoryRetry_Elapsed;


        physicsTimer = new Timer();
        physicsTimer.AutoReset = true;
        physicsTimer.Elapsed += PhysicsTimer_Elapsed;

        graphicsTimer = new Timer();
        graphicsTimer.AutoReset = true;
        graphicsTimer.Elapsed += GraphicsTimer_Elapsed;

        staticTimer = new Timer();
        staticTimer.AutoReset = true;
        staticTimer.Elapsed += StaticTimer_Elapsed;

        EverythingTimer = new Timer();
        EverythingTimer.AutoReset = true;
        EverythingTimer.Elapsed += CustomTimer_Elapsed;

        this.physicsInterval = physicsInterval;
        this.graphicsInterval = graphicsInterval;
        this.staticInterval = staticInterval;
        this.everythingInterval = everythingInterval;

        Stop();
    }

    private void CustomTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        Graphics graphics = ReadGraphics();
        Physics physics = ReadPhysics();
        StaticInfo staticInfo = ReadStaticInfo();
        OnEverythingUpdated(new EverythingEventArgs(graphics, physics, staticInfo));
    }
    private void StaticTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        StaticInfo staticInfo = ReadStaticInfo();
        OnStaticUpdated(new StaticInfoEventArgs(staticInfo));
    }

    private void GraphicsTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        Graphics graphics = ReadGraphics();
        OnGraphicsUpdated(new GraphicsEventArgs(graphics));
    }

    private void PhysicsTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        Physics physics = ReadPhysics();
        OnPhysicsUpdated(new PhysicsEventArgs(physics));
    }

    public event PhysicsUpdatedHandler PhysicsUpdated;
    public event GraphicsUpdatedHandler GraphicsUpdated;
    public event StaticUpdatedHandler StaticUpdated;
    public event EverythingUpdatedHandler EverythingUpdated;

    public virtual void OnPhysicsUpdated(PhysicsEventArgs e)
    {
        PhysicsUpdated?.Invoke(this, e);
    }

    public virtual void OnGraphicsUpdated(GraphicsEventArgs e)
    {
        GraphicsUpdated?.Invoke(this, e);
    }

    public virtual void OnStaticUpdated(StaticInfoEventArgs e)
    {
        StaticUpdated?.Invoke(this, e);
    }

    public virtual void OnEverythingUpdated(EverythingEventArgs e)
    {
        EverythingUpdated?.Invoke(this, e);
    }

    public void Start()
    {
        sharedMemoryRetry.Start();
    }

    public void Stop()
    {
        sharedMemoryRetry.Stop();

        physicsTimer.Stop();
        graphicsTimer.Stop();
        staticTimer.Stop();
        EverythingTimer.Stop();
    }

    private void SharedMemoryRetry_Elapsed(object? sender, ElapsedEventArgs e)
    {
        ConnectToSharedMemory();
        Console.WriteLine("START ACC ...");
    }

    private bool ConnectToSharedMemory()
    {
        try
        {
            physicsMMF = MemoryMappedFile.OpenExisting("Local\\acpmf_physics");
            graphicsMMF = MemoryMappedFile.OpenExisting("Local\\acpmf_graphics");
            staticMMF = MemoryMappedFile.OpenExisting("Local\\acpmf_static");

            physicsTimer.Interval = physicsInterval;
            physicsTimer.Start();

            graphicsTimer.Interval = graphicsInterval;
            graphicsTimer.Start();

            staticTimer.Interval = staticInterval;
            staticTimer.Start();

            EverythingTimer.Interval = everythingInterval;
            EverythingTimer.Start();

            sharedMemoryRetry.Stop();
            Console.WriteLine("CONNECTED SUCCESSFULLY");

            return true;

        }
        catch (FileNotFoundException)
        {
            return false;
        }
    }


    public Physics ReadPhysics()
    {

        using (var stream = physicsMMF.CreateViewStream())
        {
            using (var reader = new BinaryReader(stream))
            {
                var size = Marshal.SizeOf(typeof(Physics));
                var bytes = reader.ReadBytes(size);
                var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                var data = (Physics)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Physics));
                handle.Free();
                return data;
            }
        }
    }

    public Graphics ReadGraphics()
    {

        using (var stream = graphicsMMF.CreateViewStream())
        {
            using (var reader = new BinaryReader(stream))
            {
                var size = Marshal.SizeOf(typeof(Graphics));
                var bytes = reader.ReadBytes(size);
                var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                var data = (Graphics)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Graphics));
                handle.Free();
                return data;
            }
        }
    }

    public StaticInfo ReadStaticInfo()
    {

        using (var stream = staticMMF.CreateViewStream())
        {
            using (var reader = new BinaryReader(stream))
            {
                var size = Marshal.SizeOf(typeof(StaticInfo));
                var bytes = reader.ReadBytes(size);
                var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                var data = (StaticInfo)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(StaticInfo));
                handle.Free();
                return data;
            }
        }
    }

}
