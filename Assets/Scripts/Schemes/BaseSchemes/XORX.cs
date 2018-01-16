﻿using System.Collections;
using UnityEngine;
using System;

public class XORX : Scheme {

	public XORX (string parameters) : base (3)
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
			throw new UnityException ("Неверное имя группы блока XORX");

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
		var newValue = new BitArray(inputArray1).Xor(inputArray2);

		if (oldValue.Xor(newValue).Any((x)=>x)) 
		{
			IOGroups[Output].IOArray = newValue;
			RaiseChangedEvent (Output);
		}
	}

	public const string Type = "XORX";
	public const string Input = "Input1";
	public const string Input2 = "Input2";
	public const string Output = "Output";

	public const string DialogType = "XORX";
}

[Serializable]
public class XORXBuilder : SchemeBuilder
{
	public override Scheme Build (string parameters)
	{
		return new XORX (parameters);
	}

	[Newtonsoft.Json.JsonIgnore]
	public override string DialogType {
		get {
			return XORX.DialogType;
		}
	}
}