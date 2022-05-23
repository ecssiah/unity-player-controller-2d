using UnityEngine;

namespace C0
{
	public class WallSlideState : PlayerState
	{
		private float wallSlideTimer;

		public override void Init()
		{
			player.SetAnimation("Slide");
			player.SetVelocity(Vector2.zero);
			player.SetGravityScale(settings.WallSlideGravityScale);
		}

		public override void UpdateManaged()
		{
			UpdateTriggers();

			if (!TriggerInfo.WallSlide)
			{
				player.SetState(PlayerStateType.Move);
			}
			else if (InputInfo.Direction.x == player.Facing)
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
				if (InputInfo.Direction.x == -player.Facing)
				{
					player.SetState(PlayerStateType.Move);
					player.SetVelocity(Vector2.Scale(player.transform.localScale, settings.WallJumpVelocity));
				}
			}
		}
	}
}