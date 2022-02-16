using UnityEngine;

public class FocusArea
{
	private readonly RectShape targetRectShape;

	public Vector2 Center;
	public Vector2 Velocity;

	private Vector2 min;
	private Vector2 max;

	public FocusArea(RectShape _targetRectShape, Vector2 size)
	{
		targetRectShape = _targetRectShape;
		
		min = new Vector2(targetRectShape.Center.x - size.x / 2, targetRectShape.Center.y - size.y / 2);
		max = new Vector2(targetRectShape.Center.x + size.x / 2, targetRectShape.Center.y + size.y / 2);

		Center = (min + max) / 2;
		Velocity = Vector2.zero;
	}

	public void UpdatePosition()
	{
		Velocity = Vector2.zero;

		if (targetRectShape.Min.x < min.x)
		{
			Velocity.x = targetRectShape.Min.x - min.x;
		}
		else if (targetRectShape.Max.x > max.x)
		{
			Velocity.x = targetRectShape.Max.x - max.x;
		}

		if (targetRectShape.Min.y < min.y)
		{
			Velocity.y = targetRectShape.Min.y - min.y;
		}
		else if (targetRectShape.Max.y > max.y)
		{
			Velocity.y = targetRectShape.Max.y - max.y;
		}

		min += Velocity;
		max += Velocity;

		Center = (min + max) / 2;
	}
}