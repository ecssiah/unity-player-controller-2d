using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace C0
{
	public class WallSlideState : PlayerState
	{
		private float wallSlideTimer;

		public WallSlideState(Player player, GameSettings settings) : base(player, settings) 
		{ 
			Type = PlayerStateType.WallSlide;
		}

		public override void Init()
		{
			player.CurrentState = this;

			player.SetAnimation("Slide");

			player.RigidBody2D.velocity = Vector2.zero;
			player.RigidBody2D.gravityScale = settings.WallSlideGravityScale;
		}

		public override void Update()
		{
			player.UpdateTriggers();

			if (player.TriggerInfo.Ground || !player.TriggerInfo.Wall)
			{
				player.SetState(PlayerStateType.Move);
			}
			else if (player.InputInfo.Direction.x == player.Facing)
			{
				wallSlideTimer = settings.WallSlideHoldTime;
			}
			else
			{
				wallSlideTimer -= Time.deltaTime;

				if (wallSlideTimer <= 0)
				{
					player.SetState(PlayerStateType.Move);
				}
			}
		}

		public override void SetJumpInput(float inputValue)
		{
			base.SetJumpInput(inputValue);

			if (inputValue == 1)
			{
				if (player.InputInfo.Direction.x == -player.Facing)
				{
					player.SetState(PlayerStateType.Move);

					player.RigidBody2D.velocity = player.transform.localScale * settings.WallJumpVelocity;
				}
			}
		}
	}
}