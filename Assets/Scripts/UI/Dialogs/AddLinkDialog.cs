using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AddLinkDialog : CustomDialog {


	[SerializeField] private Text mTitle = null;
	[SerializeField] private InputField mSourceStart = null, mSourceCount = null, mTargetStart = null, mTargetCount = null;
	[SerializeField] private Dropdown mSourceGroupName = null, mTargetGroupName = null;

	public SchemeDesigner.LinkBuildParams Params = new SchemeDesigner.LinkBuildParams();

	public void Create()
	{
		Params.SourceGroupName = mSourceGroupName.captionText.text;
		Params.TargetGroupName = mTargetGroupName.captionText.text;
		Params.SourceStart = byte.Parse(mSourceStart.text);
		Params.SourceCount = byte.Parse(mSourceCount.text);
		Params.TargetStart = byte.Parse(mTargetStart.text);
		Params.TargetCount = byte.Parse(mTargetCount.text);
		DialogResult = DialogResult.Ok;
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
		
	public void ShowDialog(string title,
		UIScheme.IOGroupContainer mSourceContainerGroup, UIScheme.InnerContainer mSourceContainerInner,
		UIScheme.IOGroupContainer mTargetContainerGroup, UIScheme.InnerContainer mTargetContainerInner)
	{
		mTitle.text = title;

		mSourceStart.text = "0";
		mTargetStart.text = "0";

		if (mSourceContainerGroup != null)
		{
			mSourceGroupName.options = new List<Dropdown.OptionData>() { new Dropdown.OptionData(mSourceContainerGroup.BuildInfo.BuildString.Name) };
			mSourceGroupName.interactable = false;
			mSourceCount.text = mSourceContainerGroup.BuildInfo.BuildString.Size.ToString();
		}
		if (mSourceContainerInner != null)
		{
			var ioGroups = mSourceContainerInner.Scheme.IOGroups.Where((x) => x.Value.IO == IO.Output);
			mSourceGroupName.options = ioGroups.Select((x) => new Dropdown.OptionData(x.Key)).ToList();
			if (ioGroups.Count() > 0)
				mSourceCount.text = ioGroups.First().Value.Size.ToString();
		}
		if (mTargetContainerGroup != null)
		{
			mTargetGroupName.options = new List<Dropdown.OptionData>() { new Dropdown.OptionData(mTargetContainerGroup.BuildInfo.BuildString.Name) };
			mTargetGroupName.interactable = false;
			mTargetCount.text = mTargetContainerGroup.BuildInfo.BuildString.Size.ToString();
		}
		if (mTargetContainerInner != null)
		{
			var ioGroups = mTargetContainerInner.Scheme.IOGroups.Where((x) => x.Value.IO == IO.Input);
			mTargetGroupName.options = ioGroups.Select((x) => new Dropdown.OptionData(x.Key)).ToList();
			if (ioGroups.Count() > 0)
				mTargetCount.text = ioGroups.First().Value.Size.ToString();
		}
		gameObject.SetActive(true);
	}

}
