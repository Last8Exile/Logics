using System;
using System.Collections;
using UnityEngine;

public class CONST : Scheme {

	public CONST () : base (1)
	{
		IOGroups.Add(Output, new SchemeIOGroup(1, IO.Output));
		IOGroups[Output].IOArray[0] = true;
	}

	public override void SetIO(string groupName, BitArray values,byte valStart, byte valCount, byte ioStart, byte ioCount)
	{
		
		if (groupName != Output)
			throw new UnityException ("Неверное имя группы блока CONST");

		var array = IOGroups[Output].IOArray;
		if (array[0] != values[valStart]) {
			array[0] = values[valStart];
			RaiseChangedEvent(Output);
		}
	}

	public const string Type = "CONST";
	public const string Output = "Output";

	public const string DialogType = "CONST";
}

[Serializable]
public class CONSTBuilder : SchemeBuilder
{
	public override Scheme Build (string parameters)
	{
		return new CONST ();
	}

	[Newtonsoft.Json.JsonIgnore]
	public override string DialogType {
		get {
			return CONST.DialogType;
		}
	}
}