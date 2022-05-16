using UnityEngine;

namespace C0
{
	public class CameraFollow : MonoBehaviour
	{
		public bool DebugDraw;

		private GameObject targetObject;

		private FocusArea focusArea;

		private Vector3 smoothDampVelocity;
		private float smoothDampTime;

		void Start()
		{
			DebugDraw = true;

			targetObject = GameObject.Find("Player/Target");

			smoothDampTime = 0.05f;

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
				focusArea.FocusCenter,
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
				Gizmos.DrawWireCube(focusArea.FocusCenter, focusArea.FocusSize);
			}
		}
	}
}