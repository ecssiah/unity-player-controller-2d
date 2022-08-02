using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace C0
{
	public class DashState : PlayerState
	{
		private readonly TrailRenderer trailRenderer;

		public DashState(GameSettings settings, Player player) : base(settings, player) 
		{
			trailRenderer = Player.GetComponent<TrailRenderer>();
		}

		public override void Init()
		{
			trailRenderer.emitting = true;

			Player.SetAnimation("Dash");
			Player.SetGravityScale(0);
			Player.SetVelocity(Player.Facing * Settings.DashSpeed + Player.Velocity.x, 0);
		}

		public override void UpdateManaged()
		{
			UpdateTriggers();

			if (Mathf.Abs(Player.Velocity.x) < 0.4 * Settings.DashSpeed)
			{
				trailRenderer.emitting = false;

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
