using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class IntConstDesign : BaseInnerSchemeDesign
{

    [SerializeField] private Text mName = null, mType = null;
    [SerializeField] private InputField mNumber = null;
    [SerializeField] private Transform mOutputs = null;
    [SerializeField] private Button mAddOutputLink = null, mRemoveButton = null;
    [SerializeField] private GameObject mOutputIOGroupPrefab = null;

    private List<IOInnerGroupDesign> mOutputDesigns;
    private Dictionary<string, IOInnerGroupDesign> mIOGroupDesigns;
    private IntConst mScheme;

    public override void Init(UIScheme.InnerContainer container)
    {
        base.Init(container);

        mScheme = (IntConst) mContainer.Scheme;
        mContainer.InnerBuildInfo.BuildString.Parameters = mScheme.Number.ToString();
        mNumber.text = mContainer.InnerBuildInfo.BuildString.Parameters;
        gameObject.name = "Scheme: " + mContainer.InnerBuildInfo.BuildString.Name;
        transform.localPosition = mContainer.InnerBuildInfo.Position.ToVector3();
        (transform as RectTransform).sizeDelta = mContainer.InnerBuildInfo.Size;
        mName.text = mContainer.InnerBuildInfo.BuildString.Name;
        mType.text = mContainer.InnerBuildInfo.BuildString.Type;

        mRemoveButton.onClick.AddListener(() => SchemeDesigner.Instance.RemoveInnerScheme(mContainer));

        var outputCount = mContainer.Scheme.IOGroups.Count((x) => x.Value.IO == IO.Output);

        mOutputDesigns = new List<IOInnerGroupDesign>(outputCount);
        mIOGroupDesigns = new Dictionary<string, IOInnerGroupDesign>(outputCount);

        foreach (var ioGroup in mContainer.Scheme.IOGroups)
        {
            IOInnerGroupDesign design = null;
            switch (ioGroup.Value.IO)
            {
                case IO.Output:
                    design = Instantiate(mOutputIOGroupPrefab, mOutputs).GetComponent<IOInnerGroupDesign>();
                    mOutputDesigns.Add(design);
                    mIOGroupDesigns.Add(ioGroup.Key, design);
                    break;
            }
            design.Init(ioGroup.Key, ioGroup.Value, mContainer.Scheme);
        }

        SchemeDesigner.Instance.AddLinkStateChanged += OnAddLinkStateChanged;

        if (outputCount > 0)
        {
            mAddOutputLink.gameObject.SetActive(true);
            mAddOutputLink.onClick.AddListener(() =>
            {
                SchemeDesigner.Instance.AddLinkAsSource(mContainer);
            });
        }

        mNumber.onEndEdit.AddListener(OnNumberChanged);
    }

    private void OnNumberChanged(string value)
    {
        try
        {
            mScheme.Number = int.Parse(value);
            mContainer.InnerBuildInfo.BuildString.Parameters = value;
        }
        catch (Exception e)
        {
            Console.Instance.Log(e.Message);
        }

    }

    private void OnAddLinkStateChanged(bool sourceSelected)
    {
        mAddOutputLink.gameObject.SetActive(!sourceSelected);
    }

    public override void DestroyThis()
    {
        SchemeDesigner.Instance.AddLinkStateChanged -= OnAddLinkStateChanged;
        mNumber.onEndEdit.RemoveListener(OnNumberChanged);
        Destroy(gameObject);
    }

    public override IOBase IOBase(string groupName, byte number)
    {
        return mIOGroupDesigns[groupName].IOBase(number);
    }
}
