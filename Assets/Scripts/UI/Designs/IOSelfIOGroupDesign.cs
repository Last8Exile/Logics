using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IOSelfIOGroupDesign : IOGroupDesign {

	[SerializeField] private Transform mIOContainer = null;
	[SerializeField] private GameObject mIOPrefab = null;
	[SerializeField] private Button mAddLink = null, mRemoveButton = null;
	[SerializeField] private EventTrigger mEventTrigger = null;
	[SerializeField] private Text mName = null;

	private IOToggler[] mIOTogglers;
	private UIScheme.IOGroupContainer mContainer;

	public void Init(UIScheme.IOGroupContainer container)
	{
		mContainer = container;
		gameObject.name = (mContainer.BuildInfo.BuildString.IO == IO.Input ? "Input: " : "Output: ") + 
			mContainer.BuildInfo.BuildString.Name + " (" + mContainer.BuildInfo.BuildString.Size.ToString() + ")";
		mName.text = mContainer.BuildInfo.BuildString.Name;

		mIOContainer.localPosition = mContainer.BuildInfo.Position;

		if (mIOTogglers != null)
			foreach (var input in mIOTogglers)
				Destroy (input.gameObject);

		mIOTogglers = new IOToggler[mContainer.BuildInfo.BuildString.Size];

		for (byte i = 0; i < mContainer.BuildInfo.BuildString.Size; i++) 
		{
			mIOTogglers[i] = Instantiate (mIOPrefab, mIOContainer).GetComponent<IOToggler> ();
			mIOTogglers[i].Init(mContainer.ParentScheme.Scheme, i, mContainer.BuildInfo.BuildString.Name, mContainer.BuildInfo.BuildString.IO);
		}

		if (mContainer.BuildInfo.BuildString.Size > 0) 
		{
			switch (mContainer.BuildInfo.BuildString.IO)
			{
				case IO.Input:
					mAddLink.gameObject.SetActive(true);
					mAddLink.onClick.AddListener(() => SchemeDesigner.Instance.AddLinkAsSource(mContainer));
					break;
				case IO.Output:
					mAddLink.onClick.AddListener(() => SchemeDesigner.Instance.AddLinkAsTarget(mContainer));
					break;
			}
		}

		SchemeDesigner.Instance.AddLinkStateChanged += OnAddLinkStateChanged;
	    mRemoveButton.onClick.AddListener(() => SchemeDesigner.Instance.RemoveIOGroup(mContainer));

        EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.Drag;
		entry.callback.AddListener((data) => { OnDrag((PointerEventData)data); });
		mEventTrigger.triggers.Add(entry);

		entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.BeginDrag;
		entry.callback.AddListener((data) => { OnBeginDrag((PointerEventData)data); });
		mEventTrigger.triggers.Add(entry);
	}

	private void OnAddLinkStateChanged(bool sourceSelected)
	{
		mAddLink.gameObject.SetActive(mContainer.BuildInfo.BuildString.IO == IO.Input ? !sourceSelected : sourceSelected);
	}
		
	private Vector2 mPosDiff;

	public void OnDrag(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Left)
			return;
		var worldPos = Extensions.ScreenToWorldPos(eventData.position).ToVector2();
		var newPos = worldPos + mPosDiff;

		mIOContainer.position = newPos.ToVector3();
		mContainer.BuildInfo.Position = mIOContainer.localPosition.ToVector2();
	}

	public void OnBeginDrag(PointerEventData eventData) 
	{
		mPosDiff = mIOContainer.position.ToVector2() - Extensions.ScreenToWorldPos(eventData.position).ToVector2();
	}

	public UIScheme.SchemeContainer SchemeContainer {
		get {
			return mContainer.ParentScheme;
		}
	}

	public override IOBase IOBase (byte number)
	{
		return mIOTogglers[number];
	}

	public void DestroyThis()
	{
		SchemeDesigner.Instance.AddLinkStateChanged -= OnAddLinkStateChanged;
		Destroy(gameObject);
	}

}
