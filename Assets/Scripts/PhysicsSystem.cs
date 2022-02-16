using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhysicsSystem : MonoBehaviour
{
	private GameSettings gameSettings;

	private Player player;

	private List<RectShape> surfaces;
	private List<RectShape> climbables;

	void Awake()
	{
		gameSettings = Resources.Load<GameSettings>("Settings/GameSettings");

		player = GameObject.Find("Player").GetComponent<Player>();

		surfaces = GameObject.Find("Surfaces").GetComponentsInChildren<RectShape>().ToList();
		climbables = GameObject.Find("Climbables").GetComponentsInChildren<RectShape>().ToList();
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
		else if (!player.Hanging)
		{
			ApplyGeneralForces(ref newVelocity);
		}

		player.SetVelocity(newVelocity);
	}

	private void ApplyClimbForces(ref Vector2 newVelocity)
	{
		newVelocity.x = player.InputInfo.Direction.x * gameSettings.ClimbSpeed.x;
		newVelocity.y = player.InputInfo.Direction.y * gameSettings.ClimbSpeed.y;
	}

	private void ApplyWallSlideForces(ref Vector2 newVelocity)
	{
		newVelocity.x = 0;
		newVelocity.y += gameSettings.WallSlideDamping * Time.deltaTime * gameSettings.Gravity;
		newVelocity.y = Mathf.Max(newVelocity.y, -gameSettings.MaxWallSlideSpeed);
	}

	private void ApplyGeneralForces(ref Vector2 newVelocity)
	{
		newVelocity.x = Mathf.SmoothDamp(
			newVelocity.x,
			player.InputInfo.Direction.x * gameSettings.RunSpeed,
			ref player.CurrentHorizontalSpeed,
			player.TriggerInfo.Grounded ? gameSettings.GroundSpeedSmoothTime : gameSettings.AirSpeedSmoothTime
		);

		if (Mathf.Abs(newVelocity.x) < gameSettings.MinSpeed)
		{
			newVelocity.x = 0;
		}

		newVelocity.y += Time.deltaTime * gameSettings.Gravity;
		newVelocity.y = Mathf.Max(newVelocity.y, gameSettings.MaxFallSpeed);

		if (player.Position.y < -30)
		{
			player.SetPosition(0, 3);
			newVelocity = Vector2.zero;
		}
	}

	private void ResolveCollisions()
	{
		player.CollisionInfo.Reset();

		foreach (RectShape surface in surfaces)
		{
			Vector2 resolutionVector = SeparatingAxisTheorem.CheckForCollisionResolution(
				player.BodyRectShape, surface
			);

			if (resolutionVector != Vector2.zero)
			{
				player.Move(resolutionVector);

				if (resolutionVector.x != 0)
				{
					if (resolutionVector.x > 0)
					{
						player.CollisionInfo.Left = true;
					}
					else if (resolutionVector.x < 0)
					{
						player.CollisionInfo.Right = true;
					}

					if (player.Velocity.y > 0)
					{
						player.SetVelocity(0, 0.5f * player.Velocity.y);
					}
					else
					{
						player.SetVelocity(0, player.Velocity.y);
					}
				}

				if (resolutionVector.y != 0)
				{
					if (resolutionVector.y > 0)
					{
						player.CollisionInfo.Bottom = true;
					}
					else if (resolutionVector.y < 0)
					{
						player.CollisionInfo.Top = true;
					}

					player.SetVelocity(player.Velocity.x, 0);
				}
			}
		}
	}

	private void GroundTriggerCheck()
	{
		foreach (RectShape rectShape in surfaces)
		{
			if (SeparatingAxisTheorem.CheckForCollision(player.GroundRectShape, rectShape))
			{
				player.TriggerInfo.Grounded = true;
				return;
			}
		}
	}

	private void ClimbTriggersCheck()
	{
		foreach (RectShape rectShape in climbables)
		{
			if (SeparatingAxisTheorem.CheckForCollision(player.BodyRectShape, rectShape))
			{
				player.TriggerInfo.Climbable = true;
				break;
			}
		}

	}

	private void WallTriggersCheck()
	{
		foreach (RectShape rectShape in surfaces)
		{
			if (SeparatingAxisTheorem.CheckForCollision(player.WallTopRectShape, rectShape))
			{
				player.TriggerInfo.Top = rectShape;
			}

			if (SeparatingAxisTheorem.CheckForCollision(player.WallMidRectShape, rectShape))
			{
				player.TriggerInfo.Mid = rectShape;
			}

			if (SeparatingAxisTheorem.CheckForCollision(player.WallLowRectShape, rectShape))
			{
				player.TriggerInfo.Low = rectShape;
			}
		}
	}
}
