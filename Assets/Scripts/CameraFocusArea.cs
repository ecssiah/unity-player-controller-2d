using UnityEngine;

namespace C0
{
	public class FocusArea
	{
		private readonly Transform targetTransform;

		private Rect targetRect;
		public Vector3 TargetCenter => targetRect.center;
		public Vector3 TargetSize => targetRect.size;

		private Rect focusRect;
		public Vector3 Center => focusRect.center;
		public Vector3 FocusSize => focusRect.size;

		public Vector2 Velocity;

		public FocusArea(Transform targetTransform)
		{
			this.targetTransform = targetTransform;

			targetRect = new Rect
			{
				center = new Vector2(
					targetTransform.position.x,
					targetTransform.position.y + TargetSize.y / 2
				),
				size = new Vector3(0.5f, 1.6f)
			};

			focusRect = new Rect(Vector2.zero, new Vector3(3, 5));
			focusRect.position = new Vector2(
				targetTransform.position.x - focusRect.size.x / 2,
				targetTransform.position.y
			);

			Velocity = Vector2.zero;
		}

		public void UpdatePosition()
		{
			Velocity = Vector2.zero;

			targetRect.center = new Vector2(
				targetTransform.position.x, 
				targetTransform.position.y + TargetSize.y / 2
			);

			if (targetRect.xMin < focusRect.xMin)
			{
				Velocity.x = targetRect.xMin - focusRect.xMin;
			}
			else if (targetRect.xMax > focusRect.xMax)
			{
				Velocity.x = targetRect.xMax - focusRect.xMax;
			}

			if (targetRect.yMin < focusRect.yMin)
			{
				Velocity.y = targetRect.yMin - focusRect.yMin;
			}
			else if (targetRect.yMax > focusRect.yMax)
			{
				Velocity.y = targetRect.yMax - focusRect.yMax;
			}

			focusRect.center += Velocity;
		}
	}
}