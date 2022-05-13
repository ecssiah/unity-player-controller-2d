   using UnityEngine;

namespace C0
{
	public class CameraFollow : MonoBehaviour
	{
		public bool DebugDraw;

		private GameObject targetObject;

		private Vector3 smoothDampVelocity;
		private float smoothDampTime;

		private Vector3 offset;

		private FocusArea focusArea;

		void Start()
		{
			DebugDraw = true;

			targetObject = GameObject.Find("Player");

			smoothDampTime = 0.05f;

			offset = Vector3.zero;
			focusArea = new FocusArea(targetObject.transform);
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

				Gizmos.DrawWireCube(focusArea.TargetCenter, focusArea.TargetSize);
				Gizmos.DrawWireCube(focusArea.Center + offset, focusArea.FocusSize);
			}
		}
	}
}