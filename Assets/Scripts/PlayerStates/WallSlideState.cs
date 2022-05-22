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
			player.SetAnimation("Slide");
			player.SetVelocity(Vector2.zero);
			player.SetGravityScale(settings.WallSlideGravityScale);
		}

		public override void Update()
		{
			player.UpdateTriggers();

			if (!player.TriggerInfo.CanWallSlide)
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
					player.SetVelocity(player.transform.localScale * settings.WallJumpVelocity);
				}
			}
		}
	}
}