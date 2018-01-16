using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IOToggler : IOBase {

	[SerializeField] private Image mImage = null;
	[SerializeField] private Color mFalseColor = Color.red, mTrueColor = Color.green;
	[SerializeField] private Transform mConnectionPos = null;
	private Scheme mScheme;
	private byte mNumber;
	private string mIOGroup;

	public void Init(Scheme scheme, byte number, string ioGroup, IO io)
	{
		mScheme = scheme;
		mNumber = number;
		mIOGroup = ioGroup;
		gameObject.name = number.ToString ();

		mScheme.IOGroups[mIOGroup].IOChanged += OnValueChanged;
		mConnectionPos.localPosition = (io == IO.Input ? Vector3.right : Vector3.left) * 25;

		if (io == IO.Output) 
		{
			Destroy(GetComponent<EventTrigger>());
		}
			
	}

	public void OnMouseClick()
	{
		var value = !mScheme.IOGroups[mIOGroup].IOArray[mNumber];
		mScheme.SetIO(mIOGroup, new BitArray(1, value), 0, 1, mNumber, 1);
	}

	private void OnValueChanged()
	{
		mImage.color = mScheme.IOGroups[mIOGroup].IOArray[mNumber] ? mTrueColor : mFalseColor;
	}

	public override Transform ConnectionPosition {
		get {
			return mConnectionPos;
		}
	}
}
