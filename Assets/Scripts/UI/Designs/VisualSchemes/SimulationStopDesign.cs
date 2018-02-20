using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SimulationStopDesign : BaseInnerSchemeDesign
{
    [SerializeField] private Text mName = null, mType = null;
    [SerializeField] private Transform mInputs = null;
    [SerializeField] private Toggle mToggle = null;
    [SerializeField] private Button mAddInputLink = null, mRemoveButton = null;
    [SerializeField] private GameObject mInputIOGroupPrefab = null;

    private List<IOInnerGroupDesign> mInputDesigns;
    private Dictionary<string, IOInnerGroupDesign> mIOGroupDesigns;
    private bool mSelfClick = false;
    private SimulationStop mScheme;

    public override void Init(UIScheme.InnerContainer container)
    {
        base.Init(container);
        mScheme = (SimulationStop)mContainer.Scheme;
        mContainer.InnerBuildInfo.BuildString.Parameters = mScheme.Enabled.ToString();
        mToggle.isOn = mScheme.Enabled;

        gameObject.name = "Scheme: " + mContainer.InnerBuildInfo.BuildString.Name;
        transform.localPosition = mContainer.InnerBuildInfo.Position.ToVector3();
        (transform as RectTransform).sizeDelta = mContainer.InnerBuildInfo.Size;
        mName.text = mContainer.InnerBuildInfo.BuildString.Name;
        mType.text = mContainer.InnerBuildInfo.BuildString.Type;

        mRemoveButton.onClick.AddListener(() => SchemeDesigner.Instance.RemoveInnerScheme(mContainer));

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
    }

    private void OnAddLinkStateChanged(bool sourceSelected)
    {
        if (mSelfClick)
            mSelfClick = false;
        else
            mAddInputLink.gameObject.SetActive(sourceSelected);
    }

    public override void DestroyThis()
    {
        SchemeDesigner.Instance.AddLinkStateChanged -= OnAddLinkStateChanged;
        Destroy(gameObject);
    }

    public bool Enabled
    {
        set
        {
            mScheme.Enabled = value;
            mContainer.InnerBuildInfo.BuildString.Parameters = value.ToString();
        }
    }

    public override IOBase IOBase(string groupName, byte number)
    {
        return mIOGroupDesigns[groupName].IOBase(number);
    }
}
