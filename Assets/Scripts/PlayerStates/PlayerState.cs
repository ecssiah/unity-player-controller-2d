using UnityEngine;

namespace C0
{
	public abstract class PlayerState
	{
		protected GameSettings Settings;
		protected Player Player;

		protected InputInfo InputInfo;
		protected TriggerInfo TriggerInfo;

		private LayerMask surfaceLayerMask;
		private LayerMask climbableLayerMask;

		protected float VelocityXDamped;

		public PlayerState(GameSettings settings, Player player)
		{
			Settings = settings;
			Player = player;

			InputInfo = player.GetComponent<InputInfo>();
			TriggerInfo = player.GetComponent<TriggerInfo>();

			surfaceLayerMask = LayerMask.GetMask("Surface");
			climbableLayerMask = LayerMask.GetMask("Climbable");
		}

		public virtual void Init() { }

		public virtual void UpdateManaged() { }

		public virtual void FixedUpdateManaged() { }

		public virtual void SetHorizontalInput(float inputValue)
		{
			InputInfo.Move.x = inputValue;
		}

		public virtual void SetVerticalInput(float inputValue)
		{
			InputInfo.Move.y = inputValue;
		}

		public virtual void SetJumpInput(float inputValue)
		{
			InputInfo.Jump = inputValue;
		}

		public void UpdateTriggers()
		{
			TriggerInfo.ResetTriggers();

			UpdateGroundTrigger();
			UpdateClimbTrigger();
			UpdateWallTriggers();
		}

		private void UpdateGroundTrigger()
		{
			TriggerInfo.Ground = Physics2D.OverlapBox
			(
				TriggerInfo.GroundBounds.center, TriggerInfo.GroundBounds.size, 0f, surfaceLayerMask
			);
		}

		private void UpdateClimbTrigger()
		{
			TriggerInfo.Climb = Physics2D.OverlapBox
			(
				TriggerInfo.ClimbBounds.center, TriggerInfo.ClimbBounds.size, 0f, climbableLayerMask
			);
		}

		private void UpdateWallTriggers()
		{
			TriggerInfo.WallTop = Physics2D.OverlapBox
			(
				TriggerInfo.WallTopBounds.center, TriggerInfo.WallTopBounds.size, 0f, surfaceLayerMask
			);

			TriggerInfo.WallMid = Physics2D.OverlapBox
			(
				TriggerInfo.WallMidBounds.center, TriggerInfo.WallMidBounds.size, 0f, surfaceLayerMask
			);

			TriggerInfo.WallLow = Physics2D.OverlapBox
			(
				TriggerInfo.WallLowBounds.center, TriggerInfo.WallLowBounds.size, 0f, surfaceLayerMask
			);
		}

		public void ResetVelocityXDamping()
		{
			VelocityXDamped = 0;
		}
	}
}