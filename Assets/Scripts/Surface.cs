using UnityEngine;

public class Surface : MonoBehaviour
{
    public RectShape BodyRect;

	void Awake()
	{
		BodyRect = GetComponent<RectShape>();
    }
}
