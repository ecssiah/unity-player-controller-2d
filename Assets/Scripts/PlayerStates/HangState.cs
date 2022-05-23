using UnityEngine;

namespace C0
{
	public class HangState : PlayerState
	{
		private float nextClimbLedgeTime;

		public HangState(GameSettings settings, Player player) : base(settings, player) { }

		public override void Init()
		{
			nextClimbLedgeTime = Time.time + settings.HangTimeBeforeClimb;

			Vector2 ledgePosition = new Vector2(
				Mathf.Round(triggerInfo.WallMidBounds.center.x),
				Mathf.Round(triggerInfo.WallMidBounds.center.y)
			);

			player.SetAnimation("Hang");
			player.SetPosition(ledgePosition + Vector2.Scale(player.transform.localScale, settings.HangOffset));
			player.SetVelocity(Vector2.zero);
			player.SetGravityScale(0);
		}

		public override void UpdateManaged()
		{
			if (inputInfo.Direction.y > 0 && Time.time >= nextClimbLedgeTime)
			{
				player.SetState(PlayerStateType.ClimbLedge);
			}
		}

		public override void SetVerticalInput(float inputValue)
		{
			base.SetVerticalInput(inputValue);

			if (inputInfo.Direction.y < 0)
			{
				player.SetState(PlayerStateType.Move);
			}
		}
	}
}