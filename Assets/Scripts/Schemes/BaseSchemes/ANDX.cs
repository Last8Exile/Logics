using System.Collections;
using UnityEngine;
using System;

public class ANDX : Scheme {

	public ANDX (string parameters) : base (3)
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
			throw new UnityException ("Неверное имя группы блока NAND");

		var inputArray = IOGroups[groupName].IOArray;
		var outputArray = IOGroups[Output].IOArray;

		for (var i = 0; i < ioCount; i++) 
		{
			inputArray[i + ioStart] = valCount == 1 ? values[valStart] : values[i + valStart];
		}
		RaiseChangedEvent (groupName);

		var inputArray1 = IOGroups[Input].IOArray;
		var inputArray2 = IOGroups[Input2].IOArray;

		var oldValue = new BitArray(outputArray);
		var newValue = new BitArray(inputArray1).And(inputArray2);

		if (oldValue.Xor(newValue).Any((x)=>x)) 
		{
			IOGroups[Output].IOArray = newValue;
			RaiseChangedEvent (Output);
		}
	}

	public const string Type = "ANDX";
	public const string Input = "Input1";
	public const string Input2 = "Input2";
	public const string Output = "Output";

	public const string DialogType = "ANDX";
}

[Serializable]
public class ANDXBuilder : SchemeBuilder
{
	public override Scheme Build (string parameters)
	{
		return new ANDX (parameters);
	}

	[Newtonsoft.Json.JsonIgnore]
	public override string DialogType {
		get {
			return ANDX.DialogType;
		}
	}
}