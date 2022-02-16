using UnityEngine;

public struct FocusArea
{
	private readonly RectShape targetRectShape;

	public Vector2 Center;
	public Vector2 Velocity;

	float top, bottom;
	float left, right;

	public FocusArea(RectShape _targetRectShape, Vector2 size)
	{
		targetRectShape = _targetRectShape;

		left = targetRectShape.Center.x - size.x / 2;
		right = targetRectShape.Center.x + size.x / 2;
		bottom = targetRectShape.Center.y - size.y / 2;
		top = targetRectShape.Center.y + size.y / 2;

		Center = new Vector2((left + right) / 2, (top + bottom) / 2);
		Velocity = Vector2.zero;
	}

	public void UpdatePosition()
	{
		Velocity = Vector2.zero;

		if (targetRectShape.Min.x < left)
		{
			Velocity.x = targetRectShape.Min.x - left;
		}
		else if (targetRectShape.Max.x > right)
		{
			Velocity.x = targetRectShape.Max.x - right;
		}

		if (targetRectShape.Min.y < bottom)
		{
			Velocity.y = targetRectShape.Min.y - bottom;
		}
		else if (targetRectShape.Max.y > top)
		{
			Velocity.y = targetRectShape.Max.y - top;
		}

		left += Velocity.x;
		right += Velocity.x;
		top += Velocity.y;
		bottom += Velocity.y;

		Center = new Vector2((left + right) / 2, (top + bottom) / 2);
	}
}