using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Scheme 
{
	public Dictionary<string,SchemeIOGroup> IOGroups;
	public List<Scheme> InnerSchemesList = new List<Scheme>();

	public Scheme(byte inputGroupsSize = 0)
	{
		IOGroups = new Dictionary<string, SchemeIOGroup>(inputGroupsSize);
	}

	public virtual void SetIO(string groupName, BitArray values,byte valStart, byte valCount, byte ioStart, byte ioCount)
	{
		IOGroups[groupName].SetIO(values, valStart, valCount, ioStart, ioCount);
	}

	protected void RaiseChangedEvent(string groupName)
	{
		IOGroups[groupName].IOChanged_Invoke();
	}

	public virtual void UnlinkAll()
	{
		foreach (var group in IOGroups)
			group.Value.UnlinkAll();
		foreach (var scheme in InnerSchemesList)
			scheme.UnlinkAll();
		IOGroups.Clear();
		InnerSchemesList.Clear();
	}
}

[Serializable]
public class ComposedSchemeBuilder : SchemeBuilder
{
	public ComposedSchemeBuilder () 
	{
		
	}
	public ComposedSchemeBuilder(List<IOGroupBuildString> ioGroups, string name, List<SchemeBuildString> innerSchemes, List<LinkBuilder> innerLinks)
	{
		IOGroups = ioGroups;
		Name = name;
		InnerSchemes = innerSchemes;
		InnerLinks = innerLinks;
	}

	public List<IOGroupBuildString> IOGroups;
	public string Name;
	public List<SchemeBuildString> InnerSchemes;
	public List<LinkBuilder> InnerLinks;

	public override Scheme Build(string parameters)
	{
		var scheme = new Scheme((byte)IOGroups.Count);
		Dictionary<string,Scheme> schemes = new Dictionary<string, Scheme> (InnerSchemes.Count + 1);
		schemes.Add (Name, scheme);

		foreach (var group in IOGroups)
			scheme.IOGroups.Add(group.Name, new SchemeIOGroup(group.Size, group.IO));

		foreach (var buildString in InnerSchemes) 
		{
			schemes.Add (buildString.Name, SchemeManager.Instance.GetBuildingRule(buildString.Type).Build (buildString.Parameters));
		}

		foreach (var innerLink in InnerLinks) 
		{
			innerLink.Build(schemes[innerLink.SourceName], schemes[innerLink.TargetName]);
		}

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
public class SchemeBuildString
{
	public string Name,Type,Parameters;

	public SchemeBuildString() {}
	public SchemeBuildString(string name, string type, string parameters)
	{
		Name = name;
		Type = type;
		Parameters = parameters;
	}

    public SchemeBuildString Clone()
    {
        return new SchemeBuildString(Name, Type, Parameters);
    }
}

[Serializable]
public class IOGroupBuildString
{
	public string Name;
	public byte Size;
	public IO IO;

	public IOGroupBuildString()
	{
	}
	public IOGroupBuildString(string name, byte size, IO io)
	{
		Name = name;
		Size = size;
		IO = io;
	}

    public IOGroupBuildString Clone()
    {
        return new IOGroupBuildString(Name, Size, IO);
    }
}

public enum IO
{
	Input,
	Output
}
