using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	private struct FocusArea
	{
		public Vector2 Center;
		public Vector2 Velocity;

		float top, bottom;
		float left, right;

		public FocusArea(Bounds targetBounds, Vector2 size)
		{
			left = targetBounds.center.x - size.x / 2;
			right = targetBounds.center.x + size.x / 2;
			bottom = targetBounds.min.y;
			top = targetBounds.min.y + size.y;

			Center = new Vector2((left + right) / 2, (top + bottom) / 2);
			Velocity = Vector2.zero;
		}

		public void Update(Bounds targetBounds)
		{
			float shiftX = 0;

			if (targetBounds.min.x < left)
			{
				shiftX = targetBounds.min.x - left;
			}
			else if (targetBounds.max.x > right)
			{
				shiftX = targetBounds.max.x - right;
			}

			left += shiftX;
			right += shiftX;

			float shiftY = 0;

			if (targetBounds.min.y < bottom)
			{
				shiftY = targetBounds.min.y - bottom;
			}
			else if (targetBounds.max.y > top)
			{
				shiftY = targetBounds.max.y - top;
			}

			top += shiftY;
			bottom += shiftY;

			Center = new Vector2((left + right) / 2, (top + bottom) / 2);

			Velocity = new Vector2(shiftX, shiftY);
		}
	}

	public bool DebugDraw;

	private BoxCollider2D targetCollider;

	private FocusArea focusArea;

	private Vector2 smoothVelocity;
	private Vector2 smoothVelocityTime;

	public Vector2 focusAreaSize;
	public float verticalOffset;

	void Awake()
	{
		DebugDraw = false;

		smoothVelocityTime = new Vector2(0.05f, 0.05f);
		
		focusAreaSize = new Vector2(3, 5);

		targetCollider = GameObject.Find("Player").GetComponent<BoxCollider2D>();

		focusArea = new FocusArea(targetCollider.bounds, focusAreaSize);
	}

    void LateUpdate()
    {
		focusArea.Update(targetCollider.bounds);

		Vector2 focusPosition = focusArea.Center + verticalOffset * Vector2.up;

		focusPosition.x = Mathf.SmoothDamp(
			transform.position.x, focusPosition.x, ref smoothVelocity.x, smoothVelocityTime.x
		);

		focusPosition.y = Mathf.SmoothDamp(
			transform.position.y, focusPosition.y, ref smoothVelocity.y, smoothVelocityTime.y
		);

		transform.position = (Vector3)focusPosition + Vector3.forward * -10;
	}

	void OnDrawGizmos()
	{
		if (DebugDraw)
		{
			Gizmos.color = new Color(1, 0, 1, 0.1f);

			Gizmos.DrawCube(focusArea.Center, focusAreaSize);
		}
	}
}
