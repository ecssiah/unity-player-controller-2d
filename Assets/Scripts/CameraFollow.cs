using UnityEngine;

namespace C0
{
	public class CameraFollow : MonoBehaviour
	{
		public bool DebugDraw;

		private CapsuleCollider2D targetCollider;

		private Vector3 smoothDampVelocity;
		private float smoothDampTime;

		private Vector2 offset;

		private FocusArea focusArea;

		void Start()
		{
			DebugDraw = false;

			targetCollider = GameObject.Find("Player").GetComponent<CapsuleCollider2D>();

			smoothDampTime = 0.05f;

			offset = Vector2.zero;
			focusArea = new FocusArea(targetCollider, new Vector2(3, 5));
		}

		void LateUpdate()
		{
			UpdateCameraPosition();
		}

		private void UpdateCameraPosition()
		{
			focusArea.UpdatePosition();

			Vector3 focusPosition = Vector3.SmoothDamp(
				transform.position,
				focusArea.Center + offset,
				ref smoothDampVelocity,
				smoothDampTime
			);

			transform.position = focusPosition - 10 * Vector3.forward;
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
}