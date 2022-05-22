using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace C0
{
    public class DuckState : PlayerState
    {
        public DuckState(Player player, GameSettings settings) : base(player, settings) 
		{
			Type = PlayerStateType.Duck;
		}

		public override void Init()
		{
			player.CurrentState = this;

			player.SetAnimation("Duck");

			player.RigidBody2D.gravityScale = settings.DefaultGravityScale;
		}

		public override void Update()
		{
			player.UpdateTriggers();
			
			if (!player.TriggerInfo.Ground)
			{
				player.SetState(PlayerStateType.Move);
			}
		}
		
		public override void FixedUpdate()
		{
			Vector2 newVelocity = Vector2.zero;

			newVelocity.x = Mathf.SmoothDamp(
				player.RigidBody2D.velocity.x,
				0,
				ref player.CurrentDampedVelocity,
				settings.GroundSpeedSmoothTime
			);

			if (Mathf.Abs(newVelocity.x) < settings.MinMoveSpeed)
			{
				newVelocity.x = 0;
				player.CurrentDampedVelocity = 0;
			}

			player.RigidBody2D.velocity = newVelocity;
		}

		public override void SetVerticalInput(float inputValue)
		{
			base.SetVerticalInput(inputValue);

			if (player.InputInfo.Direction.y == 0)
			{
				player.SetState(PlayerStateType.Move);
			}
		}
	}
}