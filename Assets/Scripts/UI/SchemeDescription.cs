using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SchemeDescription : MonoBehaviour {

	[SerializeField] private Text mName = null;
	[SerializeField] private Button mAddButton = null, mEditButton = null;

	public void Init(string schemeName, SchemeBuilder builder)
	{
		mName.text = schemeName;
		gameObject.name = schemeName;
		mAddButton.onClick.AddListener(AddThis);

		if ((builder as UISchemeBuilder) != null) 
		{
			mEditButton.onClick.AddListener(EditThis);
			mEditButton.gameObject.SetActive(true);
		}
	}

	public void AddThis()
	{
		SchemeDesigner.Instance.AddInnerScheme(mName.text);
	}

	public void EditThis()
	{
		SchemeDesigner.Instance.LoadScheme(mName.text);
	}
}
