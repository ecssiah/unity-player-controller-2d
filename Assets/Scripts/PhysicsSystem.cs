using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhysicsSystem : MonoBehaviour
{
	private GameSettings gameSettings;

	private Player player;

	private float playerSpeedDamped;

	private List<Surface> surfaces;
	private List<Climbable> climbables;

	void Awake()
	{
		gameSettings = Resources.Load<GameSettings>("Settings/GameSettings");

		player = GameObject.Find("Player").GetComponent<Player>();

		surfaces = GameObject.Find("Surfaces").GetComponentsInChildren<Surface>().ToList();
		climbables = GameObject.Find("Climbables").GetComponentsInChildren<Climbable>().ToList();
	}

	void Update()
	{
		player.HangCheck();

		if (player.ClimbingLedge)
		{
			player.ClimbLedgeUpdate();
		}

		if (!player.Hanging)
		{
			MovePlayer();

			GroundCheck();
			ClimbTriggersCheck();
			WallTriggersCheck();

			player.WallSlideCheck();
			
			player.UpdateAnimation();
		}

		player.UpdateOrientation();

		Physics2D.SyncTransforms();
	}

	private void MovePlayer()
	{
		ApplyForces();

		player.Move(Time.deltaTime * player.Velocity);

		ResolveCollisions();
	}

	private void ApplyForces()
	{
		Vector2 newVelocity = player.Velocity;

		if (player.Climbing)
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

		if (Mathf.Abs(newVelocity.x) < gameSettings.MinSpeed)
		{
			newVelocity.x = 0;
		}

		player.SetVelocity(newVelocity);
	}

	private void ApplyClimbForces(ref Vector2 newVelocity)
	{
		newVelocity = Vector2.Scale(
			player.PlayerInputInfo.Direction, gameSettings.ClimbSpeed
		);
	}

	private void ApplyWallSlideForces(ref Vector2 newVelocity)
	{
		newVelocity.x = 0;
		newVelocity.y += gameSettings.WallSlideDamping * Time.deltaTime * gameSettings.Gravity;

		if (newVelocity.y < -gameSettings.MaxWallSlideSpeed)
		{
			newVelocity.y = -gameSettings.MaxWallSlideSpeed;
		}
	}

	private void ApplyGeneralForces(ref Vector2 newVelocity)
	{
		newVelocity.x = Mathf.SmoothDamp(
			player.Velocity.x,
			player.PlayerInputInfo.Direction.x * gameSettings.RunSpeed,
			ref playerSpeedDamped,
			gameSettings.SpeedSmoothTime
		);
		
		newVelocity.y += Time.deltaTime * gameSettings.Gravity;

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

				if (resolutionVector.x != 0)
				{
					player.SetVelocity(0, player.Velocity.y);

					if (resolutionVector.x > 0)
					{
						player.CollisionInfo.Left = true;
					}
					else if (resolutionVector.x < 0)
					{
						player.CollisionInfo.Right = true;
					}
				}

				if (resolutionVector.y != 0)
				{
					player.SetVelocity(player.Velocity.x, 0);

					if (resolutionVector.y > 0)
					{
						player.CollisionInfo.Bottom = true;
					}
					else if (resolutionVector.y < 0)
					{
						player.CollisionInfo.Top = true;
					}
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

		player.TriggerInfo.ClimbableTrigger = climbableContact;
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
