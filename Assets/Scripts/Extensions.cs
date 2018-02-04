using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class Extensions {

	public static void RemoveChilds(this Transform transform)
	{
		foreach (Transform child in transform)
			GameObject.Destroy (child.gameObject);
	}

	public static void SortChilds(this Transform transform)
	{
		var list = new List<Transform>(transform.childCount);
		foreach (Transform child in transform)
			list.Add(child);
		transform.DetachChildren();
		list.OrderBy((x) => x.gameObject.name).ForEach((x) => x.SetParent(transform));
	}

	public static bool Any(this BitArray array, Func<bool,bool> predicate)
	{
		for (int i = 0; i < array.Count; i++) 
		{
			if (predicate(array[i]))
				return true;
		}
		return false;
	}

	public static string Print(this BitArray array)
	{
		string str = "";
		for (int i=0;i<array.Count;i++)
			str = str + (array[i] ? '1' : '0');
		return str;
	}

	public static Vector3 ToVector3(this Vector2 vector, float value = 0)
	{
		return new Vector3 (vector.x, vector.y, value);
	}

	public static Vector2 ToVector2(this Vector3 vector, Axis sacrifise = Axis.z)
	{
		switch (sacrifise) 
		{
			case Axis.x:
				return new Vector2(vector.y, vector.z);
			case Axis.y:
				return new Vector2(vector.x, vector.z);
			case Axis.z:
				return new Vector2(vector.x, vector.y);
		}
		throw new UnityException();
	}

    public static Vector2 InverseY(this Vector2 vector)
    {
        return new Vector2(vector.x, -vector.y);
    }

    public static Vector2 Clamp(this Vector2 vector, Vector2 min, Vector2 max)
    {
        return new Vector2(Mathf.Clamp(vector.x, min.x, max.x), Mathf.Clamp(vector.y, min.y, max.y));
    }

    public static Vector2 ClampMin(this Vector2 vector, Vector2 min)
    {
        return Clamp(vector, min, Vector2.positiveInfinity);
    }

    public static Vector2 ClampMax(this Vector2 vector, Vector2 max)
    {
        return Clamp(vector, Vector2.negativeInfinity, max);
    }

    public static Vector3 ScreenToWorldPos(Vector2 point)
	{
		return Camera.main.ScreenToWorldPoint(point.ToVector3());
	}

	public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
	{
		foreach (var item in collection)
			action(item);
	}

	public static BitArray Array(params bool[] values)
	{
		return new BitArray (values);
	}

	public static string ToRageChatNotation(this string str)
	{
		if (string.IsNullOrEmpty(str))
			return "";
		string newStr = str[0].ToString();
		for (int i = 1; i < str.Length; i++)
			newStr += "\n" + str[i];
		return newStr;
	}

	public static bool HasFlag(this Enum variable, Enum value)
	{
		UInt16 num = Convert.ToUInt16(value);
		return (Convert.ToUInt16(variable) & num) == num;
	}

	public static int Pow(int value, int degree)
	{
		if (degree < 0)
			throw new ArgumentOutOfRangeException("degree", "degree должен быть не меньше нуля");
		if (degree == 0)
			return 1;
		if (degree == 1)
			return value;
		
		var x = Pow(value,degree / 2);
		if (degree % 2 == 1)
			return value * x * x;
		return x * x;
	}

	public static BitArray FromINT(int number,int size)
	{
		var array = new BitArray(size);
		for (int i = 0; i < size; i++)
		{
			array[i] = number % 2 == 1;
			number /= 2;
		}
		return new BitArray(array);
	}

	public static BitArray FromHEX(string str)
	{
		if (str.Length != 4)
			throw new ArgumentException("str", "строка должна состоять из 4-ёх символов");
		var array = new byte[2];
		array[0] = Convert.ToByte("" + str[2] + str[3], 16);
		array[1] = Convert.ToByte("" + str[0] + str[1], 16);

		return new BitArray(array);
	}

	public static int ToInt(this BitArray array)
	{
		var number = 0;
		for (int i = 0; i < array.Length; i++)
			number += ((array[i] ? 1 : 0) * Extensions.Pow(2, i));
		return number;
	}

    public static int ToIntSigned(this BitArray array)
    {
        var number = 0;
        var neg = array[array.Length - 1];
        if (neg)
            array = (array.Clone() as BitArray).Not();
        for (int i = 0; i < array.Length-1; i++)
        {
            number += ((array[i] ? 1 : 0) * Extensions.Pow(2, i));
        }
        if (neg)
            number = -(number + 1);

        return number;
    }

    public static string ToHex(this BitArray array)
	{
		if (array.Length != 16)
			throw new ArgumentException("str", "массив должнен состоять из 16-ти бит");
		var str = "";
		byte first = 0;
		for (int i = 0; i < 4; i++)
			first += (byte)((array[i] ? 1 : 0) * Extensions.Pow(2, i));
		byte second = 0;
		for (int i = 4; i < 8; i++)
			second += (byte)((array[i] ? 1 : 0) * Extensions.Pow(2, i - 4));
		byte third = 0;
		for (int i = 8; i < 12; i++)
			third += (byte)((array[i] ? 1 : 0) * Extensions.Pow(2, i - 8));
		byte fourth = 0;
		for (int i = 12; i < 16; i++)
			fourth += (byte)((array[i] ? 1 : 0) * Extensions.Pow(2, i - 12));
		str += fourth.ToString("X");
		str += third.ToString("X");
		str += second.ToString("X");
		str += first.ToString("X");
		return str;
	}
}

public enum Axis
{
	x,
	y,
	z
}
