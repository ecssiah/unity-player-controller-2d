using UnityEngine;

namespace C0
{
	public class HangState : PlayerState
	{
		private float nextClimbLedgeTime;

		public HangState(Player player, GameSettings settings) : base(player, settings) 
		{ 
			Type = PlayerStateType.Hang;
		}

		public override void Init()
		{
			nextClimbLedgeTime = Time.time + settings.HangTimeBeforeClimb;

			Vector2 ledgePosition = new Vector2(
				Mathf.Round(player.TriggerInfo.WallMidBounds.center.x),
				Mathf.Round(player.TriggerInfo.WallMidBounds.center.y)
			);

			player.SetAnimation("Hang");
			player.SetPosition(ledgePosition + player.transform.localScale * settings.HangOffset);
			player.SetVelocity(Vector2.zero);
			player.SetGravityScale(0);
		}

		public override void Update()
		{
			if (player.InputInfo.Direction.y > 0 && Time.time >= nextClimbLedgeTime)
			{
				player.SetState(PlayerStateType.ClimbLedge);
			}
		}

		public override void SetVerticalInput(float inputValue)
		{
			base.SetVerticalInput(inputValue);

			if (player.InputInfo.Direction.y < 0)
			{
				player.SetState(PlayerStateType.Move);
			}
		}
	}
}