using UnityEngine;

public class FocusArea
{
	private readonly RectShape targetRectShape;

	private Rect rect;

	public Vector2 Center => rect.center;
	public Vector2 Size => rect.size;
	public Vector2 Velocity;

	public FocusArea(RectShape _targetRectShape, Vector2 size)
	{
		targetRectShape = _targetRectShape;

		rect = new Rect(targetRectShape.Center, size);

		Velocity = Vector2.zero;
	}

	public void UpdatePosition()
	{
		Velocity = Vector2.zero;

		if (targetRectShape.Min.x < rect.min.x)
		{
			Velocity.x = targetRectShape.Min.x - rect.min.x;
		}
		else if (targetRectShape.Max.x > rect.max.x)
		{
			Velocity.x = targetRectShape.Max.x - rect.max.x;
		}

		if (targetRectShape.Min.y < rect.min.y)
		{
			Velocity.y = targetRectShape.Min.y - rect.min.y;
		}
		else if (targetRectShape.Max.y > rect.max.y)
		{
			Velocity.y = targetRectShape.Max.y - rect.max.y;
		}

		rect.center += Velocity;
	}
}