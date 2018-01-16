using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SchemeManager : MonoBehaviour
{
	public static SchemeManager Instance 
	{
		get { 
			if (mInstance == null) {
				mInstance = GameObject.Find ("SchemeManager").GetComponent<SchemeManager> ();
				mInstance.Init ();
			}
			return mInstance;
		}
	}
	private static SchemeManager mInstance;

	public void Init ()
	{
		LoadSchemes();
		LoadBaseSchemes();
	}

	private void LoadBaseSchemes()
	{
		mBuildingRules[NAND.Type] = new NANDBuilder();
		mBuildingRules[CONST.Type] = new CONSTBuilder();
		mBuildingRules[DFF.Type] = new DFFBuilder();
		mBuildingRules[NANDX.Type] = new NANDXBuilder();
		mBuildingRules[ANDX.Type] = new ANDXBuilder();
		mBuildingRules[ORX.Type] = new ORXBuilder();
		mBuildingRules[NOTX.Type] = new NOTXBuilder();
		mBuildingRules[XORX.Type] = new XORXBuilder();
		mBuildingRules[RAMX.Type] = new RAMXBuilder();
		if (SchemesChanged != null)
			SchemesChanged.Invoke();
	}

	private Dictionary<string,SchemeBuilder> mBuildingRules;

	public SchemeBuilder GetBuildingRule(string type)
	{
		return mBuildingRules[type];
	}

	public void AddOrUpdateBuildingRule(string type, SchemeBuilder builder)
	{
		mBuildingRules[type] = builder;
		if (SchemesChanged != null)
			SchemesChanged.Invoke();
	}

	public List<KeyValuePair<string,SchemeBuilder>> BuildingRules()
	{
		return mBuildingRules.ToList();
	}
	
	public Scheme BuildScheme(string type, string parameters)
	{
		return mBuildingRules[type].Build (parameters);
	}

	public void LoadSchemes()
	{
		var serialisedRules = ApplicationSettings.Load("BuildingRules");
		try
		{
			mBuildingRules = MyJsonSerializer.Deserialize<Dictionary<string,SchemeBuilder>>(serialisedRules);
			if (SchemesChanged != null)
				SchemesChanged.Invoke();
		}
		catch (Exception)
		{
			Console.Instance.Log("Произошла ошибка при загрузке схем");
			ResetBuildungRules();
		}
	}

	public void ResetBuildungRules()
	{
		var buildingRules = new Dictionary<string, SchemeBuilder>();
		mBuildingRules = buildingRules;
		LoadBaseSchemes();
	}

	public void SaveSchemes()
	{
		var serialisedRules = MyJsonSerializer.Serialize(mBuildingRules);
		ApplicationSettings.Save("BuildingRules", serialisedRules);
	}

	public event Action SchemesChanged;
}

[Serializable]
public abstract class SchemeBuilder
{
	public SchemeBuilder() {}
	public abstract Scheme Build (string parameters);
	public abstract string DialogType { get; }
}
