using UnityEngine;

namespace C0
{
	[System.Serializable]
	public struct TriggerInfo
	{
		public RectShape Grounded;
		
		public RectShape Climbable;
		
		public RectShape WallTop;
		public RectShape WallMid;
		public RectShape WallLow;

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
