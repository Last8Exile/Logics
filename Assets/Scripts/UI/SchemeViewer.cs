using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchemeViewer : MonoBehaviour {

	[SerializeField] private GameObject mSchemePrefab = null;
	[SerializeField] private Transform mContentHolder = null;

	void Start ()
	{
		SchemeManager.Instance.SchemesChanged += Reload;
		Reload ();
	}

	private void Reload()
	{
		mContentHolder.RemoveChilds ();
		foreach (var schemeBuildengRule in SchemeManager.Instance.BuildingRules()) {
			var cell = Instantiate (mSchemePrefab, mContentHolder).GetComponent<SchemeDescription> ();
			cell.Init (schemeBuildengRule.Key, schemeBuildengRule.Value);
		}
		mContentHolder.SortChilds();
	}
}
