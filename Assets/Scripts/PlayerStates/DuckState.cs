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
			player.SetAnimation("Duck");
			player.SetGravityScale(settings.DefaultGravityScale);
		}
		
		public override void Update()
		{
			player.UpdateTriggers();

			if (!player.TriggerInfo.Ground)
			{
				player.SetState(PlayerStateType.Move);
			}
			else
			{
				Vector2 newVelocity = Vector2.zero;

				newVelocity.x = Mathf.SmoothDamp(
					player.Velocity.x,
					0,
					ref player.VelocityXDamped,
					settings.GroundSpeedSmoothTime
				);

				player.SetVelocity(newVelocity);
			}
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