using UnityEngine;
using UnityEngine.UI;

public class ConfirmDialog : CustomDialog {

	[SerializeField] private Text mTitle = null, mLabel = null;

	public void Ok()
	{
		DialogResult = DialogResult.Ok;
	}

	public void Cancel()
	{
		DialogResult = DialogResult.Cancel;
	}

	public override object Result {
		get {
			return null;
		}
	}

	public void ShowDialog(string title, string label)
	{
		mTitle.text = title;
		mLabel.text = label;
		gameObject.SetActive(true);
	}
}
