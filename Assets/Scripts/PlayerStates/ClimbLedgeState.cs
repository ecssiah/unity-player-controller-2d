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
			player.CurrentState = this;

			player.SetAnimation("ClimbLedge");

			player.StartClimbLedgeCoroutine();
		}

		public override void Update()
		{

		}
	}
}