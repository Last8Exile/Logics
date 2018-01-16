using System;
using UnityEngine;
using UnityEngine.UI;

public class CycleManager : MonoBehaviour {

	public static CycleManager Instance 
	{
		get { 
			if (mInstance == null) {
				mInstance = GameObject.Find ("CycleManager").GetComponent<CycleManager> ();
				mInstance.Init ();
			}
			return mInstance;
		}
	}
	private static CycleManager mInstance;

	public void Init ()
	{
		
	}

	[SerializeField] private Text mStartStopButtonText = null;
	[SerializeField] private InputField mPeriodField = null;
	[SerializeField] private RectTransform mProgressBar = null;
	private bool mEnabled;
	private float mPeriod;
	private float mLastTickTime;
	 
	public enum TickState
	{
		PreTick,
		Tick,
		PostTick
	}
	public event Action<TickState> Tick;

	public void RaiseTick()
	{
		if (Tick != null)
		{
			Tick.Invoke(TickState.PreTick);
			Tick.Invoke(TickState.Tick);
			Tick.Invoke(TickState.PostTick);
		}
	}

	void Update () 
	{
		if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.C))
		{
			Toggle();
			return;
		}
		if (Time.time - mLastTickTime > mPeriod)
		{
			mLastTickTime = Time.time;
			mProgressBar.anchorMax = new Vector2(1, 1);
			RaiseTick();
		}
		else
		{
			mProgressBar.anchorMax = new Vector2((Time.time - mLastTickTime) / mPeriod, 1);
		}
	}

	public void Toggle()
	{
		if (mEnabled)
		{
			mStartStopButtonText.text = "Страт";
			mProgressBar.anchorMax = new Vector2(0, 1);
			mEnabled = false;
			enabled = false;
		}
		else
		{
			if (!float.TryParse(mPeriodField.text, out mPeriod) || mPeriod <= 0)
				return;
			mStartStopButtonText.text = "Стоп";
			mLastTickTime = Time.time;
			mEnabled = true;
			enabled = true;
		}
	}
}
