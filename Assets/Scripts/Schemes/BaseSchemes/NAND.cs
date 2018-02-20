using System.Collections;
using UnityEngine;
using System;

public class NAND : Scheme {

	public NAND () : base (2)
	{
		IOGroups.Add(Input, new SchemeIOGroup(2, IO.Input));
		IOGroups.Add(Output, new SchemeIOGroup(1, IO.Output));
		IOGroups[Output].IOArray[0] = true;
	}

	public override void SetIO(string groupName, BitArray values,byte valStart, byte valCount, byte ioStart, byte ioCount)
	{
		if (groupName != Input)
			throw new UnityException ("Неверное имя группы блока NAND");

	    base.SetIO(groupName, values, valStart, valCount, ioStart, ioCount);

        var inputArray = IOGroups[Input].IOArray;
		var outputArray = IOGroups[Output].IOArray;

		var oldValue = outputArray[0];
		outputArray[0] = !(inputArray[0] && inputArray[1]);

		if (oldValue != outputArray[0]) 
		{
			RaiseChangedEvent (Output);
		}
	}

	public const string Type = "NAND";
	public const string Input = "Inputs";
	public const string Output = "Output";

	public const string DialogType = "NAND";
}

[Serializable]
public class NANDBuilder : SchemeBuilder
{
	public override Scheme Build (string parameters)
	{
		return new NAND ();
	}

	[Newtonsoft.Json.JsonIgnore]
	public override string DialogType {
		get {
			return NAND.DialogType;
		}
	}
}