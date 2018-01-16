using UnityEngine;

public class CameraController : MonoBehaviour {

	[SerializeField] private Camera mCamera= null;
	public float ZoomSpeed = 0.1f;
	public float MaxZoom = 4000, MinZoom = 100;

	private bool mDragStarted;
	private Vector3 mDragStartCameraPosition, mDragStartMousePosition;

	void Update () 
	{
		if (Input.GetMouseButton(1) && !Input.GetMouseButton(0))
		{
			if (mDragStarted)
			{
				transform.position = mDragStartCameraPosition - mCamera.ScreenToWorldPoint(Input.mousePosition) + mCamera.ScreenToWorldPoint(mDragStartMousePosition);
			}
			else
			{
				mDragStarted = true;
				mDragStartCameraPosition = transform.position;
				mDragStartMousePosition = Input.mousePosition;
			}
		}
		else
		{
			mDragStarted = false;
		}

		var scroll = Input.mouseScrollDelta.y;
		if (scroll != 0)
		{
			mCamera.orthographicSize = Mathf.Clamp(mCamera.orthographicSize * Mathf.Pow(2, -scroll * ZoomSpeed), MinZoom, MaxZoom);
		}
	}
}
