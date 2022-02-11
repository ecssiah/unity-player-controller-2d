using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct SeparatingAxisTheorem
{
	public static bool CheckForCollision(RectShape rect1, RectShape rect2)
	{
		List<Vector2> normals = rect1.Normals.Concat(rect2.Normals).ToList();

		foreach (Vector2 normal in normals)
		{
			if (IsSeparatingAxis(normal, rect1, rect2))
			{
				return false;
			}
		}

		return true;
	}

	public static bool IsSeparatingAxis(Vector2 normal, RectShape rect1, RectShape rect2)
	{
		float min1 = float.PositiveInfinity;
		float max1 = float.NegativeInfinity;

		float min2 = float.PositiveInfinity;
		float max2 = float.NegativeInfinity;

		foreach (Vector2 vertex in rect1.Vertices)
		{
			float projection = Vector2.Dot(vertex, normal);

			min1 = Mathf.Min(min1, projection);
			max1 = Mathf.Max(max1, projection);
		}

		foreach (Vector2 vertex in rect2.Vertices)
		{
			float projection = Vector2.Dot(vertex, normal);

			min2 = Mathf.Min(min2, projection);
			max2 = Mathf.Max(max2, projection);
		}

		return !(max1 >= min2 && max2 >= min1);
	}

	public static Vector2 CheckForCollisionResolution(RectShape rectResolve, RectShape rectCollide)
	{
		List<Vector2> resolutionVectors = new List<Vector2>();

		List<Vector2> normals = rectResolve.Normals.Concat(rectCollide.Normals).ToList();

		foreach (Vector2 normal in normals)
		{
			Vector2 resolutionVector = FindSeparatingAxis(normal, rectResolve, rectCollide);

			if (resolutionVector == Vector2.zero)
			{
				return resolutionVector;
			}
			else
			{
				resolutionVectors.Add(resolutionVector);
			}
		}

		Vector2 minResolutionVector = CalculateMinResolutionVector(resolutionVectors);

		Vector2 centerDisplacement = rectResolve.Center - rectCollide.Center;

		if (Vector2.Dot(centerDisplacement, minResolutionVector) < 0)
		{
			minResolutionVector *= -1;
		}

		return minResolutionVector;
	}
	
	public static Vector2 CalculateMinResolutionVector(List<Vector2> resolutionVectors)
	{
		Vector2 minResolutionVector = Vector2.positiveInfinity;
		float minMagnitudeSquared = float.PositiveInfinity;

		foreach (Vector2 resolutionVector in resolutionVectors)
		{
			float resolutionMagnitudeSquared = resolutionVector.sqrMagnitude;

			if (resolutionMagnitudeSquared < minMagnitudeSquared)
			{
				minResolutionVector = resolutionVector;
				minMagnitudeSquared = resolutionMagnitudeSquared;
			}
		}

		return minResolutionVector;
	}

	public static Vector2 FindSeparatingAxis(Vector2 normal, RectShape rect1, RectShape rect2)
	{
		float min1 = float.PositiveInfinity;
		float max1 = float.NegativeInfinity;

		float min2 = float.PositiveInfinity;
		float max2 = float.NegativeInfinity;

		foreach (Vector2 vertex in rect1.Vertices)
		{
			float projection = Vector2.Dot(vertex, normal);

			min1 = Mathf.Min(min1, projection);
			max1 = Mathf.Max(max1, projection);
		}

		foreach (Vector2 vertex in rect2.Vertices)
		{
			float projection = Vector2.Dot(vertex, normal);

			min2 = Mathf.Min(min2, projection);
			max2 = Mathf.Max(max2, projection);
		}

		if (max1 >= min2 && max2 >= min1)
		{
			float overlap = Mathf.Min(max2 - min1, max1 - min2);

			float resolutionMagnitude = overlap / normal.sqrMagnitude + 1E-10f;

			Vector2 resolutionVector = resolutionMagnitude * normal;

			return resolutionVector;
		}
		else
		{
			return Vector2.zero;
		}
	}
}
