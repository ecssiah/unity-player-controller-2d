namespace C0
{
	public abstract class PlayerState
	{
		protected Player player;
		protected GameSettings settings;

		public PlayerStateType Type { get; protected set; }

		protected PlayerState(Player player, GameSettings settings)
		{
			this.player = player;
			this.settings = settings;
		}

		public abstract void Init();

		public abstract void Update();

		public virtual void FixedUpdate() { }

		public virtual void SetHorizontalInput(float inputValue) 
		{
			player.InputInfo.Direction.x = inputValue;
		}

		public virtual void SetVerticalInput(float inputValue) 
		{
			player.InputInfo.Direction.y = inputValue;
		}

		public virtual void SetJumpInput(float inputValue) 
		{ 
			player.InputInfo.Jump = inputValue;
		}
	}
}