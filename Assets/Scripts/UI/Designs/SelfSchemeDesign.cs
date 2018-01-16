using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelfSchemeDesign : SchemeDesign {

	[SerializeField] private Text mName = null;


	private UIScheme.SelfContainer mContainer;


	public void Init(UIScheme.SelfContainer container)
	{
		mContainer = container;
		gameObject.name = "Core: " + mContainer.SelfBuildInfo.Name;
		mName.text = mContainer.SelfBuildInfo.Name;
	}

	public void DestroyThis()
	{
		Destroy(gameObject);
	}

	public override UIScheme.SchemeContainer SchemeContainer {
		get {
			return mContainer;
		}
	}

	public override IOBase IOBase(string groupName, byte number)
	{
		return (mContainer.Scheme as UIScheme).IOGroupsInfo.Find((x) => x.BuildInfo.BuildString.Name == groupName).Design.IOBase(number);
	}
}
