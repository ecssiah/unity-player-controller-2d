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
		surfaces = GameObject.Find("Surfaces").GetComponentsInChildren<Surface>().ToList();
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
		ResolveLedgeCollisions();

		player.UpdateAnimation();
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
			Vector2 resolutionVector = CheckForCollisionResolution(player.BodyPolygon, surface.BodyPolygon);

			if (resolutionVector != Vector2.zero)
			{
				player.Move(resolutionVector);

				player.SetVelocity(player.Velocity.x, 0);
			}
		}

		Physics2D.SyncTransforms();
	}

	private void ResolveLedgeCollisions()
	{

	}

	private Vector2 CheckForCollisionResolution(Polygon polygonToResolve, Polygon polygonToCollide)
	{
		List<Vector2> resolutionVectors = new List<Vector2>();
		
		List<Vector2> normals = polygonToResolve.Normals.Concat(polygonToCollide.Normals).ToList();

		foreach (Vector2 normal in normals)
		{
			Vector2 resolutionVector = FindSeparatingAxis(normal, polygonToResolve, polygonToCollide);

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

		Vector2 centerDisplacement = polygonToResolve.Center - polygonToCollide.Center;

		if (Vector2.Dot(centerDisplacement, minResolutionVector) < 0)
		{
			minResolutionVector *= -1;
		}

		return minResolutionVector;
	}

	private Vector2 CalculateMinResolutionVector(List<Vector2> resolutionVectors)
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
