using System;
using System.Collections;
using UnityEngine;

public class IntConst : Scheme
{

    public int Number
    {
        get { return mNumber; }
        set
        {
            mNumber = value;
            base.SetIO(Output, Extensions.FromInt(value, 32), 0, 32, 0, 32);
        }
    }
    private int mNumber;

    public IntConst(int value) : base (1)
    {
        IOGroups.Add(Output, new SchemeIOGroup(32, IO.Output));
        Number = value;
    }

    public override void SetIO(string groupName, BitArray values, byte valStart, byte valCount, byte ioStart, byte ioCount)
    {
        if (groupName != Output)
            throw new UnityException("Неверное имя группы блока NumberConst");

        base.SetIO(groupName, values, valStart, valCount, ioStart, ioCount);
    }

    public const string Type = "IntConst";
    public const string Output = "Output";

    public const string DialogType = "IntConst";
    public const string DesignType = "IntConst";
}

[Serializable]
public class IntConstBuilder : SchemeBuilder
{
    public override Scheme Build(string parameters)
    {
        return new IntConst(string.IsNullOrEmpty(parameters) ? 0 : int.Parse(parameters));
    }

    [Newtonsoft.Json.JsonIgnore]
    public override string DialogType { get { return IntConst.DialogType; } }

    [Newtonsoft.Json.JsonIgnore]
    public override string DesignType { get { return IntConst.DesignType; } }
}
