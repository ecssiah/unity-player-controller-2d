using UnityEngine;

namespace C0
{
	[System.Serializable]
	public struct TriggerInfo
	{
		public BoxCollider2D Grounded;

		public BoxCollider2D Climbable;

		public BoxCollider2D WallTop;
		public BoxCollider2D WallMid;
		public BoxCollider2D WallLow;

		public bool Ledge => !WallTop && WallMid;
		public bool Wall => WallTop && WallMid && WallLow;

		public void Reset()
		{
			Grounded = null;
			Climbable = null;

			WallTop = null;
			WallMid = null;
			WallLow = null;
		}
	}
}
