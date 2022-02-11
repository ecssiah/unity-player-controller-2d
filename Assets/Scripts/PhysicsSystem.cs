using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhysicsSystem : MonoBehaviour
{
	private PhysicsSettings physicsSettings;

	private Player player;

	private float playerVelocityXDamped;
	private float playerVelocityXSmoothTime;

	public float hangTimer;
	private float wallSlideTimer;

	private List<Surface> surfaces;
	private List<Climbable> climbables;

	void Awake()
	{
		physicsSettings = Resources.Load<PhysicsSettings>("Settings/PhysicsSettings");

		player = GameObject.Find("Player").GetComponent<Player>();

		playerVelocityXSmoothTime = 0.1f;

		hangTimer = 0.4f;
		wallSlideTimer = 0.0f;

		surfaces = GameObject.Find("Surfaces").GetComponentsInChildren<Surface>().ToList();
		climbables = GameObject.Find("Climbables").GetComponentsInChildren<Climbable>().ToList();
	}

	void Update()
	{
		MovePlayer();
	}

	private void MovePlayer()
	{
		if (player.Hanging)
		{
			LedgeClimbCheck();
		}

		if (player.ClimbingLedge)
		{
			player.ClimbLedge();
		}
		else
		{
			ApplyForces();
			ResolveCollisions();
			GroundCheck();

			if (!player.Grounded)
			{
				ClimbCheck();
				WallSlideCheck();

				if (!player.Hanging)
				{
					LedgeCheck();
				}
			}

			if (!player.Hanging && !player.Climbing && player.WallSliding == 0)
			{
				if (player.Velocity.y > 0)
				{
					player.SetAnimation("Jump");
				}
				else if (player.Velocity.y < 0)
				{
					player.SetAnimation("Fall");
				}
				else if (player.Velocity.x != 0)
				{
					player.SetAnimation("Run");
				}
				else
				{
					player.SetAnimation("Idle");
				}
			}
		}

		Physics2D.SyncTransforms();

		player.UpdateOrientation();
	}

	private void LedgeClimbCheck()
	{
		if (hangTimer <= 0)
		{
			if (player.PlayerInputInfo.Direction.y > 0)
			{
				player.Hanging = false;
				player.ClimbingLedge = true;

				player.SetAnimation("LedgeClimb");
			}
		}
		else
		{
			hangTimer -= Time.deltaTime;
		}
	}

	private void ApplyForces()
	{
		Vector2 newVelocity = player.Velocity;

		if (player.Hanging)
		{
			newVelocity = Vector2.zero;
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
		else if (player.WallSliding != 0)
		{
			newVelocity.x = 0;
			newVelocity += Time.deltaTime * player.Mass * physicsSettings.Gravity;

			if (newVelocity.y < -player.WallSlideSpeed)
			{
				newVelocity.y = -player.WallSlideSpeed;
			}
		}
		else
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
			Vector2 resolutionVector = SeparatingAxisTheorem.CheckForCollisionResolution(player.BodyBox, surface.BodyBox);

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

	private void ClimbCheck()
	{
		if (player.Hanging)
		{
			return;
		}

		bool climbableContact = false;

		foreach (Climbable climbable in climbables)
		{
			if (SeparatingAxisTheorem.CheckForCollision(player.WallBox, climbable.BodyBox))
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

			player.SetAnimation("Climb");
			player.SetVelocity(0, 0);
		}
	}

	private void WallSlideCheck()
	{
		if (player.Hanging || player.Climbing)
		{
			wallSlideTimer = 0;
			player.WallSliding = 0;
			return;
		}

		bool wallContact = false;

		foreach (Surface surface in surfaces)
		{
			if (SeparatingAxisTheorem.CheckForCollision(player.WallBox, surface.BodyBox))
			{
				wallContact = true;
				break;
			}
		}

		if (wallContact)
		{
			if (player.PlayerInputInfo.Direction.x == -1 && player.CollisionInfo.Left)
			{
				wallSlideTimer = 0;
				player.WallSliding = -1;

				player.SetAnimation("Slide");
			}
			else if (player.PlayerInputInfo.Direction.x == 1 && player.CollisionInfo.Right)
			{
				wallSlideTimer = 0;
				player.WallSliding = 1;

				player.SetAnimation("Slide");
			}
		}

		if (player.WallSliding != 0)
		{
			if (!wallContact)
			{
				wallSlideTimer = 0;
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

	private void LedgeCheck()
	{
		bool handContact = false;

		foreach (Surface surface in surfaces)
		{
			if (SeparatingAxisTheorem.CheckForCollision(player.HandBox, surface.BodyBox))
			{
				handContact = true;
				break;
			}
		}

		if (handContact)
		{
			return;
		}

		Surface wallSurface = null;

		foreach (Surface surface in surfaces)
		{
			if (SeparatingAxisTheorem.CheckForCollision(player.WallBox, surface.BodyBox))
			{
				wallSurface = surface;
				break;
			}
		}

		bool canGrabLedge = wallSurface != null;

		if (canGrabLedge && player.PlayerInputInfo.Direction.y > 0)
		{
			hangTimer = 0.4f;

			player.Hanging = true;
			player.Climbing = false;

			player.SetAnimation("Hang");

			Vector2 hangPosition = player.Position;

			if (player.Facing == 1)
			{
				hangPosition = wallSurface.BodyBox.TopLeft;
				hangPosition.x -= physicsSettings.HangPositionOffset.x;
				hangPosition.y += physicsSettings.HangPositionOffset.y;
			}
			else if (player.Facing == -1)
			{
				hangPosition = wallSurface.BodyBox.TopRight;
				hangPosition.x += physicsSettings.HangPositionOffset.x;
				hangPosition.y += physicsSettings.HangPositionOffset.y;
			}

			player.SetPosition(hangPosition);
			player.SetVelocity(Vector2.zero);
		}
	}

	private void GroundCheck()
	{
		foreach (Surface surface in surfaces)
		{
			if (SeparatingAxisTheorem.CheckForCollision(player.GroundBox, surface.BodyBox))
			{
				player.Grounded = true;
				return;
			}
		}

		player.Grounded = false;
	}
}
