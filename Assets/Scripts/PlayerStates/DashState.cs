using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace C0
{
	public class DashState : PlayerState
	{
		public DashState(GameSettings settings, Player player) : base(settings, player) { }

		public override void Init()
		{
			Player.SetAnimation("Dash");
			Player.SetVelocity(Player.Facing * Settings.DashSpeed + Player.Velocity.x, Player.Velocity.y);
		}

		public override void UpdateManaged()
		{
			UpdateTriggers();

			if (Mathf.Abs(Player.Velocity.x) < 0.2 * Settings.DashSpeed)
			{
				Player.SetState(PlayerStateType.Move);
			}
		}

		public override void FixedUpdateManaged()
		{
			Vector2 newVelocity = Player.Velocity;

			newVelocity.x = Mathf.SmoothDamp(
				Player.Velocity.x,
				0,
				ref VelocityXDamped,
				TriggerInfo.Ground ? Settings.GroundSpeedSmoothTime : Settings.AirSpeedSmoothTime
			);

			Player.SetVelocity(newVelocity);
		}
	}
}
