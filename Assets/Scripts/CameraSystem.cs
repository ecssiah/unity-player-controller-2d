using UnityEngine;

namespace C0
{
	public class CameraSystem : MonoBehaviour
	{
		private Camera mainCamera;
		private GameObject targetObject;

		private FocusArea focusArea;

		private Vector3 velocityDamped;
		private float smoothDampTime;

		public void AwakeManaged()
		{
			mainCamera = GameObject.Find("Camera").GetComponent<Camera>();
			targetObject = GameObject.Find("Player/Target");

			focusArea = new FocusArea(targetObject.transform);

			smoothDampTime = 0.05f;
		}

		public void LateUpdateManaged()
		{
			focusArea.UpdatePosition();

			Vector3 focusPosition = Vector3.SmoothDamp(
				mainCamera.transform.position,
				focusArea.FocusCenter,
				ref velocityDamped,
				smoothDampTime
			);

			mainCamera.transform.position = focusPosition - 10 * Vector3.forward;
		}

		void OnDrawGizmos()
		{
			Gizmos.color = new Color(1, 0, 1, 0.1f);

			Gizmos.DrawWireCube(focusArea.TargetCenter, focusArea.TargetSize);
			Gizmos.DrawWireCube(focusArea.FocusCenter, focusArea.FocusSize);
		}
	}
}