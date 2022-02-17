using UnityEngine;

namespace C0
{
	[System.Serializable]
	public struct TriggerInfo
	{
		public RectShape WallTop;
		public RectShape WallMid;
		public RectShape WallLow;

		public RectShape Grounded;
		public RectShape Climbable;
		public bool Ledge => !WallTop && WallMid;
		public bool Wall => WallTop && WallMid && WallLow;

		public void Reset()
		{
			WallTop = null;
			WallMid = null;
			WallLow = null;

			Grounded = null;
			Climbable = null;
		}
	}
}
