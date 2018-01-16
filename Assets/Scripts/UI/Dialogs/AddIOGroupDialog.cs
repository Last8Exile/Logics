using UnityEngine;
using UnityEngine.UI;

public class AddIOGroupDialog : CustomDialog {

	[SerializeField] private Text mTitle = null;
	[SerializeField] private InputField mName = null, mSize = null;
	[SerializeField] private Dropdown mIOSelector = null;

	private IOGroupBuildString Params = new IOGroupBuildString();

	public void Create()
	{
		Params.Name = mName.text;
		Params.IO = mIOSelector.value == 0 ? IO.Input : IO.Output;
		if (byte.TryParse(mSize.text, out Params.Size))
		if (!string.IsNullOrEmpty(Params.Name))
		{
			DialogResult = DialogResult.Ok;
		}
	}

	public void Cancel()
	{
		DialogResult = DialogResult.Cancel;
	}

	public override object Result {
		get {
			return Params;
		}
	}

	public void ShowDialog(string title)
	{
		mTitle.text = title;
		gameObject.SetActive(true);
	}
}
