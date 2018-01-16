using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomDialog : MonoBehaviour {

	public DialogResult DialogResult = DialogResult.NotReady;

	public abstract object Result { get; }
	public void Dispose()
	{
		Destroy(gameObject);
	}
}

public enum DialogResult
{
	NotReady,
	Ok,
	Cancel
}