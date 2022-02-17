using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public bool DebugDraw;

	private RectShape targetRectShape;

	private Vector2 smoothVelocity;
	private float smoothVelocityTime;

	private Vector2 offset;

	private FocusArea focusArea;

	void Start()
	{
		DebugDraw = false;

		targetRectShape = GameObject.Find("Player").GetComponent<RectShape>();
		
		smoothVelocityTime = 0.05f;

		offset = Vector2.zero;
		focusArea = new FocusArea(targetRectShape, new Vector2(3, 5));

		UpdateCameraPosition();
	}

	void LateUpdate()
	{
		UpdateCameraPosition();
	}

	private void UpdateCameraPosition()
	{
		focusArea.UpdatePosition();

		Vector2 focusPosition = focusArea.Center + offset;

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

			Gizmos.DrawWireCube(focusArea.Center + offset, focusArea.Size);
		}
	}
}
