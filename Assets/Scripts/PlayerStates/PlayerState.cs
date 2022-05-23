using UnityEngine;

namespace C0
{
	public abstract class PlayerState : MonoBehaviour
	{
		protected GameSettings settings;
		
		protected Player player;

		protected float VelocityXDamped;

		public static InputInfo InputInfo;
		public static TriggerInfo TriggerInfo;

		private LayerMask surfaceLayerMask;
		private LayerMask climbableLayerMask;

		void Awake()
		{
			player = GameObject.Find("Player").GetComponent<Player>();
			settings = Resources.Load<GameSettings>("Settings/GameSettings");

			surfaceLayerMask = LayerMask.GetMask("Surface");
			climbableLayerMask = LayerMask.GetMask("Climbable");

			TriggerInfo.GroundBounds = new Bounds(Vector3.zero, new Vector2(player.Bounds.size.x - 0.02f, 0.05f));
			TriggerInfo.ClimbBounds = new Bounds(Vector3.zero, new Vector2(player.Bounds.size.x - 0.02f, 0.4f));
			TriggerInfo.WallTopBounds = new Bounds(Vector3.zero, settings.WallTriggerSize);
			TriggerInfo.WallMidBounds = new Bounds(Vector3.zero, settings.WallTriggerSize);
			TriggerInfo.WallLowBounds = new Bounds(Vector3.zero, settings.WallTriggerSize);

			float horizontalOffset = player.Bounds.extents.x + 0.05f;

			TriggerInfo.TopOffset = new Vector3(horizontalOffset, 1.1f * player.Bounds.size.y);
			TriggerInfo.MidOffset = new Vector3(horizontalOffset, 0.8f * player.Bounds.size.y);
			TriggerInfo.LowOffset = new Vector3(horizontalOffset, 0.1f * player.Bounds.size.y);
		}

		public virtual void Init() { }

		public virtual void UpdateManaged() { }

		public virtual void FixedUpdateManaged() { }

		public virtual void SetHorizontalInput(float inputValue) 
		{
			InputInfo.Direction.x = inputValue;
		}

		public virtual void SetVerticalInput(float inputValue) 
		{
			InputInfo.Direction.y = inputValue;
		}

		public virtual void SetJumpInput(float inputValue) 
		{ 
			InputInfo.Jump = inputValue;
		}

		public void UpdateTriggers()
		{
			TriggerInfo.Reset();

			Vector3 playerPosition = player.Position;

			UpdateGroundTrigger(playerPosition);
			UpdateClimbTrigger(playerPosition);
			UpdateWallTriggers(playerPosition);
		}

		private void UpdateGroundTrigger(Vector3 playerPosition)
		{
			TriggerInfo.GroundBounds.center = playerPosition + 0.025f * Vector3.down;

			TriggerInfo.Ground = Physics2D.OverlapBox
			(
				TriggerInfo.GroundBounds.center, TriggerInfo.GroundBounds.size, 0f, surfaceLayerMask
			);
		}

		private void UpdateClimbTrigger(Vector3 playerPosition)
		{
			TriggerInfo.ClimbBounds.center = playerPosition + 0.6f * Vector3.up;

			TriggerInfo.Climb = Physics2D.OverlapBox
			(
				TriggerInfo.ClimbBounds.center, TriggerInfo.ClimbBounds.size, 0f, climbableLayerMask
			);
		}

		private void UpdateWallTriggers(Vector3 playerPosition)
		{
			Vector3 localScale = new Vector3(player.Facing, 1, 1);

			TriggerInfo.WallTopBounds.center = playerPosition + Vector3.Scale(localScale, TriggerInfo.TopOffset);
			TriggerInfo.WallMidBounds.center = playerPosition + Vector3.Scale(localScale, TriggerInfo.MidOffset);
			TriggerInfo.WallLowBounds.center = playerPosition + Vector3.Scale(localScale, TriggerInfo.LowOffset);

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

		void OnDrawGizmos()
		{
			if (Application.isPlaying)
			{
				Gizmos.color = new Color(1, 0, 1, 0.4f);

				Gizmos.DrawWireCube(TriggerInfo.GroundBounds.center, TriggerInfo.GroundBounds.size);
				Gizmos.DrawWireCube(TriggerInfo.ClimbBounds.center, TriggerInfo.ClimbBounds.size);

				Gizmos.DrawWireCube(TriggerInfo.WallTopBounds.center, TriggerInfo.WallTopBounds.size);
				Gizmos.DrawWireCube(TriggerInfo.WallMidBounds.center, TriggerInfo.WallMidBounds.size);
				Gizmos.DrawWireCube(TriggerInfo.WallLowBounds.center, TriggerInfo.WallLowBounds.size);
			}
		}
	}
}