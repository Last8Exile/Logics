using UnityEngine;
using UnityEngine.UI;

public class InnerSchemeDialog : CustomDialog {

	[SerializeField] private Text mTitle = null;
	[SerializeField] private InputField mName = null;

	protected SchemeDesigner.InnerSchemeBuildParams Params = new SchemeDesigner.InnerSchemeBuildParams();
	public void SetName(string name)
	{
		Params.Name = name;
	}

	public virtual void Create()
	{
		Params.Name = mName.text;
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
