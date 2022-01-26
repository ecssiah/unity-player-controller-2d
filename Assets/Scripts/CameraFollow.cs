using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	private struct FocusArea
	{
		public Vector2 center;
		public Vector2 velocity;

		float top, bottom;
		float left, right;

		public FocusArea(Bounds targetBounds, Vector2 size)
		{
			left = targetBounds.center.x - size.x / 2;
			right = targetBounds.center.x + size.x / 2;
			bottom = targetBounds.min.y;
			top = targetBounds.min.y + size.y;

			velocity = Vector2.zero;

			center = new Vector2((left + right) / 2, (top + bottom) / 2);
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

			center = new Vector2((left + right) / 2, (top + bottom) / 2);

			velocity = new Vector2(shiftX, shiftY);
		}
	}

	private BoxCollider2D targetCollider;

	public Vector2 focusAreaSize;
	private FocusArea focusArea;

	public float verticalOffset;

	void Awake()
	{
		focusAreaSize = new Vector2(3, 5);

		targetCollider = GameObject.Find("Player").GetComponent<BoxCollider2D>();

		focusArea = new FocusArea(targetCollider.bounds, focusAreaSize);
	}

    void LateUpdate()
    {
		focusArea.Update(targetCollider.bounds);

		Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;

		transform.position = (Vector3)focusPosition + Vector3.forward * -10;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = new Color(1, 0, 1, 0.3f);

		Gizmos.DrawCube(focusArea.center, focusAreaSize);
	}
}
