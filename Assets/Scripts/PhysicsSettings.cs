using UnityEngine;

[CreateAssetMenu(fileName = "PhysicsSettings", menuName = "Physics Settings")]
public class PhysicsSettings : ScriptableObject
{
	public Vector2 Gravity = new Vector2(0, -9.8f);

	public float TerminalVelocity = -53f;

	public Vector2 HangPositionOffset = new Vector2(0, -1.3f);
	public Vector2 ClimbLedgePosition = new Vector2(0.55f, 1.20f);
}
