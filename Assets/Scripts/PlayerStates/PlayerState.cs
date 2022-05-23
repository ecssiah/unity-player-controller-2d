using UnityEngine;

namespace C0
{
	public abstract class PlayerState
	{
		protected GameSettings settings;
		
		protected Player player;

		protected float VelocityXDamped;

		protected InputInfo inputInfo;
		protected TriggerInfo triggerInfo;

		private LayerMask surfaceLayerMask;
		private LayerMask climbableLayerMask;

		public PlayerState(GameSettings settings, Player player)
		{
			this.settings = settings;
			this.player = player;

			inputInfo = player.GetComponent<InputInfo>();
			triggerInfo = player.GetComponent<TriggerInfo>();

			surfaceLayerMask = LayerMask.GetMask("Surface");
			climbableLayerMask = LayerMask.GetMask("Climbable");
		}

		public virtual void Init() { }

		public virtual void UpdateManaged() { }

		public virtual void FixedUpdateManaged() { }

		public virtual void SetHorizontalInput(float inputValue) 
		{
			inputInfo.Direction.x = inputValue;
		}

		public virtual void SetVerticalInput(float inputValue) 
		{
			inputInfo.Direction.y = inputValue;
		}

		public virtual void SetJumpInput(float inputValue) 
		{ 
			inputInfo.Jump = inputValue;
		}

		public void UpdateTriggers()
		{
			triggerInfo.ResetTriggers();

			Vector3 playerPosition = player.Position;

			UpdateGroundTrigger(playerPosition);
			UpdateClimbTrigger(playerPosition);
			UpdateWallTriggers(playerPosition);
		}

		private void UpdateGroundTrigger(Vector3 playerPosition)
		{
			triggerInfo.Ground = Physics2D.OverlapBox
			(
				triggerInfo.GroundBounds.center, triggerInfo.GroundBounds.size, 0f, surfaceLayerMask
			);
		}

		private void UpdateClimbTrigger(Vector3 playerPosition)
		{
			triggerInfo.Climb = Physics2D.OverlapBox
			(
				triggerInfo.ClimbBounds.center, triggerInfo.ClimbBounds.size, 0f, climbableLayerMask
			);
		}

		private void UpdateWallTriggers(Vector3 playerPosition)
		{
			triggerInfo.WallTop = Physics2D.OverlapBox
			(
				triggerInfo.WallTopBounds.center, triggerInfo.WallTopBounds.size, 0f, surfaceLayerMask
			);

			triggerInfo.WallMid = Physics2D.OverlapBox
			(
				triggerInfo.WallMidBounds.center, triggerInfo.WallMidBounds.size, 0f, surfaceLayerMask
			);

			triggerInfo.WallLow = Physics2D.OverlapBox
			(
				triggerInfo.WallLowBounds.center, triggerInfo.WallLowBounds.size, 0f, surfaceLayerMask
			);
		}

		public void ResetVelocityXDamping()
		{
			VelocityXDamped = 0;
		}
	}
}