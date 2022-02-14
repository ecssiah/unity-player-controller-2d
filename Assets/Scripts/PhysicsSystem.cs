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
		if (player.Hanging)
		{
			player.ClimbLedgeCheck();
		} 
		else if (player.ClimbingLedge)
		{
			player.ClimbLedgeUpdate();
		} 
		else
		{
			player.UpdateState();
			
			ApplyForces();

			player.Move(Time.deltaTime * player.Velocity);
			
			ResolveCollisions();

			player.TriggerInfo.Reset();

			GroundTriggerCheck();
			ClimbTriggersCheck();
			WallTriggersCheck();
		}

		Physics2D.SyncTransforms();
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
		
		if (player.Position.y < -30)
		{
			player.SetPosition(0, 3);
			player.SetVelocity(0, 0);
		}
		else
		{
			player.SetVelocity(newVelocity);
		}
	}

	private void ApplyClimbForces(ref Vector2 newVelocity)
	{
		newVelocity.x = player.PlayerInputInfo.Direction.x * gameSettings.ClimbSpeed.x;
		newVelocity.y = player.PlayerInputInfo.Direction.y * gameSettings.ClimbSpeed.y;
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

		if (Mathf.Abs(newVelocity.x) < gameSettings.MinSpeed)
		{
			newVelocity.x = 0;
		}

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
			Vector2 resolutionVector = SeparatingAxisTheorem.CheckForCollisionResolution(
				player.BodyRectShape, surface.BodyRect
			);

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

	private void GroundTriggerCheck()
	{
		foreach (Surface surface in surfaces)
		{
			if (SeparatingAxisTheorem.CheckForCollision(player.GroundRectShape, surface.BodyRect))
			{
				player.TriggerInfo.Grounded = true;
				return;
			}
		}
	}

	private void ClimbTriggersCheck()
	{
		foreach (Climbable climbable in climbables)
		{
			if (SeparatingAxisTheorem.CheckForCollision(player.BodyRectShape, climbable.BodyRect))
			{
				player.TriggerInfo.Climbable = true;
				break;
			}
		}

	}

	private void WallTriggersCheck()
	{
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
