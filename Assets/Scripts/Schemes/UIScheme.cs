using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class UIScheme : Scheme 
{
	public SelfContainer Self;
	public List<IOGroupContainer> IOGroupsInfo;
	public List<InnerContainer> InnerSchemes;
	public List<LinkContainer> Links;

	public Dictionary<string,SchemeContainer> Schemes;

	public UIScheme(UISelfSchemeBuildInfo selfInfo, byte ioGropsCount = 0, byte innerSchemesCount = 0, byte linksCount = 0) : base(ioGropsCount)
	{
		Self = new SelfContainer (selfInfo, this);

		IOGroupsInfo = new List<IOGroupContainer>(ioGropsCount);
		InnerSchemes = new List<InnerContainer> (innerSchemesCount);
		Links = new List<LinkContainer> (linksCount);
		Schemes = new Dictionary<string, SchemeContainer> (innerSchemesCount + 1);

		Schemes.Add (selfInfo.Name, Self);

		Self.Design = SchemeDesigner.Instance.CreateSelfDesign (Self);
	}

	public void AddScheme(UIInnerSchemeBuildInfo innerSchemeInfo)
	{
		var innerContainer = new InnerContainer (innerSchemeInfo);

		InnerSchemes.Add (innerContainer);
		InnerSchemesList.Add(innerContainer.Scheme);
		Schemes.Add (innerSchemeInfo.BuildString.Name, innerContainer);

		innerContainer.Design = SchemeDesigner.Instance.CreateInnerScheme (innerContainer);
	}

	public void DeleteInnerScheme(InnerContainer container)
	{
		var removedLinks = new List<LinkContainer>();
		container.SourceThisLinks.ForEach((x) => {
			if (removedLinks.Contains(x))
				return;
			removedLinks.Add(x);
			x.Link.RemoveLink();
			x.Design.DestroyThis();
			x.TargetScheme.TargetThisLinks.Remove(x);
			Links.Remove(x);
		});
		container.TargetThisLinks.ForEach((x) => {
			if (removedLinks.Contains(x))
				return;
			x.Link.RemoveLink();
			x.Design.DestroyThis();
			x.SourceScheme.SourceThisLinks.Remove(x);
			Links.Remove(x);
		});
		removedLinks.Clear();
		(container.Design as InnerSchemeDesign).DestroyThis();
		container.Scheme.UnlinkAll();
		InnerSchemes.Remove(container);
		InnerSchemesList.Remove(container.Scheme);
		Schemes.Remove(container.SchemeName);
	}

	public void AddIOGroup(UIIOGroupBuildInfo ioGroupBuildInfo)
	{
		var ioGroupContainer = new IOGroupContainer(ioGroupBuildInfo, Self);

		IOGroups.Add(ioGroupContainer.BuildInfo.BuildString.Name, ioGroupContainer.IOGroup);
		IOGroupsInfo.Add(ioGroupContainer);

		ioGroupContainer.Design = SchemeDesigner.Instance.CreateSelfIOGroupDesign(ioGroupContainer);
	}

    public void DeleteIOGroup(IOGroupContainer container)
    {
        var removedLinks = new List<LinkContainer>();
        switch (container.IOGroup.IO)
        {
            case IO.Input:
                Self.SourceThisLinks.Where(x=>x.BuildInfo.SourceGroupName==container.BuildInfo.BuildString.Name).ForEach(x =>
                {
                    if (removedLinks.Contains(x))
                        return;
                    removedLinks.Add(x);
                    x.Link.RemoveLink();
                    x.Design.DestroyThis();
                    x.TargetScheme.TargetThisLinks.Remove(x);
                    Links.Remove(x);
                });
                removedLinks.ForEach(x => Self.SourceThisLinks.Remove(x));
                break;
            case IO.Output:
                Self.TargetThisLinks.Where(x => x.BuildInfo.TargetGroupName == container.BuildInfo.BuildString.Name).ForEach((x) => {
                    if (removedLinks.Contains(x))
                        return;
                    x.Link.RemoveLink();
                    x.Design.DestroyThis();
                    x.SourceScheme.SourceThisLinks.Remove(x);
                    Links.Remove(x);
                });
                removedLinks.ForEach(x => Self.TargetThisLinks.Remove(x));
                break;
        }
        removedLinks.Clear();
        (container.Design as IOSelfIOGroupDesign).DestroyThis();
        IOGroups.Remove(container.BuildInfo.BuildString.Name);
        IOGroupsInfo.Remove(container);
    }

	public void AddLink(LinkBuilder linkInfo)
	{
		var linkContainer = new LinkContainer (linkInfo, Schemes[linkInfo.SourceName], Schemes[linkInfo.TargetName]);

		Links.Add (linkContainer);

		linkContainer.Design = SchemeDesigner.Instance.CreateLinkDesign (linkContainer);
	}

	public UISchemeBuilder CreateBuilder()
	{
		var builder = new UISchemeBuilder ();

		builder.SelfSchemeBuildInfo = Self.SelfBuildInfo;
		builder.IOGroupsInfo = IOGroupsInfo.Select(x => x.BuildInfo).ToList();
		builder.InnerSchemesBuildInfo = InnerSchemes.Select (x => x.InnerBuildInfo).ToList ();
		builder.LinksBuildInfo = Links.Select (x => x.BuildInfo).ToList ();

		return builder;
	}
		
	public void DestroyThis()
	{
		foreach (var link in Links) 
		{
			link.Link.RemoveLink();
			link.Design.DestroyThis();
		}
		foreach (var innerScheme in InnerSchemes) 
		{
			innerScheme.Scheme.UnlinkAll();
			(innerScheme.Design as InnerSchemeDesign).DestroyThis();
		}
		foreach (var ioGroup in IOGroupsInfo)
		{
			(ioGroup.Design as IOSelfIOGroupDesign).DestroyThis();
		}
		UnlinkAll();
		(Self.Design as SelfSchemeDesign).DestroyThis();
	}

	#region InnerClasses

	public abstract class SchemeContainer
	{
		public Scheme Scheme;
		public List<LinkContainer> SourceThisLinks = new List<LinkContainer> (0);
		public List<LinkContainer> TargetThisLinks = new List<LinkContainer> (0);
		public SchemeDesign Design;
		public abstract string SchemeName { get; }
	}
		
	public class SelfContainer : SchemeContainer
	{
		public SelfContainer(UISelfSchemeBuildInfo selfInfo, Scheme scheme)
		{
			SelfBuildInfo = selfInfo;
			Scheme = scheme;
		}

		public UISelfSchemeBuildInfo SelfBuildInfo;

		public override string SchemeName {
			get {
				return SelfBuildInfo.Name;
			}
		}
	}

	public class InnerContainer : SchemeContainer
	{
		public InnerContainer(UIInnerSchemeBuildInfo buildInfo)
		{
			InnerBuildInfo = buildInfo.Clone();
			Scheme = SchemeManager.Instance.GetBuildingRule(InnerBuildInfo.BuildString.Type).Build (InnerBuildInfo.BuildString.Parameters);
		}

		public UIInnerSchemeBuildInfo InnerBuildInfo;

		public override string SchemeName {
			get {
				return InnerBuildInfo.BuildString.Name;
			}
		}
	}

	public class IOGroupContainer
	{
		public IOGroupContainer(UIIOGroupBuildInfo buildInfo, SchemeContainer parentScheme)
		{
			BuildInfo = buildInfo.Clone();
			IOGroup = new SchemeIOGroup(buildInfo.BuildString.Size, buildInfo.BuildString.IO);
			ParentScheme = parentScheme;
		}
			
		public SchemeIOGroup IOGroup;
		public SchemeContainer ParentScheme;
        public IOGroupDesign Design;


		public UIIOGroupBuildInfo BuildInfo;
	}
		
	public class LinkContainer
	{
		public LinkContainer(LinkBuilder builder, SchemeContainer source, SchemeContainer target)
		{
			BuildInfo = builder;
			SourceScheme = source;
			TargetScheme = target;
			Link = BuildInfo.Build(SourceScheme.Scheme,TargetScheme.Scheme);

			SourceScheme.SourceThisLinks.Add(this);
			TargetScheme.TargetThisLinks.Add(this);
		}

		public Link Link;
		public SchemeContainer SourceScheme, TargetScheme;
		public LinkDesign Design;

		public LinkBuilder BuildInfo;
	}

	#endregion InnerClasses
}


