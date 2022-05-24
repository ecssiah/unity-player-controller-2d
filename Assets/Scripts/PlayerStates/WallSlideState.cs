using UnityEngine;

namespace C0
{
	public class WallSlideState : PlayerState
	{
		private float wallSlideTimer;

		public WallSlideState(GameSettings settings, Player player) : base(settings, player) { }

		public override void Init()
		{
			Player.SetAnimation("Slide");
			Player.SetVelocity(Vector2.zero);
			Player.SetGravityScale(Settings.WallSlideGravityScale);
		}

		public override void UpdateManaged()
		{
			UpdateTriggers();

			if (!TriggerInfo.WallSlide)
			{
				Player.SetState(PlayerStateType.Move);
			}
			else if (InputInfo.Direction.x == Player.Facing)
			{
				wallSlideTimer = Settings.WallSlideHoldTime;
			}
			else
			{
				wallSlideTimer -= Time.deltaTime;

				if (wallSlideTimer <= 0)
				{
					Player.SetState(PlayerStateType.Move);
				}
			}
		}

		public override void SetJumpInput(float inputValue)
		{
			base.SetJumpInput(inputValue);

			if (inputValue == 1)
			{
				if (InputInfo.Direction.x == -Player.Facing)
				{
					Player.SetState(PlayerStateType.Move);
					Player.SetVelocity(Vector2.Scale(Player.transform.localScale, Settings.WallJumpSpeed));
				}
			}
		}
	}
}