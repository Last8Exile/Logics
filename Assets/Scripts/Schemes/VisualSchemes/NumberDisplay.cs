using System;
using System.Collections;
using UnityEngine;

public class NumberDisplay : Scheme
{

    private Parameters mSchemeParameters;

    public int Number { get; private set; }

    public NumberDisplay(Parameters parameters) : base (1)
    {
        mSchemeParameters = parameters;
        IOGroups.Add(Input, new SchemeIOGroup(mSchemeParameters.Size, IO.Input));
    }

    public override void SetIO(string groupName, BitArray values, byte valStart, byte valCount, byte ioStart, byte ioCount)
    {
        if (groupName != Input)
            throw new UnityException("Неверное имя группы блока NumberDisplay");

        var inputArray = IOGroups[groupName].IOArray;

        for (var i = 0; i < ioCount; i++)
        {
            inputArray[i + ioStart] = valCount == 1 ? values[valStart] : values[i + valStart];
        }
        Number = mSchemeParameters.Signed ? inputArray.ToIntSigned() : inputArray.ToInt();
        RaiseChangedEvent(groupName);
    }

    public const string Type = "NumberDisplay";
    public const string Input = "Input";

    public const string DialogType = "NumberDisplay";
    public const string DesignType = "NumberDisplay";

    [Serializable]
    public struct Parameters
    {
        public byte Size;
        public bool Signed;
    }
}

[Serializable]
public class NumberDisplayBuilder : SchemeBuilder
{
    public override Scheme Build(string parameters)
    {
        return new NumberDisplay(MyJsonSerializer.Deserialize<NumberDisplay.Parameters>(parameters));
    }

    [Newtonsoft.Json.JsonIgnore]
    public override string DialogType { get { return NumberDisplay.DialogType; } }

    [Newtonsoft.Json.JsonIgnore]
    public override string DesignType { get { return NumberDisplay.DesignType; } }
}
