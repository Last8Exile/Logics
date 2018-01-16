using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IOLook : IOBase {

	[SerializeField] private Image mImage = null;
	[SerializeField] private Color mFalseColor = Color.red, mTrueColor = Color.green;
	[SerializeField] private Transform mConnectionPos = null;

	private Scheme mScheme;
	private byte mNumber;
	private string mIOGroup;

	public void Init(Scheme scheme, byte number, string ioGroupName)
	{
		mScheme = scheme;
		mNumber = number;
		mIOGroup = ioGroupName;
		gameObject.name = number.ToString();

		var ioGroup = mScheme.IOGroups[mIOGroup];
		ioGroup.IOChanged += OnValueChanged;
		mConnectionPos.localPosition = (ioGroup.IO == IO.Input ? Vector3.left : Vector3.right) * 17;
		OnValueChanged();

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
