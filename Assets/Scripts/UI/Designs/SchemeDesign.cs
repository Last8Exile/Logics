using UnityEngine;

public abstract class SchemeDesign : MonoBehaviour {

	public abstract IOBase IOBase(string groupName, byte number);
	public abstract UIScheme.SchemeContainer SchemeContainer { get; }

}
