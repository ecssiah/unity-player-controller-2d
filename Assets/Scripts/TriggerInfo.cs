using UnityEngine;

namespace C0
{
	[System.Serializable]
	public struct TriggerInfo
	{
		public Collider2D Ground;
		public Collider2D Climb;
		public Collider2D WallTop;
		public Collider2D WallMid;
		public Collider2D WallLow;

		public Bounds GroundBounds { get; set; }
		public Bounds ClimbBounds { get; set; }
		public Bounds WallTopBounds { get; set; }
		public Bounds WallMidBounds { get; set; }
		public Bounds WallLowBounds { get; set; }

		public bool Ledge => !WallTop && WallMid;
		public bool Wall => WallTop && WallMid && WallLow;
		public bool CanWallSlide => !Ground && Wall;

		public void Reset()
		{
			Ground = null;
			Climb = null;
			WallTop = null;
			WallMid = null;
			WallLow = null;
		}
	}
}
