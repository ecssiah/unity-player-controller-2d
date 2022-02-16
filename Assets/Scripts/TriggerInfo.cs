using UnityEngine;

[System.Serializable]
public struct TriggerInfo
{
	public RectShape Top;
	public RectShape Mid;
	public RectShape Low;

	public bool Grounded;
	public bool Climbable;
	public bool Ledge => !Top && Mid;
	public bool Wall => Top && Mid && Low;

	public void Reset()
	{
		Top = null;
		Mid = null;
		Low = null;

		Grounded = false;
		Climbable = false;
	}
}