[Serializable]
public class UISchemeBuilder : SchemeBuilder
{
	public UISelfSchemeBuildInfo SelfSchemeBuildInfo;
	public List<UIIOGroupBuildInfo> IOGroupsInfo;
	public List<UIInnerSchemeBuildInfo> InnerSchemesBuildInfo;
	public List<LinkBuilder> LinksBuildInfo;

    public UISchemeBuilder Clone()
    {
        var newBuilder = new UISchemeBuilder();

        newBuilder.SelfSchemeBuildInfo = SelfSchemeBuildInfo.Clone();
        newBuilder.IOGroupsInfo = IOGroupsInfo.Select(x => x.Clone()).ToList();
        newBuilder.InnerSchemesBuildInfo = InnerSchemesBuildInfo.Select(x => x.Clone()).ToList();
        newBuilder.LinksBuildInfo = LinksBuildInfo.Select(x => x.Clone()).ToList();

        return newBuilder;
    }

	public override Scheme Build (string parameters)
	{
		var scheme = new Scheme ((byte)IOGroupsInfo.Count);

		Dictionary<string,Scheme> schemes = new Dictionary<string, Scheme> (InnerSchemesBuildInfo.Count + 1);
		schemes.Add (SelfSchemeBuildInfo.Name, scheme);

		foreach (var ioGroup in IOGroupsInfo)
			scheme.IOGroups.Add(ioGroup.BuildString.Name, new SchemeIOGroup(ioGroup.BuildString.Size, ioGroup.BuildString.IO));

		foreach (var innerSchemeBuildInfo in InnerSchemesBuildInfo)
		{
			var innerScheme = SchemeManager.Instance.GetBuildingRule(innerSchemeBuildInfo.BuildString.Type).Build(innerSchemeBuildInfo.BuildString.Parameters);
			scheme.InnerSchemesList.Add(innerScheme);
			schemes.Add(innerSchemeBuildInfo.BuildString.Name,innerScheme);
		}

		foreach (var innerLink in LinksBuildInfo) 
			innerLink.Build (schemes[innerLink.SourceName], schemes[innerLink.TargetName]);

		return scheme;
	}

