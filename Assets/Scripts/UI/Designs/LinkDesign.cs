using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LinkDesign : MonoBehaviour {

	[SerializeField] private GameObject mLinePrefab = null;

	private UIScheme.LinkContainer mContainer;
	private LineContainer[] mLines;

	public void Init(UIScheme.LinkContainer container)
	{
		mContainer = container;
		gameObject.name = mContainer.BuildInfo.SourceName + " - " + mContainer.BuildInfo.TargetName;
		StartCoroutine (Initialisation ());
	}

	void Update()
	{
		foreach (var line in mLines)
			line.Update();
	}

	private IEnumerator Initialisation()
	{
		yield return new WaitForEndOfFrame ();

		Transform[] startPositions = new Transform[mContainer.BuildInfo.SourceCount];
		Transform[] endPositions = new Transform[mContainer.BuildInfo.TargetCount];
	

		for (byte i = 0; i < mContainer.BuildInfo.SourceCount; i++)
			startPositions[i] = mContainer.SourceScheme.Design.IOBase (mContainer.BuildInfo.SourceGroupName, (byte)(mContainer.BuildInfo.SourceStart + i)).ConnectionPosition;
		for (byte i = 0; i < mContainer.BuildInfo.TargetCount; i++)
			endPositions[i] = mContainer.TargetScheme.Design.IOBase (mContainer.BuildInfo.TargetGroupName, (byte)(mContainer.BuildInfo.TargetStart + i)).ConnectionPosition;

		mLines = new LineContainer[mContainer.BuildInfo.TargetCount];

		for (byte i = 0; i < mContainer.BuildInfo.TargetCount; i++)
		{
			var line = Instantiate (mLinePrefab, transform).GetComponent<LineRenderer> ();

			mLines[i] = new LineContainer();
			mLines[i].LineRenderer = line;

			if (mContainer.BuildInfo.SourceCount == 1) 
			{
				mLines[i].Start = startPositions[0];
				line.gameObject.name = mContainer.BuildInfo.SourceStart.ToString () + " - " + (mContainer.BuildInfo.TargetStart + i).ToString ();
			} 
			else 
			{
				mLines[i].Start = startPositions[i];
				line.gameObject.name = (mContainer.BuildInfo.SourceStart + i).ToString () + " - " + (mContainer.BuildInfo.TargetStart + i).ToString ();
			}
				
			mLines[i].End = endPositions[i];
		}

		enabled = true;

		yield break;
	}

	private class LineContainer
	{
		public LineRenderer LineRenderer;
		public Transform Start,End;

		public void Update()
		{
			LineRenderer.SetPosition (0, Start.position);
			LineRenderer.SetPosition (1, End.position);
		}
	}

	public void DestroyThis()
	{
		Destroy(gameObject);
	}
}
