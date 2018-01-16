using UnityEngine;
using UnityEngine.UI;
using System;

public class NewSchemeDialog : CustomDialog {

	[SerializeField] private InputField mName = null;

	public UISelfSchemeBuildInfo BuildInfo = new UISelfSchemeBuildInfo ();

	public void Create()
	{
		BuildInfo.Name = mName.text;
		DialogResult = DialogResult.Ok;
	}

	public void Cancel()
	{
		DialogResult = DialogResult.Cancel;
	}

	public override object Result {
		get {
			return BuildInfo;
		}
	}

	public void ShowDialog(string title)
	{
		throw new NotImplementedException();
	}
}