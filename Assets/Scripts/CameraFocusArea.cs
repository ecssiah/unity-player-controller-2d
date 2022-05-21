using UnityEngine;

namespace C0
{
	public class FocusArea
	{
		private readonly GameSettings gameSettings;

		private Transform targetTransform;

		private Rect targetRect;
		public Vector3 TargetCenter => targetRect.center;
		public Vector3 TargetSize => targetRect.size;

		private Rect focusRect;
		public Vector3 FocusCenter => focusRect.center;
		public Vector3 FocusSize => focusRect.size;

		public FocusArea(Transform targetTransform)
		{
			this.targetTransform = targetTransform;

			gameSettings = Resources.Load<GameSettings>("Settings/GameSettings");

			targetRect = new Rect
			{
				center = new Vector2(
					targetTransform.position.x,
					targetTransform.position.y + gameSettings.CameraTargetSize.y / 2
				),
				size = gameSettings.CameraTargetSize
			};

			focusRect = new Rect
			{
				center = new Vector2(
					targetTransform.position.x - gameSettings.FocusTargetSize.x / 2,
					targetTransform.position.y
				),
				size = gameSettings.FocusTargetSize
			};
		}

		public void UpdatePosition()
		{
			Vector2 velocity = Vector2.zero;

			targetRect.center = new Vector2(
				targetTransform.position.x,
				targetTransform.position.y + targetRect.size.y / 2
			);

			if (targetRect.xMin < focusRect.xMin)
			{
				velocity.x = targetRect.xMin - focusRect.xMin;
			}
			else if (targetRect.xMax > focusRect.xMax)
			{
				velocity.x = targetRect.xMax - focusRect.xMax;
			}

			if (targetRect.yMin < focusRect.yMin)
			{
				velocity.y = targetRect.yMin - focusRect.yMin;
			}
			else if (targetRect.yMax > focusRect.yMax)
			{
				velocity.y = targetRect.yMax - focusRect.yMax;
			}

			focusRect.center += velocity;
		}
	}
}