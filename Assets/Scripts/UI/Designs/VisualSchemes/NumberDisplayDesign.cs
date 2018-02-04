using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NumberDisplayDesign : BaseInnerSchemeDesign
{

    [SerializeField] private Text mName = null, mType = null;
    [SerializeField] private InputField mNumber = null;
    [SerializeField] private Transform mInputs = null;
    [SerializeField] private Button mAddInputLink = null, mRemoveButton = null;
    [SerializeField] private EventTrigger mEventTrigger = null;
    [SerializeField] private GameObject mInputIOGroupPrefab = null;

    private UIScheme.InnerContainer mContainer;
    private List<IOInnerGroupDesign> mInputDesigns;
    private Dictionary<string, IOInnerGroupDesign> mIOGroupDesigns;
    private bool mSelfClick = false;
    private NumberDisplay mScheme;

    public override void Init(UIScheme.InnerContainer container)
    {
        mContainer = container;
        mScheme = (NumberDisplay) mContainer.Scheme;

        gameObject.name = "Scheme: " + mContainer.InnerBuildInfo.BuildString.Name;
        transform.localPosition = mContainer.InnerBuildInfo.Position.ToVector3();
        (transform as RectTransform).sizeDelta = mContainer.InnerBuildInfo.Size;
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

        mInputDesigns = new List<IOInnerGroupDesign>(inputCount);
        mIOGroupDesigns = new Dictionary<string, IOInnerGroupDesign>(inputCount);

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
            }
            design.Init(ioGroup.Key, ioGroup.Value, mContainer.Scheme);
        }

        SchemeDesigner.Instance.AddLinkStateChanged += OnAddLinkStateChanged;

        if (inputCount > 0)
        {
            //mAddInputLink.gameObject.SetActive(true);
            mAddInputLink.onClick.AddListener(() => SchemeDesigner.Instance.AddLinkAsTarget(mContainer));
        }

        mContainer.Scheme.IOGroups[NumberDisplay.Input].IOChanged += OnInputChanged;
    }

    private void OnInputChanged()
    {
        mNumber.text = mScheme.Number.ToString();
    }

    private void OnAddLinkStateChanged(bool sourceSelected)
    {
        if (mSelfClick)
            mSelfClick = false;
        else
            mAddInputLink.gameObject.SetActive(sourceSelected);
    }

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

    public override void DestroyThis()
    {
        SchemeDesigner.Instance.AddLinkStateChanged -= OnAddLinkStateChanged;
        mContainer.Scheme.IOGroups[NumberDisplay.Input].IOChanged -= OnInputChanged;
        Destroy(gameObject);
    }

    public override UIScheme.SchemeContainer SchemeContainer
    {
        get
        {
            return mContainer;
        }
    }

    public override IOBase IOBase(string groupName, byte number)
    {
        return mIOGroupDesigns[groupName].IOBase(number);
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
