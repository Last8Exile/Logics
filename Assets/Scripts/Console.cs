using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class Console : MonoBehaviour {

	[SerializeField] private Text mText = null;

	public static Console Instance 
	{
		get { 
			if (mInstance == null) {
				mInstance = GameObject.Find ("Console").GetComponent<Console> ();
				mInstance.Init ();
			}
			return mInstance;
		}
	}
	private static Console mInstance;

	public void Init ()
	{
		
	}

	public int MaxLines;
	public int ClearAmount;
	private int mCurrentLines;
	public bool Enabled = true;

	public void Log(string message)
	{
		if (!Enabled)
			return;
		mCurrentLines += 2;
		message.ForEach((c) => {
			if (c == '\n')
				mCurrentLines++;
		});
		mText.text += "\n----------------\n" + message;
		ClearOld();
	}

	public void LogShort(string message)
	{
		if (!Enabled)
			return;
		mCurrentLines++;
		message.ForEach((c) => {
			if (c == '\n')
				mCurrentLines++;
		});
		mText.text += "\n" + message;
		ClearOld();
	}

	private void ClearOld()
	{
		if (mCurrentLines >= MaxLines)
		{
			var nextLines = 0;
			for (int i = 0; i < mText.text.Length; i++)
			{
				if (mCurrentLines - nextLines < MaxLines - ClearAmount)
				{
					mText.text = mText.text.Substring(i);
					mCurrentLines -= nextLines;
					return;
				}
				if (mText.text[i] == '\n')
				{
					nextLines++;
				}	
			}
		}
	}

	public void Log<T>(IEnumerable collection, Func<T,string> func)
	{
		foreach (T item in collection)
			Log (func (item));
	}

	public void Clear()
	{
		mText.text = "";
	    mCurrentLines = 0;
	}
}
