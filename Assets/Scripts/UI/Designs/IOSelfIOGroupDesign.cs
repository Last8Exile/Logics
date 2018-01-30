using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IOSelfIOGroupDesign : IOGroupDesign {

	[SerializeField] private Transform mIOContainer = null, mBitsContainer = null;
	[SerializeField] private GameObject mIOPrefab = null;
	[SerializeField] private Button mAddLink = null, mRemoveButton = null;
	[SerializeField] private EventTrigger mEventTrigger = null;
	[SerializeField] private Text mName = null;


    public const float MinCellWidth = 50;
    public const float MinCellHeight = 10;
    public const float DefaultCellHeight = 50;

    private Vector2 mMinSize
    {
        get { return new Vector2(MinCellWidth, MinCellHeight * mContainer.BuildInfo.BuildString.Size); }
    }

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
			mIOTogglers[i] = Instantiate (mIOPrefab, mBitsContainer).GetComponent<IOToggler> ();
			mIOTogglers[i].Init(mContainer.ParentScheme.Scheme, i, mContainer.BuildInfo.BuildString.Name, mContainer.BuildInfo.BuildString.IO);
		}
	    (mBitsContainer as RectTransform).sizeDelta = mContainer.BuildInfo.Size;

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

    #region ResizeCorner

    private Vector2 mCursorStartPos, mStartPos, mStartSize;
    private static bool mDragStarted;

    public void CornerOnPointerEnter()
    {
        if (mDragStarted)
            return;
        CursorsManager.Instance.SetCursor(CursorType.Resize);
    }

    public void CornerOnPointerExit()
    {
        if (mDragStarted)
            return;
        CursorsManager.Instance.SetCursor(CursorType.Default);
    }

    public void CornerOnBeginDrag(BaseEventData eventData)
    {
        CornerOnPointerEnter();
        mDragStarted = true;

        var pEventData = (PointerEventData)eventData;
        var ioTransform = (RectTransform)mIOContainer;
        var bitsTransform = (RectTransform)mBitsContainer;

        mCursorStartPos = Extensions.ScreenToWorldPos(pEventData.position).ToVector2();
        mStartPos = ioTransform.position;
        mStartSize = bitsTransform.sizeDelta;
    }

    public void CornerOnDrag(BaseEventData eventData)
    {
        var pEventData = (PointerEventData)eventData;
        var ioTransform = (RectTransform)mIOContainer;
        var bitsTransform = (RectTransform)mBitsContainer;

        if (pEventData.button != PointerEventData.InputButton.Left)
            return;
        var worldPos = Extensions.ScreenToWorldPos(pEventData.position).ToVector2();
        var deltaPos = worldPos - mCursorStartPos;
        deltaPos.x = 0;

        var newSize = mStartSize + deltaPos.InverseY() / 2;
        var actualSize = newSize.ClampMin(mMinSize);
        deltaPos = (actualSize - mStartSize).InverseY() * 2;

        var newPos = mStartPos + deltaPos / 2;

        ioTransform.position = newPos;
        bitsTransform.sizeDelta = actualSize;
        mContainer.BuildInfo.Position = transform.localPosition.ToVector2();
        mContainer.BuildInfo.Size = actualSize;
    }

    public void CornerOnEndDrag()
    {
        mDragStarted = false;
        CornerOnPointerExit();
    }

    #endregion ResizeCorner

}
