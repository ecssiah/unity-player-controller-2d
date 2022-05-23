namespace C0
{
	public class ClimbLedgeState : PlayerState
	{
		public ClimbLedgeState(GameSettings settings, Player player) : base(settings, player) { }

		public override void Init()
		{
			player.SetAnimation("ClimbLedge");

			player.RunClimbLedgeAction();
		}
	}
}