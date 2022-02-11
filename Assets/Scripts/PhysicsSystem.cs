using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhysicsSystem : MonoBehaviour
{
	private GameSettings gameSettings;

	private Player player;

	private float playerVelocityXDamped;
	private float playerVelocityXSmoothTime;

	private float wallSlideDamping;

	public float hangTimer;

	private float wallSlideTimer;

	private List<Surface> surfaces;
	private List<Climbable> climbables;

	private struct WallTriggers
	{
		public Surface Top;
		public Surface Mid;
		public Surface Low;

		public void Reset()
		{
			Top = Mid = Low = null;
		}
	}

	private WallTriggers wallTriggers;

	void Awake()
	{
		gameSettings = Resources.Load<GameSettings>("Settings/GameSettings");

		player = GameObject.Find("Player").GetComponent<Player>();

		playerVelocityXSmoothTime = 0.1f;

		wallSlideDamping = 0.1f;

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
		else if (!player.Hanging)
		{
			ApplyForces();
			ResolveCollisions();

			GroundCheck();
			ClimbCheck();

			WallTriggersCheck();

			WallSlideCheck();
			LedgeCheck();
			
			player.UpdateAnimation();
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
			ApplyClimbingForces(ref newVelocity);	
		}
		else if (player.WallSliding != 0)
		{
			ApplyWallSlidingForces(ref newVelocity);
		}
		else
		{
			ApplyGeneralForces(ref newVelocity);
		}

		player.SetVelocity(newVelocity);

		player.Move(Time.deltaTime * player.Velocity);
	}

	private void ApplyClimbingForces(ref Vector2 newVelocity)
	{
		newVelocity.y = player.PlayerInputInfo.Direction.y * player.ClimbSpeed;

		newVelocity.x = Mathf.SmoothDamp(
			player.Velocity.x,
			player.PlayerInputInfo.Direction.x * player.Speed,
			ref playerVelocityXDamped,
			playerVelocityXSmoothTime
		);

		if (Mathf.Abs(newVelocity.x) < gameSettings.MinimumVelocity)
		{
			newVelocity.x = 0;
		}
	}

	private void ApplyWallSlidingForces(ref Vector2 newVelocity)
	{
		newVelocity.x = 0;
		newVelocity.y += wallSlideDamping * Time.deltaTime * player.Mass * gameSettings.Gravity;

		if (newVelocity.y < -player.WallSlideSpeed)
		{
			newVelocity.y = -player.WallSlideSpeed;
		}
	}

	private void ApplyGeneralForces(ref Vector2 newVelocity)
	{
		newVelocity.y += Time.deltaTime * player.Mass * gameSettings.Gravity;

		newVelocity.x = Mathf.SmoothDamp(
			player.Velocity.x,
			player.PlayerInputInfo.Direction.x * player.Speed,
			ref playerVelocityXDamped,
			playerVelocityXSmoothTime
		);

		if (Mathf.Abs(newVelocity.x) < gameSettings.MinimumVelocity)
		{
			newVelocity.x = 0;
		}

		if (newVelocity.y < gameSettings.TerminalVelocity)
		{
			newVelocity.y = gameSettings.TerminalVelocity;
		}
	}
	
	private void ResolveCollisions()
	{
		player.CollisionInfo.Reset();

		foreach (Surface surface in surfaces)
		{
			Vector2 resolutionVector = SeparatingAxisTheorem.CheckForCollisionResolution(player.BodyRectShape, surface.BodyRect);

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

	private void GroundCheck()
	{
		foreach (Surface surface in surfaces)
		{
			if (SeparatingAxisTheorem.CheckForCollision(player.GroundRectShape, surface.BodyRect))
			{
				player.Grounded = true;
				return;
			}
		}

		player.Grounded = false;
	}

	private void ClimbCheck()
	{
		if (player.Hanging)
		{
			return;
		}

		if (player.Climbing && player.Grounded)
		{
			player.Climbing = false;
		}

		bool climbableContact = false;

		foreach (Climbable climbable in climbables)
		{
			if (SeparatingAxisTheorem.CheckForCollision(player.BodyRectShape, climbable.BodyRect))
			{
				climbableContact = true;
				break;
			}
		}

		if (!climbableContact)
		{
			player.Climbing = false;
		} 
		else if (climbableContact && !player.Climbing && player.PlayerInputInfo.Direction.y != 0)
		{
			player.Climbing = true;

			player.SetAnimation("Climb");
			player.SetVelocity(0, 0);
		}
	}

	private void WallTriggersCheck()
	{
		wallTriggers.Reset();

		foreach (Surface surface in surfaces)
		{
			if (SeparatingAxisTheorem.CheckForCollision(player.WallTopRectShape, surface.BodyRect))
			{
				wallTriggers.Top = surface;
			}

			if (SeparatingAxisTheorem.CheckForCollision(player.WallMidRectShape, surface.BodyRect))
			{
				wallTriggers.Mid = surface;
			}

			if (SeparatingAxisTheorem.CheckForCollision(player.WallLowRectShape, surface.BodyRect))
			{
				wallTriggers.Low = surface;
			}
		}
	}

	private void WallSlideCheck()
	{
		if (player.Hanging || player.Climbing || player.Grounded)
		{
			wallSlideTimer = 0;
			player.WallSliding = 0;
			return;
		}

		bool wallContact = wallTriggers.Top && wallTriggers.Mid && wallTriggers.Low;

		if (wallContact)
		{
			if (player.CollisionInfo.Left && player.PlayerInputInfo.Direction.x == -1)
			{
				wallSlideTimer = 0;
				player.WallSliding = -1;

				player.SetAnimation("Slide");
				player.SetVelocity(player.Velocity.x, 0);
			}
			else if (player.CollisionInfo.Right && player.PlayerInputInfo.Direction.x == 1)
			{
				wallSlideTimer = 0;
				player.WallSliding = 1;

				player.SetAnimation("Slide");
				player.SetVelocity(player.Velocity.x, 0);
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
		if (wallTriggers.Top)
		{
			return;
		}

		bool canGrabLedge = wallTriggers.Mid;

		if (canGrabLedge && player.PlayerInputInfo.Direction.y > 0)
		{
			hangTimer = gameSettings.HangTime;

			player.Hanging = true;
			player.Climbing = false;

			player.SetAnimation("Hang");

			Vector2 hangPosition = player.Position;

			if (player.Facing == 1)
			{
				hangPosition = wallTriggers.Mid.BodyRect.TopLeft;
				hangPosition.x -= gameSettings.HangPositionOffset.x;
				hangPosition.y += gameSettings.HangPositionOffset.y;
			}
			else if (player.Facing == -1)
			{
				hangPosition = wallTriggers.Mid.BodyRect.TopRight;
				hangPosition.x += gameSettings.HangPositionOffset.x;
				hangPosition.y += gameSettings.HangPositionOffset.y;
			}

			player.SetPosition(hangPosition);
			player.SetVelocity(Vector2.zero);
		}
	}
}
