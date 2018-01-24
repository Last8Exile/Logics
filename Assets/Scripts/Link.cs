using System.Collections;
using UnityEngine;
using System;

public class Link {

	private Scheme mSource,mTarget;
	private byte mSourceStart, mSourceCount, mTargetStart, mTargetCount;
	private string mSourceGroupName, mTargetGroupName;

	public Link(Scheme source, Scheme target, string sourceGroupName, string targetGroupName, byte sourceStart, byte sourceCount, byte targetStart, byte targetCount)
	{
		mSource = source;
		mTarget = target;
		mSourceGroupName = sourceGroupName;
		mTargetGroupName = targetGroupName;

		mSourceStart = sourceStart;
		mSourceCount = sourceCount;
		mTargetStart = targetStart;
		mTargetCount = targetCount;

		mSource.IOGroups[mSourceGroupName].IOChanged += OnSourceChanged;

		OnSourceChanged ();
	}

	public void RemoveLink()
	{
		mSource.IOGroups[mSourceGroupName].IOChanged -= OnSourceChanged;

		mTarget.SetIO(mTargetGroupName, new BitArray(1, false), 0, 1, mTargetStart, mTargetCount);
		mSource = null;
		mTarget = null;
	}

	private void OnSourceChanged()
	{
		mTarget.SetIO(mTargetGroupName, mSource.IOGroups[mSourceGroupName].IOArray, mSourceStart, mSourceCount, mTargetStart, mTargetCount);
	}
}

[Serializable]
public class LinkBuilder
{
	public LinkBuilder()
	{

	}
	public LinkBuilder(string sourceName,string targerName, string sourceGroupName, string targetGroupName, byte sourceStart, byte sourceCount, byte targetStart, byte targetCount)
	{
		SourceName = sourceName;
		TargetName = targerName;
		SourceGroupName = sourceGroupName;
		TargetGroupName = targetGroupName;
		SourceStart = sourceStart;
		SourceCount = sourceCount;
		TargetStart = targetStart;
		TargetCount = targetCount;
	}

    public LinkBuilder Clone()
    {
        return (LinkBuilder) MemberwiseClone();

    }
		
	public string SourceName, TargetName, SourceGroupName, TargetGroupName;
	public byte SourceStart,  SourceCount,  TargetStart,  TargetCount;

	public Link Build(Scheme source, Scheme target)
	{
		return new Link (source, target, SourceGroupName, TargetGroupName, SourceStart, SourceCount, TargetStart, TargetCount);
	}
}