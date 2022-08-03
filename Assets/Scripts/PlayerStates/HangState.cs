using UnityEngine;

namespace C0
{
	public class HangState : PlayerState
	{
		public HangState(GameSettings settings, Player player) : base(settings, player) { }

		public override void Init()
		{
			Player.CanClimbLedgeAt = Time.time + Settings.ClimbLedgeWaitTime;

			Vector2 ledgePosition = new Vector2(
				Mathf.Round(TriggerInfo.WallMidBounds.center.x),
				Mathf.Round(TriggerInfo.WallMidBounds.center.y)
			);

			Player.SetAnimation("Hang");
			Player.SetPosition(ledgePosition + Vector2.Scale(Player.transform.localScale, Settings.HangOffset));
			Player.SetVelocity(Vector2.zero);
			Player.SetGravityScale(0);
		}

		public override void UpdateManaged()
		{
			if (Player.CanClimbLedge && InputInfo.Move.y > 0)
			{
				Player.SetState(PlayerStateType.ClimbLedge);
			}
		}

		public override void SetVerticalInput(float inputValue)
		{
			base.SetVerticalInput(inputValue);

			if (InputInfo.Move.y < 0)
			{
				Player.SetState(PlayerStateType.Move);
			}
		}
	}
}