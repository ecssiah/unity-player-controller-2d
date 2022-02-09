using UnityEngine;

public class Climbable : MonoBehaviour
{
    private BoxCollider2D bodyBoxCollider2D;

    public BoxShape BodyBox;

    void Awake()
    {
        bodyBoxCollider2D = GetComponent<BoxCollider2D>();

        BodyBox = new BoxShape(bodyBoxCollider2D);
    }
}
