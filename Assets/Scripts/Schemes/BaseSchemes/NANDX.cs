using System.Collections;
using UnityEngine;
using System;

public class NANDX : Scheme {

	public NANDX (string parameters) : base (3)
	{
		var size = byte.Parse(parameters);
		IOGroups.Add(Input, new SchemeIOGroup(size, IO.Input));
		IOGroups.Add(Input2, new SchemeIOGroup(size, IO.Input));
		IOGroups.Add(Output, new SchemeIOGroup(size, IO.Output));
		IOGroups[Output].IOArray.SetAll(true);
	}

	public override void SetIO(string groupName, BitArray values,byte valStart, byte valCount, byte ioStart, byte ioCount)
	{
		if (groupName != Input && groupName != Input2)
			throw new UnityException ("Неверное имя группы блока NANDX");

	    base.SetIO(groupName, values, valStart, valCount, ioStart, ioCount);

		var inputArray1 = IOGroups[Input].IOArray;
		var inputArray2 = IOGroups[Input2].IOArray;
	    var outputArray = IOGroups[Output].IOArray;

        var oldValue = new BitArray(outputArray);
		var newValue = new BitArray(inputArray1).And(inputArray2).Not();

		if (oldValue.Xor(newValue).Any((x)=>x)) 
		{
			IOGroups[Output].IOArray = newValue;
			RaiseChangedEvent (Output);
		}
	}

	public const string Type = "NANDX";
	public const string Input = "Input1";
	public const string Input2 = "Input2";
	public const string Output = "Output";

	public const string DialogType = "NANDX";
}

[Serializable]
public class NANDXBuilder : SchemeBuilder
{
	public override Scheme Build (string parameters)
	{
		return new NANDX (parameters);
	}

	[Newtonsoft.Json.JsonIgnore]
	public override string DialogType {
		get {
			return NANDX.DialogType;
		}
	}
}