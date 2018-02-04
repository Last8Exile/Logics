using UnityEngine;
using UnityEngine.UI;

public class RAMXDialog : InnerSchemeDialog {

	[SerializeField] private InputField mValueSize = null, mAddressSize = null;

	public override void Create()
	{
		byte valueSize, addressSize;
		if (byte.TryParse(mValueSize.text, out valueSize) && valueSize > 0 && byte.TryParse(mAddressSize.text, out addressSize) && addressSize > 0)
		{
			Params.Parameters = mValueSize.text + " " + mAddressSize.text;
			base.Create();
		}
	}

}
