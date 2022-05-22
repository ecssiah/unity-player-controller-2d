namespace C0
{
	public class ClimbLedgeState : PlayerState
	{
		public ClimbLedgeState(Player player, GameSettings settings) : base(player, settings) 
		{
			Type = PlayerStateType.ClimbLedge;
		}

		public override void Init()
		{
			player.SetAnimation("ClimbLedge");

			player.RunClimbLedgeAction();
		}

		public override void Update()
		{

		}
	}
}