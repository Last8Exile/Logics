using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SchemeDesigner : MonoBehaviour {

	public static SchemeDesigner Instance 
	{
		get { 
			if (mInstance == null) {
				mInstance = GameObject.Find ("SchemeDesigner").GetComponent<SchemeDesigner> ();
				mInstance.Init ();
			}
			return mInstance;
		}
	}
	private static SchemeDesigner mInstance;

    [Serializable]
    public struct Dialogs
    {
        public GameObject
            mSchemeDialogPrefab,
            mSizeSchemeDialogPrefab,
            mRAMXDialogPrefab,
            mNumberDisplayDialogPrefab;
    }
    [SerializeField]
    private Dialogs mDialogsStruct;

    [Serializable]
    public struct Designs
    {
        public GameObject
            mInnerDesignPrefab,
            mNumberDisplayDesignPrefab,
            mSimulationStopDesignPrefab,
            mIntConstDesignPrefab;
    }
    [SerializeField]
    private Designs mDesignsStruct;


    //Containers
    [SerializeField] private Transform 
		mUICanvas = null, 
		mSchemeContainer = null, 
		mLinksContainer = null;

    //Prefabs
    [SerializeField]
    private GameObject
        mSelfDesignPrefab = null,
        mSelfIOGroupPrefab = null,
        mLinkPrefab = null,
        mNewSchemePrefab = null,
        mNewLinkDialogPrefab = null,
        mAddIOGroupDialogPrefab = null,
        mConfirmDialogPrefab = null;


    //Buttons
    [SerializeField] private Button 
		mCreateButton = null, 
		mSaveButton = null,
		mIOGroupButton = null;

    private Dictionary<string, DialogContainer> mDialogs;
    private Dictionary<string, DesignContainer> mDesigns;

	public UIScheme CurrentScheme;

    private void Init()
    {
        mDialogs = new Dictionary<string, DialogContainer>();
        mDialogs.Add("Scheme", new DialogContainer(mDialogsStruct.mSchemeDialogPrefab, typeof(InnerSchemeDialog)));
        mDialogs.Add(NAND.DialogType, new DialogContainer(mDialogsStruct.mSchemeDialogPrefab, typeof(InnerSchemeDialog)));
        mDialogs.Add(CONST.DialogType, new DialogContainer(mDialogsStruct.mSchemeDialogPrefab, typeof(InnerSchemeDialog)));
        mDialogs.Add(DFF.DialogType, new DialogContainer(mDialogsStruct.mSchemeDialogPrefab, typeof(InnerSchemeDialog)));
        mDialogs.Add(NANDX.DialogType, new DialogContainer(mDialogsStruct.mSizeSchemeDialogPrefab, typeof(SizeSchemeDialog)));
        mDialogs.Add(ANDX.DialogType, new DialogContainer(mDialogsStruct.mSizeSchemeDialogPrefab, typeof(SizeSchemeDialog)));
        mDialogs.Add(ORX.DialogType, new DialogContainer(mDialogsStruct.mSizeSchemeDialogPrefab, typeof(SizeSchemeDialog)));
        mDialogs.Add(NOTX.DialogType, new DialogContainer(mDialogsStruct.mSizeSchemeDialogPrefab, typeof(SizeSchemeDialog)));
        mDialogs.Add(XORX.DialogType, new DialogContainer(mDialogsStruct.mSizeSchemeDialogPrefab, typeof(SizeSchemeDialog)));
        mDialogs.Add(RAMX.DialogType, new DialogContainer(mDialogsStruct.mRAMXDialogPrefab, typeof(RAMXDialog)));
        mDialogs.Add(NumberDisplay.DialogType, new DialogContainer(mDialogsStruct.mNumberDisplayDialogPrefab, typeof(NumberDisplayDialog)));
        mDialogs.Add(SimulationStop.DialogType, new DialogContainer(mDialogsStruct.mSchemeDialogPrefab, typeof(InnerSchemeDialog)));
        mDialogs.Add(IntConst.DialogType, new DialogContainer(mDialogsStruct.mSchemeDialogPrefab, typeof(InnerSchemeDialog)));

        mDesigns = new Dictionary<string, DesignContainer>();
        mDesigns.Add("Default", new DesignContainer(mDesignsStruct.mInnerDesignPrefab, typeof(InnerSchemeDesign)));
        mDesigns.Add(NumberDisplay.DesignType, new DesignContainer(mDesignsStruct.mNumberDisplayDesignPrefab, typeof(NumberDisplayDesign)));
        mDesigns.Add(SimulationStop.DesignType, new DesignContainer(mDesignsStruct.mSimulationStopDesignPrefab, typeof(SimulationStopDesign)));
        mDesigns.Add(IntConst.DesignType, new DesignContainer(mDesignsStruct.mIntConstDesignPrefab, typeof(IntConstDesign)));
    }


    public void CreateScheme()
    {
        CreateSchemeAdv();
    }

    public NewSchemeDialog CreateSchemeAdv()
	{
	    mCreateButton.interactable = false;
	    var newSchemeDialog = Instantiate(mNewSchemePrefab, mUICanvas).GetComponent<NewSchemeDialog>();

        StartCoroutine (createScheme (newSchemeDialog));

	    return newSchemeDialog;
	}

	private IEnumerator createScheme(NewSchemeDialog newSchemeDialog)
	{
		yield return new WaitWhile (() => newSchemeDialog.DialogResult == DialogResult.NotReady);

		var buildInfo = newSchemeDialog.BuildInfo;
		var dialogResult = newSchemeDialog.DialogResult;
		newSchemeDialog.Dispose ();
		mCreateButton.interactable = true;

		if (dialogResult == DialogResult.Cancel) 
		{
			yield break;
		}

		CreateScheme(new UIScheme(buildInfo));

		yield break;
	}

	public void CreateScheme(UIScheme scheme)
	{
		if (CurrentScheme != null) 
		{
			CurrentScheme.DestroyThis();
		}
		CurrentScheme = scheme;
		mSaveButton.gameObject.SetActive(true);
		mIOGroupButton.gameObject.SetActive(true);
	}
		
	public void LoadScheme(string type)
	{
		if (CurrentScheme != null) 
		{
			CurrentScheme.DestroyThis();
		}

	    var builder = (SchemeManager.Instance.GetBuildingRule(type) as UISchemeBuilder).Clone();
		CurrentScheme = builder.BuildUIScheme();
		mSaveButton.gameObject.SetActive(true);
		mIOGroupButton.gameObject.SetActive(true);
	}

	public void SaveScheme()
	{
		SchemeManager.Instance.AddOrUpdateBuildingRule(CurrentScheme.Self.SchemeName, CurrentScheme.CreateBuilder());
	}

	public SelfSchemeDesign CreateSelfDesign(UIScheme.SelfContainer container)
	{
		var design = Instantiate (mSelfDesignPrefab,mSchemeContainer).GetComponent<SelfSchemeDesign> ();
		design.Init (container);
		return design;
	}

	public IOSelfIOGroupDesign CreateSelfIOGroupDesign(UIScheme.IOGroupContainer container)
	{
		var design = Instantiate(mSelfIOGroupPrefab, mSchemeContainer).GetComponent<IOSelfIOGroupDesign>();
		design.Init(container);
		return design;
	}

	public BaseInnerSchemeDesign CreateInnerScheme(UIScheme.InnerContainer container)
	{
	    var designName = SchemeManager.Instance.GetBuildingRule(container.InnerBuildInfo.BuildString.Type).DesignType;
	    var designContainer = mDesigns[designName];
	    var innerScheme = (BaseInnerSchemeDesign)Instantiate(designContainer.Prefab, mSchemeContainer).GetComponent(designContainer.Type);
		innerScheme.Init (container);
		return innerScheme;
	}

	public LinkDesign CreateLinkDesign(UIScheme.LinkContainer container)
	{
		var link = Instantiate (mLinkPrefab, mLinksContainer).GetComponent<LinkDesign> ();
		link.Init (container);
		return link;
	}

	public void AddIOGroup()
	{
	    if (CurrentScheme == null)
	    {
	        Console.Instance.Log("Сначала необходимо создать или загрузить схему");
	        return;
	    }
        AddIOGroupAdv();
	}

    public AddIOGroupDialog AddIOGroupAdv()
    {
        var dialog = Instantiate(mAddIOGroupDialogPrefab, mUICanvas).GetComponent<AddIOGroupDialog>();
        StartCoroutine(addIOGroup(dialog));
        return dialog;
    }

    private IEnumerator addIOGroup(AddIOGroupDialog dialog)
	{
		dialog.ShowDialog("НОВАЯ ГРУППА");
		yield return new WaitWhile(() => dialog.DialogResult == DialogResult.NotReady);

		var dialogResult = dialog.DialogResult;
		var buildParams = (IOGroupBuildString)dialog.Result;
		dialog.Dispose();

		if (dialogResult == DialogResult.Cancel)
			yield break;

	    var ioGroupBuildInfo = new UIIOGroupBuildInfo(buildParams, Vector2.zero, new Vector2(IOSelfIOGroupDesign.MinCellWidth, buildParams.Size * IOSelfIOGroupDesign.DefaultCellHeight));
		CurrentScheme.AddIOGroup(ioGroupBuildInfo);
		yield break;
	}

    public void RemoveIOGroup(UIScheme.IOGroupContainer container)
    {
        StartCoroutine(removeIOGroup(container));
    }

    private IEnumerator removeIOGroup(UIScheme.IOGroupContainer container)
    {
        var dialog = Instantiate(mConfirmDialogPrefab, mUICanvas).GetComponent<ConfirmDialog>();
        dialog.ShowDialog("Удалить группу " + container.BuildInfo.BuildString.Name + "?", "Удалив группу, будут удалены все связанные с ней ссылки");
        yield return new WaitWhile(() => dialog.DialogResult == DialogResult.NotReady);

        var dialogResult = dialog.DialogResult;
        dialog.Dispose();

        if (dialogResult == DialogResult.Cancel)
            yield break;

        CurrentScheme.DeleteIOGroup(container);
        yield break;
    }

    public void AddInnerScheme(string type)
    {
        if (CurrentScheme == null)
        {
            Console.Instance.Log("Сначала необходимо создать или загрузить схему");
            return;
        }
        AddInnerSchemeAdv(type);
    }

    public InnerSchemeDialog AddInnerSchemeAdv(string type)
	{
	    var builder = SchemeManager.Instance.GetBuildingRule(type);
	    var dialogContainer = mDialogs[builder.DialogType];
	    var dialog = (InnerSchemeDialog)Instantiate(dialogContainer.Prefab, mUICanvas).GetComponent(dialogContainer.Type);
        StartCoroutine(addInnerScheme(type,dialog));
	    return dialog;
	}

	private IEnumerator addInnerScheme(string type, InnerSchemeDialog dialog)
	{
		dialog.ShowDialog(type);
		yield return new WaitWhile(() => dialog.DialogResult == DialogResult.NotReady);

		var dialogResult = dialog.DialogResult;
		var buildParams = (InnerSchemeBuildParams)dialog.Result;
		dialog.Dispose();

		if (dialogResult == DialogResult.Cancel)
			yield break;

		var innerSchemeBuildInfo = new UIInnerSchemeBuildInfo(buildParams.Name, type, buildParams.Parameters, Vector2.zero, Vector2.one * 240);
		CurrentScheme.AddScheme(innerSchemeBuildInfo);

		yield break;
	}

    public void RemoveInnerScheme(UIScheme.InnerContainer container)
    {
        RemoveInnerSchemeAdv(container);
    }


    public ConfirmDialog RemoveInnerSchemeAdv(UIScheme.InnerContainer container)
	{
	    var dialog = Instantiate(mConfirmDialogPrefab, mUICanvas).GetComponent<ConfirmDialog>();
	    StartCoroutine(removeInnerScheme(container, dialog));
	    return dialog;
	}

	private IEnumerator removeInnerScheme(UIScheme.InnerContainer container, ConfirmDialog dialog)
	{
		dialog.ShowDialog("Удалить схему " + container.SchemeName + "?", "Удалив схему, будут удалены все связанные с ней ссылки");
		yield return new WaitWhile(() => dialog.DialogResult == DialogResult.NotReady);

		var dialogResult = dialog.DialogResult;
		dialog.Dispose();

		if (dialogResult == DialogResult.Cancel)
			yield break;

		CurrentScheme.DeleteInnerScheme(container);
		yield break;
	}
		

	private UIScheme.IOGroupContainer mSourceContainerGroup, mTargetContainerGroup;
	private UIScheme.InnerContainer mSourceContainerInner, mTargetContainerInner;
	public event Action<bool> AddLinkStateChanged;

	public void AddLinkAsSource(UIScheme.IOGroupContainer selfIOGroupContainer)
	{
		mSourceContainerGroup = selfIOGroupContainer;
		if (AddLinkStateChanged != null)
			AddLinkStateChanged.Invoke(true);
	}
	public void AddLinkAsSource(UIScheme.InnerContainer innerContainer)
	{
		mSourceContainerInner = innerContainer;
		if (AddLinkStateChanged != null)
			AddLinkStateChanged.Invoke(true);
	}
	public void AddLinkAsTarget(UIScheme.IOGroupContainer selfIOGroupContainer, bool complete = true)
	{
		mTargetContainerGroup = selfIOGroupContainer;
		if (AddLinkStateChanged != null)
			AddLinkStateChanged.Invoke(false);
	    if (complete)
	        AddLinkAdv();
	}
	public void AddLinkAsTarget(UIScheme.InnerContainer innerContainer, bool complete = true)
	{
		mTargetContainerInner = innerContainer;
		if (AddLinkStateChanged != null)
			AddLinkStateChanged.Invoke(false);
	    if (complete)
            AddLinkAdv();
    }

    public AddLinkDialog AddLinkAdv()
    {
        var dialog = Instantiate(mNewLinkDialogPrefab, mUICanvas).GetComponent<AddLinkDialog>();
        StartCoroutine(addLink(dialog));
        return dialog;
    }

	private IEnumerator addLink(AddLinkDialog dialog)
	{
		var startName = "";
		var endName = "";

		if (mSourceContainerGroup != null)
			startName = mSourceContainerGroup.ParentScheme.SchemeName;
		if (mSourceContainerInner != null)
			startName = mSourceContainerInner.SchemeName;
		if (mTargetContainerGroup != null)
			endName = mTargetContainerGroup.ParentScheme.SchemeName;
		if (mTargetContainerInner != null)
			endName = mTargetContainerInner.SchemeName;

		dialog.ShowDialog(startName + " - " + endName,  mSourceContainerGroup, mSourceContainerInner, mTargetContainerGroup, mTargetContainerInner);
		yield return new WaitWhile(() => dialog.DialogResult == DialogResult.NotReady);

		var dialogResult = dialog.DialogResult;
		var buildParams = dialog.Params;
		dialog.Dispose();

		if (dialogResult == DialogResult.Cancel)
		{
			ResetAddLink();
			yield break;
		}

		var linkBuilder = new LinkBuilder(startName, endName, buildParams.SourceGroupName, buildParams.TargetGroupName, buildParams.SourceStart, buildParams.SourceCount, buildParams.TargetStart, buildParams.TargetCount);
		CurrentScheme.AddLink(linkBuilder);

		ResetAddLink();

		yield break;
	}

	private void ResetAddLink()
	{
		mSourceContainerGroup = null;
		mSourceContainerInner = null;
		mTargetContainerGroup = null;
		mTargetContainerInner = null;
	}

	#region InnerClasses

	private class DialogContainer
	{
		public DialogContainer(GameObject prefab, Type type)
		{
			Prefab = prefab;
			Type = type;
		}

		public GameObject Prefab;
		public Type Type;
	}

    private class DesignContainer
    {
        public DesignContainer(GameObject prefab, Type type)
        {
            Prefab = prefab;
            Type = type;
        }

        public GameObject Prefab;
        public Type Type;
    }

	public class InnerSchemeBuildParams
	{
		public string Name;
		public string Parameters;
	}

	public class LinkBuildParams
	{
		public string SourceGroupName, TargetGroupName;
		public byte SourceStart, SourceCount;
		public byte TargetStart, TargetCount;
	}

	#endregion InnerClasses
}
