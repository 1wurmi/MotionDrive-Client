﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionDrive.Recorder.iRacingRecorder.BitFields;
public abstract class BitfieldBase<T>
       where T : struct, IConvertible, IComparable
{
    protected BitfieldBase(int value)
    {
        _Value = (uint)value;
    }

    private readonly uint _Value;
    public uint Value { get { return _Value; } }

    public bool Contains(T bit)
    {
        var bitValue = (uint)Convert.ChangeType(bit, bit.GetTypeCode());
        return (this.Value & bitValue) == bitValue;
    }
}
