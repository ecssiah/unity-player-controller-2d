using UnityEngine;

public class Climbable : MonoBehaviour
{
	public RectShape BodyRect;

	void Awake()
	{
		BodyRect = GetComponent<RectShape>();
	}
}
