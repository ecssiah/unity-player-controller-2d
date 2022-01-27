using UnityEngine;

public class Surface : MonoBehaviour
{
    public bool DebugDraw;

    private BoxCollider2D bodyBoxCollider2D;

    public BoxShape BodyBox;

	void Awake()
	{
        DebugDraw = true;

        bodyBoxCollider2D = GetComponent<BoxCollider2D>();

		BodyBox = new BoxShape(bodyBoxCollider2D);
    }
}
