using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RAMX : Scheme {
	
	public RAMX (string parameters) : base (4)
	{
		var sizes = parameters.Split(' ');
		var valueSize = byte.Parse(sizes[0]);
		var addressSize = byte.Parse(sizes[1]);
		var valuesTotal = Extensions.Pow(2, addressSize);
		var totalSize = valuesTotal * valueSize;
		if (totalSize > MaxSize)
			throw new UnityException("Превышен максимальный размер RAM");
		IOGroups.Add(Input, new SchemeIOGroup(valueSize, IO.Input));
		IOGroups.Add(Address, new SchemeIOGroup(addressSize, IO.Input));
		IOGroups.Add(Load, new SchemeIOGroup(1, IO.Input));
		IOGroups.Add(Output, new SchemeIOGroup(valueSize, IO.Output));
		CycleManager.Instance.Tick += OnTick;
		mRam = new BitArray[valuesTotal];
		for (int i = 0; i < valuesTotal; i++)
			mRam[i] = new BitArray(valueSize);
	}

	private bool mTickStarted;
	private Dictionary<string,BitArray> mLastValue = new Dictionary<string,BitArray>(3);

	private BitArray[] mRam;

	public override void SetIO(string groupName, BitArray values,byte valStart, byte valCount, byte ioStart, byte ioCount)
	{
		if (groupName != Input && groupName != Load && groupName != Address)
			throw new UnityException ("Неверное имя группы блока RAMX");

        var lastValue = new BitArray(IOGroups[groupName].IOArray);
		for (var i = 0; i < ioCount; i++) 
		{
		    lastValue[i + ioStart] = valCount == 1 ? values[valStart] : values[i + valStart];
		}
	    mLastValue[groupName] = lastValue;

        if (mTickStarted)
			return;

	    IOGroups[groupName].IOArray = lastValue;
		RaiseChangedEvent(groupName);

		if (groupName == Address)
			CheckAdress();
	}

	private void CheckAdress()
	{
		int address = CalckAdress(IOGroups[Address].IOArray);

		var oldValue = new BitArray(IOGroups[Output].IOArray);
		var newValue = mRam[address];
		if (oldValue.Xor(newValue).Any((x) => x))
		{
			IOGroups[Output].IOArray = newValue;
			RaiseChangedEvent(Output);
		}
	}

	private int CalckAdress(BitArray array)
	{
		int address = 0;
		for (int i = 0; i < array.Count; i++)
			address += (array[i] ? 1 : 0) * Extensions.Pow(2, i);
		return address;
	}

	private void OnTick(CycleManager.TickState state)
	{
		switch (state)
		{
			case CycleManager.TickState.PreTick:
				mTickStarted = true;
				break;
			case CycleManager.TickState.Tick:
				var load = IOGroups[Load].IOArray[0];
				if (!load)
					return;

				var address = CalckAdress(IOGroups[Address].IOArray);
				mRam[address] = IOGroups[Input].IOArray;

				var oldValue = new BitArray(IOGroups[Output].IOArray);
				var newValue = mRam[address];
				if (oldValue.Xor(newValue).Any((x) => x))
				{
					IOGroups[Output].IOArray = newValue;
					RaiseChangedEvent(Output);
				}
				break;
			case CycleManager.TickState.PostTick:
				mTickStarted = false;
				if (mLastValue.ContainsKey(Input))
				{
					IOGroups[Input].IOArray = mLastValue[Input];
					RaiseChangedEvent(Input);
				}
				if (mLastValue.ContainsKey(Address))
				{
					IOGroups[Address].IOArray = mLastValue[Address];
					RaiseChangedEvent(Address);
				}
				if (mLastValue.ContainsKey(Load))
				{
					IOGroups[Load].IOArray = mLastValue[Load];
					RaiseChangedEvent(Load);
				}
				CheckAdress();
				break;
		}
	}

	public void LoadValue(BitArray value, int address)
	{
		mRam[address] = value;
		if (address == CalckAdress(IOGroups[Address].IOArray))
			CheckAdress();
	}

	public override void UnlinkAll()
	{
		base.UnlinkAll();
		CycleManager.Instance.Tick -= OnTick;
	}

	public const string Type = "RAMX";
	public const string Input = "In";
	public const string Load = "Load";
	public const string Address = "Addr";
	public const string Output = "Out";

	public const string DialogType = "RAMX";

	public const int MaxSize = 1048576;
}
[Serializable]
public class RAMXBuilder : SchemeBuilder
{
	public override Scheme Build (string parameters)
	{
		return new RAMX (parameters);
	}

	[Newtonsoft.Json.JsonIgnore]
	public override string DialogType {
		get {
			return RAMX.DialogType;
		}
	}
}