using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public bool DebugDraw;

	private RectShape targetRectShape;

	private Vector2 smoothVelocity;
	private float smoothVelocityTime;

	private float verticalOffset;
	private Vector2 focusAreaSize;

	private FocusArea focusArea;

	void Start()
	{
		DebugDraw = false;

		targetRectShape = GameObject.Find("Player").GetComponent<RectShape>();
		
		smoothVelocityTime = 0.05f;

		verticalOffset = 0;
		focusAreaSize = new Vector2(3, 5);
		focusArea = new FocusArea(targetRectShape, focusAreaSize);

		UpdateCameraPosition();
	}

	void LateUpdate()
	{
		UpdateCameraPosition();
	}

	private void UpdateCameraPosition()
	{
		focusArea.UpdatePosition();

		Vector2 focusPosition = focusArea.Center + verticalOffset * Vector2.up;

		focusPosition = Vector2.SmoothDamp(
			transform.position, focusPosition, ref smoothVelocity, smoothVelocityTime
		);

		transform.position = (Vector3)focusPosition + Vector3.forward * -10;
	}

	void OnDrawGizmos()
	{
		if (DebugDraw)
		{
			Gizmos.color = new Color(1, 0, 1, 0.1f);

			Gizmos.DrawWireCube(focusArea.Center, focusAreaSize);
		}
	}
}
