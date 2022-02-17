using UnityEngine;

namespace C0
{
	[System.Serializable]
	public struct CollisionInfo
	{
		public bool Top;
		public bool Bottom;
		public bool Left;
		public bool Right;

		public void Reset()
		{
			Top = false;
			Bottom = false;
			Left = false;
			Right = false;
		}
	}
}