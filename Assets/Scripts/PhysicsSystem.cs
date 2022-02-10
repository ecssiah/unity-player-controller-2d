using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhysicsSystem : MonoBehaviour
{
	private PhysicsSettings physicsSettings;

	private Player player;

	private float playerVelocityXDamped;
	private float playerVelocityXSmoothTime;

	private float wallSlideTimer;

	private List<Surface> surfaces;
	private List<Climbable> climbables;

	void Awake()
	{
		physicsSettings = Resources.Load<PhysicsSettings>("Settings/PhysicsSettings");

		player = GameObject.Find("Player").GetComponent<Player>();

		playerVelocityXSmoothTime = 0.1f;

		wallSlideTimer = 0.0f;

		surfaces = GameObject.Find("Surfaces").GetComponentsInChildren<Surface>().ToList();
		climbables = GameObject.Find("Climbables").GetComponentsInChildren<Climbable>().ToList();
	}

	void LateUpdate()
	{
		MovePlayer();
	}

	private void MovePlayer()
	{
		ApplyForces();
		
		ResolveCollisions();

		LedgeCheck();
		ClimbCheck();
		WallSlideCheck();
		GroundCheck();

		Physics2D.SyncTransforms();

		player.UpdateAnimation();
	}

	private void ApplyForces()
	{
		Vector2 newVelocity = player.Velocity;

		if (player.WallSliding != 0 && newVelocity.y < -player.WallSlideSpeed)
		{
			newVelocity.y = -player.WallSlideSpeed;
		}
		else if (player.Climbing)
		{
			newVelocity.y = player.PlayerInputInfo.Direction.y * player.ClimbSpeed;

			newVelocity.x = Mathf.SmoothDamp(
				player.Velocity.x,
				player.PlayerInputInfo.Direction.x * player.Speed,
				ref playerVelocityXDamped,
				playerVelocityXSmoothTime
			);

			if (Mathf.Abs(newVelocity.x) < 0.1f)
			{
				newVelocity.x = 0;
			}
		}
		else if (!player.Hanging)
		{
			newVelocity += Time.deltaTime * player.Mass * physicsSettings.Gravity;

			newVelocity.x = Mathf.SmoothDamp(
				player.Velocity.x,
				player.PlayerInputInfo.Direction.x * player.Speed,
				ref playerVelocityXDamped,
				playerVelocityXSmoothTime
			);

			if (Mathf.Abs(newVelocity.x) < 0.1f)
			{
				newVelocity.x = 0;
			}

			if (newVelocity.y < physicsSettings.TerminalVelocity)
			{
				newVelocity.y = physicsSettings.TerminalVelocity;
			}
		}

		player.SetVelocity(newVelocity);

		player.Move(Time.deltaTime * player.Velocity);
	}

	private void ResolveCollisions()
	{
		player.CollisionInfo.Reset();

		foreach (Surface surface in surfaces)
		{
			Vector2 resolutionVector = CheckForCollisionResolution(player.BodyBox, surface.BodyBox);

			if (resolutionVector != Vector2.zero)
			{
				player.Move(resolutionVector);

				if (resolutionVector.x > 0)
				{
					player.CollisionInfo.Left = true;
					player.SetVelocity(0, player.Velocity.y);
				}
				else if (resolutionVector.x < 0)
				{
					player.CollisionInfo.Right = true;
					player.SetVelocity(0, player.Velocity.y);
				}

				if (resolutionVector.y > 0)
				{
					player.CollisionInfo.Bottom = true;
					player.SetVelocity(player.Velocity.x, 0);
				}
				else if (resolutionVector.y < 0)
				{
					player.CollisionInfo.Top = true;
					player.SetVelocity(player.Velocity.x, 0);
				}
			}
		}
	}

	private void LedgeCheck()
	{
		if (player.Grounded)
		{
			return;
		}

		bool handContact = false;

		foreach (Surface surface in surfaces)
		{
			if (CheckForCollision(player.HandBox, surface.BodyBox))
			{
				handContact = true;
				break;
			}
		}

		bool wallContact = false;
		BoxShape wallShape;

		foreach (Surface surface in surfaces)
		{
			if (CheckForCollision(player.WallBox, surface.BodyBox))
			{
				wallContact = true;
				wallShape = surface.BodyBox;
				break;
			}
		}

		bool canLedgeClimb = wallContact && !handContact;

		if (canLedgeClimb && player.PlayerInputInfo.Direction.y > 0)
		{
			player.Climbing = false;
			player.Hanging = true;
			player.SetVelocity(Vector2.zero);
		}
	}

	private void ClimbCheck()
	{
		if (player.Hanging)
		{
			return;
		}

		bool climbableContact = false;

		foreach (Climbable climbable in climbables)
		{
			if (CheckForCollision(player.WallBox, climbable.BodyBox))
			{
				climbableContact = true;
				break;
			}
		}

		if (!climbableContact)
		{
			player.Climbing = false;
		} 
		else if (!player.Climbing && player.PlayerInputInfo.Direction.y != 0)
		{
			player.Climbing = true;
			player.WallSliding = 0;

			player.SetVelocity(0, player.Velocity.y);
		}
	}

	private void WallSlideCheck()
	{
		if (player.Grounded || player.Climbing)
		{
			wallSlideTimer = 0;
			player.WallSliding = 0;
			return;
		}

		bool wallContact = false;

		foreach (Surface surface in surfaces)
		{
			if (CheckForCollision(player.WallBox, surface.BodyBox))
			{
				wallContact = true;
				break;
			}
		}

		if (wallContact)
		{
			if (player.Facing == -1 && player.CollisionInfo.Left)
			{
				wallSlideTimer = 0;
				player.WallSliding = -1;
			}
			else if (player.Facing == 1 && player.CollisionInfo.Right)
			{
				wallSlideTimer = 0;
				player.WallSliding = 1;
			}
		}

		if (player.WallSliding != 0)
		{
			if (!wallContact)
			{
				player.WallSliding = 0;
			}
			else if (player.PlayerInputInfo.Direction.x != player.WallSliding)
			{
				wallSlideTimer += Time.deltaTime;

				if (wallSlideTimer >= player.WallSlideStickTime)
				{
					wallSlideTimer = 0;
					player.WallSliding = 0;
				}
			}
		}
	}

	private void GroundCheck()
	{
		foreach (Surface surface in surfaces)
		{
			if (CheckForCollision(player.GroundBox, surface.BodyBox))
			{
				player.Grounded = true;
				return;
			}
		}

		player.Grounded = false;
	}

	private bool CheckForCollision(BoxShape polygon1, BoxShape polygon2)
	{
		List<Vector2> normals = polygon1.Normals.Concat(polygon2.Normals).ToList();

		foreach (Vector2 normal in normals)
		{
			if (IsSeparatingAxis(normal, polygon1, polygon2))
			{
				return false;
			}
		}

		return true;
	}
	
	private bool IsSeparatingAxis(Vector2 normal, BoxShape polygon1, BoxShape polygon2)
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
		
		return !(max1 >= min2 && max2 >= min1);
	}

	private Vector2 FindSeparatingAxis(Vector2 normal, BoxShape polygon1, BoxShape polygon2)
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

	private Vector2 CheckForCollisionResolution(BoxShape polygonToResolve, BoxShape polygonToCollide)
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
}
