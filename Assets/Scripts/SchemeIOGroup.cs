using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchemeIOGroup {

	public BitArray IOArray;
	public event Action IOChanged;
	public IO IO;

	public void IOChanged_Invoke()
	{
		if (IOChanged != null)
			IOChanged.Invoke();
	}

	public SchemeIOGroup(byte size, IO io)
	{
		IOArray = new BitArray(size);
		IO = io;
	}

	public byte Size 
	{
		get { return (byte)IOArray.Count; }
		private set { IOArray = new BitArray(value); }
	}

	public void SetIO(BitArray values, byte fromStart, byte fromCount, byte toStart, byte toCount)
	{

		CheckParams (values, fromStart, fromCount, toStart, toCount);

		var oldInput = (BitArray) IOArray.Clone ();
		for (var i = 0; i < toCount; i++) 
		{
			IOArray[i + toStart] = fromCount == 1 ? values[fromStart] : values[i + fromStart];
		}
		if (IOChanged!=null && oldInput.Xor (IOArray).Any (x => x)) 
		{
			IOChanged.Invoke();
		}
	}

	private void CheckParams(BitArray values,byte valStart, byte valCount, byte ioStart, byte ioCount)
	{
		if (valCount == 0 || ioCount == 0) 
		{
			throw new UnityException ("Ошибка в параметрах вызова Scheme.SetInputs(). Размер данных не может быть нулевым");
		}
		if (ioStart+ioCount > IOArray.Count || valStart+valCount > values.Count)
		{
			throw new UnityException ("Ошибка в параметрах вызова Scheme.SetInputs(). Выход за границы массива");
		}
		if ((valCount) != 1 && valCount != ioCount) 
		{
			throw new UnityException ("Ошибка в параметрах вызова Scheme.SetInputs(). Размеры входа и выхода не совпадают");
		}
	}

	public void UnlinkAll()
	{
		if (IOChanged != null)
			IOChanged.GetInvocationList().ForEach((x) => IOChanged -= (Action)x);
	}

}
