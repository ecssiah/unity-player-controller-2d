using UnityEngine;

namespace C0
{
	public class HangState : PlayerState
	{
		private float nextClimbLedgeTime;

		public HangState(GameSettings settings, Player player) : base(settings, player) { }

		public override void Init()
		{
			nextClimbLedgeTime = Time.time + Settings.HangBeforeClimbTime;

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
			if (InputInfo.Direction.y > 0 && Time.time >= nextClimbLedgeTime)
			{
				Player.SetState(PlayerStateType.ClimbLedge);
			}
		}

		public override void SetVerticalInput(float inputValue)
		{
			base.SetVerticalInput(inputValue);

			if (InputInfo.Direction.y < 0)
			{
				Player.SetState(PlayerStateType.Move);
			}
		}
	}
}