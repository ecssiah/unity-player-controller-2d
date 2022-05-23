namespace C0
{
	public class ClimbLedgeState : PlayerState
	{
		public override void Init()
		{
			player.SetAnimation("ClimbLedge");

			player.RunClimbLedgeAction();
		}
	}
}