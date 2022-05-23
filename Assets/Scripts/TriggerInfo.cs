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

		[HideInInspector]
		public Bounds GroundBounds;
		[HideInInspector]
		public Bounds ClimbBounds;
		[HideInInspector]
		public Bounds WallTopBounds;
		[HideInInspector]
		public Bounds WallMidBounds;
		[HideInInspector]
		public Bounds WallLowBounds;

		[HideInInspector]
		public Vector3 TopOffset;
		[HideInInspector]
		public Vector3 MidOffset;
		[HideInInspector]
		public Vector3 LowOffset;

		public bool Ledge => !WallTop && WallMid;
		public bool WallSlide => !Ground && WallTop && WallMid && WallLow;

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
