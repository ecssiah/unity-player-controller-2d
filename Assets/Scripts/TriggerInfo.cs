using UnityEngine;

namespace C0
{
	public class TriggerInfo : MonoBehaviour
	{
		private GameSettings settings;
		private Player player;

		public Collider2D Ground;
		public Collider2D Climb;
		public Collider2D WallTop;
		public Collider2D WallMid;
		public Collider2D WallLow;

		public bool Ledge => !WallTop && WallMid;
		public bool WallSlide => !Ground && WallTop && WallMid && WallLow;

		public Vector3 GroundOffset { get; set; }
		public Vector3 ClimbOffset { get; set; }
		public Vector3 TopOffset { get; set; }
		public Vector3 MidOffset { get; set; }
		public Vector3 LowOffset { get; set; }

		private Bounds groundBounds;
		public Bounds GroundBounds
		{
			get
			{
				groundBounds.center = player.Position + GroundOffset;

				return groundBounds;
			}
		}

		private Bounds climbBounds;
		public Bounds ClimbBounds
		{
			get
			{
				climbBounds.center = player.Position + ClimbOffset;

				return climbBounds;
			}
		}

		private Bounds wallTopBounds;
		public Bounds WallTopBounds
		{
			get
			{
				wallTopBounds.center = player.Position + Vector3.Scale(player.Scale, TopOffset);

				return wallTopBounds;
			}
		}

		private Bounds wallMidBounds;
		public Bounds WallMidBounds
		{
			get
			{
				wallMidBounds.center = player.Position + Vector3.Scale(player.Scale, MidOffset);

				return wallMidBounds;
			}
		}

		private Bounds wallLowBounds;
		public Bounds WallLowBounds
		{
			get
			{
				wallLowBounds.center = player.Position + Vector3.Scale(player.Scale, LowOffset);

				return wallLowBounds;
			}
		}

		void Awake()
		{
			settings = Resources.Load<GameSettings>("Settings/GameSettings");
			player = GetComponentInParent<Player>();
		}

		void Start()
		{
			GroundOffset = 0.025f * Vector3.down;
			ClimbOffset = 0.6f * Vector3.up;
			TopOffset = new Vector3(player.Bounds.extents.x + 0.05f, 1.1f * player.Bounds.size.y);
			MidOffset = new Vector3(player.Bounds.extents.x + 0.05f, 0.8f * player.Bounds.size.y);
			LowOffset = new Vector3(player.Bounds.extents.x + 0.05f, 0.1f * player.Bounds.size.y);

			groundBounds = new Bounds(Vector3.zero, new Vector2(player.Bounds.size.x - 0.02f, 0.05f));
			climbBounds = new Bounds(Vector3.zero, new Vector2(player.Bounds.size.x - 0.02f, 0.4f));
			wallTopBounds = new Bounds(Vector3.zero, settings.WallTriggerSize);
			wallMidBounds = new Bounds(Vector3.zero, settings.WallTriggerSize);
			wallLowBounds = new Bounds(Vector3.zero, settings.WallTriggerSize);
		}

		public void ResetTriggers()
		{
			Ground = null;
			Climb = null;
			WallTop = null;
			WallMid = null;
			WallLow = null;
		}
	}
}
