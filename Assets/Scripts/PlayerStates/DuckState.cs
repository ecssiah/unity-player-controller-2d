using UnityEngine;

namespace C0
{
	public class DuckState : PlayerState
	{
		public DuckState(GameSettings settings, Player player) : base(settings, player) { }

		public override void Init()
		{
			Player.SetAnimation("Duck");
			Player.SetGravityScale(Settings.DefaultGravityScale);
		}

		public override void UpdateManaged()
		{
			UpdateTriggers();

			if (!TriggerInfo.Ground)
			{
				Player.SetState(PlayerStateType.Move);
			}
		}

		public override void FixedUpdateManaged()
		{
			Vector2 newVelocity = Vector2.zero;

			newVelocity.x = Mathf.SmoothDamp(
				Player.Velocity.x,
				0,
				ref VelocityXDamped,
				Settings.GroundSpeedSmoothTime
			);

			Player.SetVelocity(newVelocity);
		}

		public override void SetVerticalInput(float inputValue)
		{
			base.SetVerticalInput(inputValue);

			if (InputInfo.Move.y == 0)
			{
				Player.SetState(PlayerStateType.Move);
			}
		}
	}
}