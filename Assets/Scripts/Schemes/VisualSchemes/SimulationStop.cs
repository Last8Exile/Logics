using System;
using System.Collections;
using UnityEngine;

public class SimulationStop : Scheme
{

    public bool Enabled;

    public SimulationStop(bool enabled) : base (1)
    {
        IOGroups.Add(Input, new SchemeIOGroup(1, IO.Input));
        CycleManager.Instance.Tick += OnTick;
        Enabled = enabled;
    }

    public override void SetIO(string groupName, BitArray values, byte valStart, byte valCount, byte ioStart, byte ioCount)
    {
        if (groupName != Input)
            throw new UnityException("Неверное имя группы блока NumberDisplay");

        base.SetIO(groupName, values, valStart, valCount, ioStart, ioCount);
    }

    private void OnTick(CycleManager.TickState state)
    {
        switch (state)
        {
            case CycleManager.TickState.PreTick:
                break;
            case CycleManager.TickState.Tick:
                break;
            case CycleManager.TickState.PostTick:
                if (IOGroups[Input].IOArray[0] && Enabled)
                    CycleManager.Instance.Stop();
                break;
        }
    }

    public override void UnlinkAll()
    {
        base.UnlinkAll();
        CycleManager.Instance.Tick -= OnTick;
    }

    public const string Type = "SimulationStop";
    public const string Input = "Input";

    public const string DialogType = "SimulationStop";
    public const string DesignType = "SimulationStop";
}

[Serializable]
public class SimulationStopBuilder : SchemeBuilder
{
    public override Scheme Build(string parameters)
    {
        if (string.IsNullOrEmpty(parameters))
            parameters = true.ToString();
        return new SimulationStop(bool.Parse(parameters));
    }

    [Newtonsoft.Json.JsonIgnore]
    public override string DialogType { get { return SimulationStop.DialogType; } }

    [Newtonsoft.Json.JsonIgnore]
    public override string DesignType { get { return SimulationStop.DesignType; } }
}
