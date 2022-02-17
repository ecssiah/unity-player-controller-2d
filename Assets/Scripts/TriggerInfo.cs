using UnityEngine;

namespace C0
{
	[System.Serializable]
	public struct TriggerInfo
	{
		public RectShape Top;
		public RectShape Mid;
		public RectShape Low;

		public RectShape Grounded;
		public RectShape Climbable;
		public bool Ledge => !Top && Mid;
		public bool Wall => Top && Mid && Low;

		public void Reset()
		{
			Top = null;
			Mid = null;
			Low = null;

			Grounded = null;
			Climbable = null;
		}
	}
}
