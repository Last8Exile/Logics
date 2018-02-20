using UnityEngine;
using UnityEngine.EventSystems;

public abstract class SchemeDesign : MonoBehaviour {

	public abstract IOBase IOBase(string groupName, byte number);
	public abstract UIScheme.SchemeContainer SchemeContainer { get; }
}

public abstract class BaseInnerSchemeDesign : SchemeDesign
{
    public abstract void DestroyThis();


    protected UIScheme.InnerContainer mContainer;

    public virtual void Init(UIScheme.InnerContainer container)
    {
        mContainer = container;

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Drag;
        entry.callback.AddListener((data) => { OnDrag((PointerEventData)data); });
        mEventTrigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.BeginDrag;
        entry.callback.AddListener((data) => { OnBeginDrag((PointerEventData)data); });
        mEventTrigger.triggers.Add(entry);
    }

    public override UIScheme.SchemeContainer SchemeContainer
    {
        get
        {
            return mContainer;
        }
    }

    #region Drag

    [SerializeField] private EventTrigger mEventTrigger = null;
    private Vector2 mPosDiff;

    public void OnBeginDrag(PointerEventData eventData)
    {
        mPosDiff = transform.position.ToVector2() - Extensions.ScreenToWorldPos(eventData.position).ToVector2();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        var worldPos = Extensions.ScreenToWorldPos(eventData.position).ToVector2();
        var newPos = worldPos + mPosDiff;
        transform.position = newPos.ToVector3();
        mContainer.InnerBuildInfo.Position = transform.localPosition.ToVector2();
    }

    #endregion Drag

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
        var rectTransform = (RectTransform)transform;

        mCursorStartPos = Extensions.ScreenToWorldPos(pEventData.position).ToVector2();
        mStartPos = rectTransform.position;
        mStartSize = rectTransform.sizeDelta;
    }

    public void CornerOnDrag(BaseEventData eventData)
    {
        var pEventData = (PointerEventData)eventData;
        var rectTransform = (RectTransform)transform;

        if (pEventData.button != PointerEventData.InputButton.Left)
            return;
        var worldPos = Extensions.ScreenToWorldPos(pEventData.position).ToVector2();
        var deltaPos = worldPos - mCursorStartPos;

        var newSize = mStartSize + deltaPos.InverseY() / 2;
        var actualSize = newSize.ClampMin(new Vector2(240, 240));
        deltaPos = (actualSize - mStartSize).InverseY() * 2;

        var newPos = mStartPos + deltaPos / 2;

        rectTransform.position = newPos;
        rectTransform.sizeDelta = actualSize;
        mContainer.InnerBuildInfo.Position = transform.localPosition.ToVector2();
        mContainer.InnerBuildInfo.Size = actualSize;
    }

    public void CornerOnEndDrag()
    {
        mDragStarted = false;
        CornerOnPointerExit();
    }

    #endregion ResizeCorner
}
