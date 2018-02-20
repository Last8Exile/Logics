using System.Collections;
using UnityEngine;
using System;

public class NOTX : Scheme {

	public NOTX (string parameters) : base (2)
	{
		var size = byte.Parse(parameters);
		IOGroups.Add(Input, new SchemeIOGroup(size, IO.Input));
		IOGroups.Add(Output, new SchemeIOGroup(size, IO.Output));
		IOGroups[Output].IOArray.SetAll(true);
	}

	public override void SetIO(string groupName, BitArray values,byte valStart, byte valCount, byte ioStart, byte ioCount)
	{
		if (groupName != Input)
			throw new UnityException ("Неверное имя группы блока NOTX");

	    base.SetIO(groupName, values, valStart, valCount, ioStart, ioCount);

        var inputArray = IOGroups[Input].IOArray;
		var outputArray = IOGroups[Output].IOArray;

		var oldValue = new BitArray(outputArray);
		var newValue = new BitArray(inputArray).Not();

		if (oldValue.Xor(newValue).Any((x)=>x)) 
		{
			IOGroups[Output].IOArray = newValue;
			RaiseChangedEvent (Output);
		}
	}

	public const string Type = "NOTX";
	public const string Input = "Input";
	public const string Output = "Output";

	public const string DialogType = "NOTX";
}

[Serializable]
public class NOTXBuilder : SchemeBuilder
{
	public override Scheme Build (string parameters)
	{
		return new NOTX (parameters);
	}

	[Newtonsoft.Json.JsonIgnore]
	public override string DialogType {
		get {
			return NOTX.DialogType;
		}
	}
}