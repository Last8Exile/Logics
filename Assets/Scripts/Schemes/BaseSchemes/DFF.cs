using System;
using System.Collections;
using UnityEngine;

public class DFF : Scheme {

	public DFF () : base (2)
	{
		IOGroups.Add(Input, new SchemeIOGroup(1, IO.Input));
		IOGroups.Add(Output, new SchemeIOGroup(1, IO.Output));
		CycleManager.Instance.Tick += OnTick;
	}

	private bool mTickStarted;
	private bool mLastValue;

	public override void SetIO(string groupName, BitArray values,byte valStart, byte valCount, byte ioStart, byte ioCount)
	{
		if (groupName != Input)
			throw new UnityException ("Неверное имя группы блока DFF"); 

		mLastValue = values[valStart];
		if (mTickStarted)
			return;

		IOGroups[Input].IOArray[0] = mLastValue;
		RaiseChangedEvent (Input);
	}

	private void OnTick(CycleManager.TickState state)
	{
		switch (state)
		{
			case CycleManager.TickState.PreTick:
				mTickStarted = true;
				break;
			case CycleManager.TickState.Tick:
				var inputArray = IOGroups[Input].IOArray;
				var outputArray = IOGroups[Output].IOArray;

				var oldValue = outputArray[0];
				outputArray[0] = inputArray[0];

				if (oldValue != outputArray[0]) 
				{
					RaiseChangedEvent (Output);
				}
				break;
			case CycleManager.TickState.PostTick:
				mTickStarted = false;
				IOGroups[Input].IOArray[0] = mLastValue;
				RaiseChangedEvent (Input);
				break;
		}
	}

	public override void UnlinkAll()
	{
		base.UnlinkAll();
		CycleManager.Instance.Tick -= OnTick;
	}

	public const string Type = "DFF";
	public const string Input = "Input";
	public const string Output = "Output";

	public const string DialogType = "DFF";
}

[Serializable]
public class DFFBuilder : SchemeBuilder
{
	public override Scheme Build (string parameters)
	{
		return new DFF ();
	}

	[Newtonsoft.Json.JsonIgnore]
	public override string DialogType {
		get {
			return DFF.DialogType;
		}
	}
}