	public UIScheme BuildUIScheme ()
	{
		var scheme = new UIScheme(SelfSchemeBuildInfo, (byte)IOGroupsInfo.Count, (byte)InnerSchemesBuildInfo.Count, (byte)LinksBuildInfo.Count);

		foreach (var ioGroup in IOGroupsInfo)
			scheme.AddIOGroup(ioGroup);

		foreach (var innerScheme in InnerSchemesBuildInfo)
			scheme.AddScheme (innerScheme);
		
		foreach (var link in LinksBuildInfo)
			scheme.AddLink (link);

		return scheme;
	}

	[Newtonsoft.Json.JsonIgnore]
	public override string DialogType {
		get {
			return "Scheme";
		}
	}
}

[Serializable]
public class UISelfSchemeBuildInfo
{
	public UISelfSchemeBuildInfo()
	{
	}
	public UISelfSchemeBuildInfo(string name)
	{
		Name = name;
	}

    public string Name;

    public UISelfSchemeBuildInfo Clone()
    {
        return new UISelfSchemeBuildInfo(Name);
    }
}

[Serializable]
public class UIIOGroupBuildInfo : IUpdateable
{
	public UIIOGroupBuildInfo()
	{
	}
	public UIIOGroupBuildInfo(string name, byte size, IO io, Vector2 position, Vector2 designSize) : this(new IOGroupBuildString(name,size,io), position, designSize)
	{
	}
	public UIIOGroupBuildInfo(IOGroupBuildString buildString, Vector2 position, Vector2 designSize)
	{
		BuildString = buildString;
		Position = position;
        Size = designSize;
	}

    public UIIOGroupBuildInfo Clone()
    {
        return new UIIOGroupBuildInfo(BuildString.Clone(), Position, Size);
    }

    public IOGroupBuildString BuildString;
    public Vector2 Position;
    public Vector2 Size;

    public int Version = LastVersion;
    private const int LastVersion = 1;

    private static int Migrate_0(JToken token)
    {
        token["Size"] = JToken.FromObject(new Vector2(IOSelfIOGroupDesign.MinCellWidth, token["BuildString"]["Size"].Value<byte>() * IOSelfIOGroupDesign.DefaultCellHeight), MyJsonSerializer.DefaultSerialiser);
        return 1;
    }
}

[Serializable]
public class UIInnerSchemeBuildInfo : IUpdateable
{
	public UIInnerSchemeBuildInfo()
	{
		
	}
	public UIInnerSchemeBuildInfo(string name,string type, string parameters, Vector2 position, Vector2 size) : this(new SchemeBuildString(name, type, parameters), position, size)
	{
		
	}
	public UIInnerSchemeBuildInfo(SchemeBuildString buildString, Vector2 position, Vector2 size)
	{
		BuildString = buildString;
		Position = position;
        Size = size;
	}

    public UIInnerSchemeBuildInfo Clone()
    {
        return new UIInnerSchemeBuildInfo(BuildString.Clone(), Position, Size);
    }

    public SchemeBuildString BuildString;

    public Vector2 Position;

    public Vector2 Size;


    public int Version = LastVersion;
    private const int LastVersion = 1;

    private static int Migrate_0(JToken token)
    {
        token["Size"] = JToken.FromObject(Vector2.one * 240, MyJsonSerializer.DefaultSerialiser);
        return 1;
    }
}
