using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace C0
{
	public class PhysicsSystem : MonoBehaviour
	{
		private int waitFrames;

		private GameSettings gameSettings;

		private Player player;

		private float playerSmoothDampVelocityX;

		private List<RectShape> surfaces;
		private List<RectShape> climbables;

		public void AwakeSystem()
		{
			gameSettings = Resources.Load<GameSettings>("Settings/GameSettings");

			player = GameObject.Find("Player").GetComponent<Player>();

			surfaces = GameObject.Find("Surfaces").GetComponentsInChildren<RectShape>().ToList();
			climbables = GameObject.Find("Climbables").GetComponentsInChildren<RectShape>().ToList();

			waitFrames = 8;
		}

		public void UpdateSystem()
		{
			if (waitFrames > 0)
			{
				waitFrames--;
			}
			else if (player.Hanging)
			{
				player.ClimbLedgeCheck();
			}
			else if (player.ClimbingLedge)
			{
				player.ClimbLedgeUpdate();
			}
			else
			{
				UpdateTriggers();

				player.UpdateState();

				ApplyForces();

				player.Move(Time.deltaTime * player.Velocity);

				ResolveCollisions();
			}
		}

		private void ApplyForces()
		{
			Vector2 newVelocity = player.Velocity;

			if (player.Velocity.x == 0)
			{
				playerSmoothDampVelocityX = 0;
			}

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
			newVelocity = Vector2.Scale(player.InputInfo.Direction, gameSettings.ClimbSpeed);
		}

		private void ApplyWallSlideForces(ref Vector2 newVelocity)
		{
			newVelocity.x = 0;

			newVelocity.y += gameSettings.WallSlideDamping * Time.deltaTime * gameSettings.Gravity;
			newVelocity.y = Mathf.Max(newVelocity.y, -gameSettings.MaxWallSlideSpeed);
		}

		private void ApplyGeneralForces(ref Vector2 newVelocity)
		{
			if (player.Position.y < -30)
			{
				waitFrames = 100;

				player.SetPosition(0, 3);
				newVelocity = Vector2.zero;

				return;
			}

			newVelocity.x = Mathf.SmoothDamp(
				newVelocity.x,
				player.InputInfo.Direction.x * gameSettings.RunSpeed,
				ref playerSmoothDampVelocityX,
				player.TriggerInfo.Grounded ? gameSettings.GroundSpeedSmoothTime : gameSettings.AirSpeedSmoothTime
			);

			if (Mathf.Abs(newVelocity.x) < gameSettings.MinSpeed)
			{
				newVelocity.x = 0;
			}

			newVelocity.y += Time.deltaTime * gameSettings.Gravity;
			newVelocity.y = Mathf.Max(newVelocity.y, gameSettings.MaxFallSpeed);
		}

		private void ResolveCollisions()
		{
			foreach (RectShape rectShape in surfaces)
			{
				Vector2 resolutionVector = SeparatingAxisTheorem.CheckForCollisionResolution(
					player.BodyRectShape, rectShape
				);

				if (resolutionVector != Vector2.zero)
				{
					player.Move(resolutionVector);

					if (resolutionVector.x != 0)
					{
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
						if (player.Velocity.y > 0)
						{
							player.SetVelocity(0.5f * player.Velocity.x, 0);
						}
						else
						{
							player.SetVelocity(player.Velocity.x, 0);
						}
					}
				}
			}
		}

		private void UpdateTriggers()
		{
			player.TriggerInfo.Reset();

			GroundTriggerCheck();
			ClimbTriggersCheck();
			WallTriggersCheck();
		}

		private void GroundTriggerCheck()
		{
			foreach (RectShape rectShape in surfaces)
			{
				if (SeparatingAxisTheorem.CheckForCollision(player.GroundRectShape, rectShape))
				{
					player.TriggerInfo.Grounded = rectShape;
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
					player.TriggerInfo.Climbable = rectShape;
					return;
				}
			}
		}

		private void WallTriggersCheck()
		{
			foreach (RectShape rectShape in surfaces)
			{
				if (SeparatingAxisTheorem.CheckForCollision(player.WallTopRectShape, rectShape))
				{
					player.TriggerInfo.WallTop = rectShape;
				}

				if (SeparatingAxisTheorem.CheckForCollision(player.WallMidRectShape, rectShape))
				{
					player.TriggerInfo.WallMid = rectShape;
				}

				if (SeparatingAxisTheorem.CheckForCollision(player.WallLowRectShape, rectShape))
				{
					player.TriggerInfo.WallLow = rectShape;
				}

				if (player.TriggerInfo.WallTop && player.TriggerInfo.WallMid && player.TriggerInfo.WallLow)
				{
					return;
				}
			}
		}
	}
}
