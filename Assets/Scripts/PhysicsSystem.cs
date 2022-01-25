using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhysicsSystem : MonoBehaviour
{
	private PhysicsSettings physicsSettings;

	private Player player;

	private List<Surface> surfaces;

	void Awake()
	{
		physicsSettings = Resources.Load<PhysicsSettings>("Settings/PhysicsSettings");

		player = GameObject.Find("Player").GetComponent<Player>();
		surfaces = new List<Surface>(GameObject.Find("Surfaces").GetComponentsInChildren<Surface>());
	}

	void FixedUpdate()
	{
		MovePlayer();
	}

	private void MovePlayer()
	{
		ApplyGravity();

		player.Move(Time.fixedDeltaTime * player.Velocity);

		ResolveCollisions();

		Physics2D.SyncTransforms();
	}

	private void ApplyGravity()
	{
		Vector2 newVelocity = player.Velocity + Time.fixedDeltaTime * physicsSettings.Gravity;

		if (newVelocity.y < physicsSettings.TerminalVelocity)
		{
			newVelocity.y = physicsSettings.TerminalVelocity;
		}

		player.SetVelocity(newVelocity);
	}

	private void ResolveCollisions()
	{
		foreach (Surface surface in surfaces)
		{
			Vector2 resolutionVector = Collide(surface.Polygon, player.Polygon);

			if (resolutionVector != Vector2.zero)
			{
				player.Move(resolutionVector);
				player.SetVelocity(player.Velocity.x, 0);
			}
		}
	}

	private Vector2 Collide(Polygon polygon1, Polygon polygon2)
	{
		List<Vector2> combinedNormals = polygon1.Normals.Concat(polygon2.Normals).ToList();

		List<Vector2> resolutionVectors = new List<Vector2>();

		foreach (Vector2 normal in combinedNormals)
		{
			Vector2 resolutionVector = FindSeparatingAxis(normal, polygon1, polygon2);

			if (resolutionVector == Vector2.zero)
			{
				return resolutionVector;
			}
			else
			{
				resolutionVectors.Add(resolutionVector);
			}
		}

		Vector2 mininumResolutionVector = Vector2.positiveInfinity;

		foreach (Vector2 resolutionVector in resolutionVectors)
		{
			float resolutionMagnitudeSquared = Vector2.Dot(resolutionVector, resolutionVector);
			float minimumMagnitudeSquared = Vector2.Dot(mininumResolutionVector, mininumResolutionVector);

			if (resolutionMagnitudeSquared < minimumMagnitudeSquared) 
			{
				mininumResolutionVector = resolutionVector;
			}
		}

		Vector2 centerDisplacement = polygon1.Center - polygon2.Center;

		if (Vector2.Dot(centerDisplacement, mininumResolutionVector) > 0)
		{
			mininumResolutionVector *= -1;
		}

		return mininumResolutionVector;
	}

	private Vector2 FindSeparatingAxis(Vector2 normal, Polygon polygon1, Polygon polygon2)
	{
		float min1 = float.PositiveInfinity; 
		float max1 = float.NegativeInfinity;

		float min2 = float.PositiveInfinity;
		float max2 = float.NegativeInfinity;

		foreach (Vector2 vertex in polygon1.Vertices)
		{
			float projection = Vector2.Dot(vertex, normal);

			min1 = Mathf.Min(min1, projection);
			max1 = Mathf.Max(max1, projection);
		}

		foreach (Vector2 vertex in polygon2.Vertices)
		{
			float projection = Vector2.Dot(vertex, normal);

			min2 = Mathf.Min(min2, projection);
			max2 = Mathf.Max(max2, projection);
		}

		if (max1 >= min2 && max2 >= min1)
		{
			float overlap = Mathf.Min(max2 - min1, max1 - min2);

			float resolutionMagnitude = overlap / Vector2.Dot(normal, normal) + 1E-10f;

			Vector2 resolutionVector = resolutionMagnitude * normal;
			
			return resolutionVector;
		}
		else
		{
			return Vector2.zero;
		}
	}
}
