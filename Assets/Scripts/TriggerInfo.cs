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

		public Bounds GroundBounds;
		public Bounds ClimbBounds;
		public Bounds WallTopBounds;
		public Bounds WallMidBounds;
		public Bounds WallLowBounds;

		public bool Ledge => !WallTop && WallMid;
		public bool Wall => WallTop && WallMid && WallLow;

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
