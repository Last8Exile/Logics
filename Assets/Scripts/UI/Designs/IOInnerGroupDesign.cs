using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IOInnerGroupDesign : IOGroupDesign {

	[SerializeField] private Transform mIOContainer = null;
	[SerializeField] private GameObject mIOPrefab = null;
	[SerializeField] private Text mName = null;

	private IOLook[] mIOLooks;

	public void Init(string name, SchemeIOGroup ioGroup, Scheme parentScheme)
	{
		gameObject.name = name + " (" + ioGroup.Size.ToString() + ")";
		mName.text = name.ToRageChatNotation();

		if (mIOLooks != null)
			foreach (var input in mIOLooks)
				Destroy (input.gameObject);

		mIOLooks = new IOLook[ioGroup.Size];

		for (byte i = 0; i < ioGroup.Size; i++) 
		{
			mIOLooks[i] = Instantiate (mIOPrefab, mIOContainer).GetComponent<IOLook> ();
			mIOLooks[i].Init(parentScheme, i, name);
		}
	}

	public override IOBase IOBase(byte number)
	{
		return mIOLooks[number];
	}
}
