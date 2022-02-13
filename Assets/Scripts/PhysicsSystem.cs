using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhysicsSystem : MonoBehaviour
{
	private GameSettings gameSettings;

	private Player player;

	private float playerJumpImpulseDamping;

	private float playerSpeedDamped;
	private float playerSpeedSmoothTime;

	private List<Surface> surfaces;
	private List<Climbable> climbables;

	void Awake()
	{
		gameSettings = Resources.Load<GameSettings>("Settings/GameSettings");

		player = GameObject.Find("Player").GetComponent<Player>();

		playerJumpImpulseDamping = 0.9f;
		playerSpeedSmoothTime = 0.1f;

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
			player.ClimbLedgeCheck();
		}

		if (player.ClimbingLedge)
		{
			player.ClimbLedgeUpdate();
		}
		else if (!player.Hanging)
		{
			GroundCheck();
			ClimbTriggersCheck();
			WallTriggersCheck();
			
			player.WallSlideCheck();
			player.HangingCheck();

			ApplyForces();
			ResolveCollisions();
			
			player.UpdateAnimation();
		}

		player.UpdateOrientation();
		
		Physics2D.SyncTransforms();
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
			ApplyClimbForces(ref newVelocity);	
		}
		else if (player.WallSliding != 0)
		{
			ApplyWallSlideForces(ref newVelocity);
		}
		else
		{
			ApplyGeneralForces(ref newVelocity);
		}

		player.SetVelocity(newVelocity);

		player.Move(Time.deltaTime * player.Velocity);
	}

	private void ApplyClimbForces(ref Vector2 newVelocity)
	{
		newVelocity = Vector2.Scale(
			player.PlayerInputInfo.Direction, gameSettings.ClimbSpeed
		);

		if (Mathf.Abs(newVelocity.x) < gameSettings.MinSpeed)
		{
			newVelocity.x = 0;
		}
	}

	private void ApplyWallSlideForces(ref Vector2 newVelocity)
	{
		newVelocity.x = 0;
		newVelocity.y += gameSettings.WallSlideDamping * Time.deltaTime * player.Mass * gameSettings.Gravity;

		if (newVelocity.y < -gameSettings.MaxWallSlideSpeed)
		{
			newVelocity.y = -gameSettings.MaxWallSlideSpeed;
		}
	}

	private void ApplyGeneralForces(ref Vector2 newVelocity)
	{
		player.JumpImpulse *= playerJumpImpulseDamping;
		Vector2 appliedJumpImpulse = player.JumpImpulse;
		
		if (appliedJumpImpulse != Vector2.zero)
		{
			player.CurrentJumpForce += player.JumpImpulse;

			float magnitudeDifference = player.CurrentJumpForce.magnitude - gameSettings.MaxJumpSpeed;

			if (magnitudeDifference > 0)
			{
				player.SetJumpInput(0);
			}
		}
		
		newVelocity.x = Mathf.SmoothDamp(
			player.Velocity.x,
			player.PlayerInputInfo.Direction.x * player.Speed + appliedJumpImpulse.x,
			ref playerSpeedDamped,
			playerSpeedSmoothTime
		);

		if (Mathf.Abs(newVelocity.x) < gameSettings.MinSpeed)
		{
			newVelocity.x = 0;
		}

		newVelocity.y += Time.deltaTime * player.Mass * gameSettings.Gravity + appliedJumpImpulse.y;

		if (newVelocity.y < gameSettings.MaxFallSpeed)
		{
			newVelocity.y = gameSettings.MaxFallSpeed;
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

	private void ClimbTriggersCheck()
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

		if (!climbableContact && player.Climbing)
		{
			player.Climbing = false;
		} 
		else if (climbableContact && !player.Climbing && player.PlayerInputInfo.Direction.y != 0)
		{
			player.ClimbCheck();
		}
	}

	private void WallTriggersCheck()
	{
		player.TriggerInfo.Reset();

		foreach (Surface surface in surfaces)
		{
			if (SeparatingAxisTheorem.CheckForCollision(player.WallTopRectShape, surface.BodyRect))
			{
				player.TriggerInfo.Top = surface;
			}

			if (SeparatingAxisTheorem.CheckForCollision(player.WallMidRectShape, surface.BodyRect))
			{
				player.TriggerInfo.Mid = surface;
			}

			if (SeparatingAxisTheorem.CheckForCollision(player.WallLowRectShape, surface.BodyRect))
			{
				player.TriggerInfo.Low = surface;
			}
		}
	}
}
