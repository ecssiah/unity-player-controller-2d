using UnityEngine;

namespace C0
{
    public class DuckState : PlayerState
    {
		public override void Init()
		{
			player.SetAnimation("Duck");
			player.SetGravityScale(settings.DefaultGravityScale);
		}

		public override void UpdateManaged()
		{
			UpdateTriggers();

			if (!TriggerInfo.Ground)
			{
				player.SetState(PlayerStateType.Move);
			}
		}

		public override void FixedUpdateManaged()
		{
			Vector2 newVelocity = Vector2.zero;

			newVelocity.x = Mathf.SmoothDamp(
				player.Velocity.x,
				0,
				ref VelocityXDamped,
				settings.GroundSpeedSmoothTime
			);

			player.SetVelocity(newVelocity);
		}

		public override void SetVerticalInput(float inputValue)
		{
			base.SetVerticalInput(inputValue);

			if (InputInfo.Direction.y == 0)
			{
				player.SetState(PlayerStateType.Move);
			}
		}
	}
}