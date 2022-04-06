using UnityEngine;

namespace C0
{
	public class FocusArea
	{
		private readonly Collider2D targetCollider;

		private Rect rect;

		public Vector2 Center => rect.center;
		public Vector2 Size => rect.size;
		public Vector2 Velocity;

		public FocusArea(Collider2D targetCollider, Vector2 size)
		{
			this.targetCollider = targetCollider;

			rect = new Rect(targetCollider.bounds.center, size);

			Velocity = Vector2.zero;
		}

		public void UpdatePosition()
		{
			Velocity = Vector2.zero;

			if (targetCollider.bounds.min.x < rect.min.x)
			{
				Velocity.x = targetCollider.bounds.min.x - rect.min.x;
			}
			else if (targetCollider.bounds.max.x > rect.max.x)
			{
				Velocity.x = targetCollider.bounds.max.x - rect.max.x;
			}

			if (targetCollider.bounds.min.y < rect.min.y)
			{
				Velocity.y = targetCollider.bounds.min.y - rect.min.y;
			}
			else if (targetCollider.bounds.max.y > rect.max.y)
			{
				Velocity.y = targetCollider.bounds.max.y - rect.max.y;
			}

			rect.center += Velocity;
		}
	}
}