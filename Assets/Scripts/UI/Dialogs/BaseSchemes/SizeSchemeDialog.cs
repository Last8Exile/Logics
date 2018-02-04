using UnityEngine;
using UnityEngine.UI;

public class SizeSchemeDialog : InnerSchemeDialog {

	[SerializeField] private InputField mSize = null;

	public override void Create()
	{
		byte size;
		if (byte.TryParse(mSize.text, out size) && size > 0)
		{
			Params.Parameters = mSize.text;
			base.Create();
		}
	}

}
