using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InnerSchemeDesign : SchemeDesign {
	
	[SerializeField] private Text mName = null, mType = null;
	[SerializeField] private Transform mInputs = null, mOutputs = null;
	[SerializeField] private Button mAddInputLink = null, mAddOutputLink = null, mRemoveButton = null;
	[SerializeField] private EventTrigger mEventTrigger = null;
	[SerializeField] private GameObject mInputIOGroupPrefab = null, mOutputIOGroupPrefab = null;

	private UIScheme.InnerContainer mContainer;
	private List<IOInnerGroupDesign> mInputDesigns, mOutputDesigns;
	private Dictionary<string,IOInnerGroupDesign> mIOGroupDesigns;
	private bool mSelfClick = false;

	public void Init(UIScheme.InnerContainer container)
	{
		mContainer = container;

		gameObject.name = "Scheme: " + mContainer.InnerBuildInfo.BuildString.Name;
		transform.localPosition = mContainer.InnerBuildInfo.Position.ToVector3 ();
		mName.text = mContainer.InnerBuildInfo.BuildString.Name;
		mType.text = mContainer.InnerBuildInfo.BuildString.Type;

		mRemoveButton.onClick.AddListener(() => SchemeDesigner.Instance.RemoveInnerScheme(mContainer));

		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.Drag;
		entry.callback.AddListener((data) => { OnDrag((PointerEventData)data); });
		mEventTrigger.triggers.Add(entry);

		entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.BeginDrag;
		entry.callback.AddListener((data) => { OnBeginDrag((PointerEventData)data); });
		mEventTrigger.triggers.Add(entry);

		var inputCount = mContainer.Scheme.IOGroups.Count((x) => x.Value.IO == IO.Input);
		var outputCount = mContainer.Scheme.IOGroups.Count - inputCount;

		mInputDesigns = new List<IOInnerGroupDesign>(inputCount);
		mOutputDesigns = new List<IOInnerGroupDesign>(outputCount);
		mIOGroupDesigns = new Dictionary<string, IOInnerGroupDesign>(inputCount + outputCount);

		foreach (var ioGroup in mContainer.Scheme.IOGroups)
		{
			IOInnerGroupDesign design = null;
			switch (ioGroup.Value.IO)
			{
				case IO.Input:
					design = Instantiate(mInputIOGroupPrefab, mInputs).GetComponent<IOInnerGroupDesign>();
					mInputDesigns.Add(design);
					mIOGroupDesigns.Add(ioGroup.Key, design);
					break;
				case IO.Output:
					design = Instantiate(mOutputIOGroupPrefab, mOutputs).GetComponent<IOInnerGroupDesign>();
					mOutputDesigns.Add(design);
					mIOGroupDesigns.Add(ioGroup.Key, design);
					break;
			}
			design.Init(ioGroup.Key, ioGroup.Value, mContainer.Scheme);
		}

		SchemeDesigner.Instance.AddLinkStateChanged += OnAddLinkStateChanged;

		if (inputCount > 0) 
		{
			//mAddInputLink.gameObject.SetActive(true);
			mAddInputLink.onClick.AddListener(() => SchemeDesigner.Instance.AddLinkAsTarget(mContainer));
		}

		if (outputCount > 0) 
		{
			mAddOutputLink.gameObject.SetActive(true);
			mAddOutputLink.onClick.AddListener(() => 
				{ 
					mSelfClick = true;
					SchemeDesigner.Instance.AddLinkAsSource(mContainer);
				});
		}

	}

	private void OnAddLinkStateChanged(bool sourceSelected)
	{
		if (mSelfClick)
			mSelfClick = false;
		else
			mAddInputLink.gameObject.SetActive(sourceSelected);

		mAddOutputLink.gameObject.SetActive(!sourceSelected);
	}

	private Vector2 mPosDiff;

	public void OnDrag(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Left)
			return;
		var worldPos = Extensions.ScreenToWorldPos(eventData.position).ToVector2();
		var newPos = worldPos + mPosDiff;
		transform.position = newPos.ToVector3();
		mContainer.InnerBuildInfo.Position = transform.localPosition.ToVector2();
	}

	public void OnBeginDrag(PointerEventData eventData) 
	{
		mPosDiff = transform.position.ToVector2() - Extensions.ScreenToWorldPos(eventData.position).ToVector2();
	}

	public void DestroyThis()
	{
		SchemeDesigner.Instance.AddLinkStateChanged -= OnAddLinkStateChanged;
		Destroy(gameObject);
	}

	public override UIScheme.SchemeContainer SchemeContainer {
		get {
			return mContainer;
		}
	}

	public override IOBase IOBase(string groupName, byte number)
	{
		return mIOGroupDesigns[groupName].IOBase(number);
	}
}
