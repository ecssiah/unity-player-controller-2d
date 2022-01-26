using UnityEngine;

public class Surface : MonoBehaviour
{
    public bool DebugDraw;

    private BoxCollider2D boxCollider2D;
    public Polygon BodyPolygon;

    public Polygon LeftLedgePolygon;
    public Polygon RightLedgePolygon;

	void Awake()
	{
        DebugDraw = true;

        boxCollider2D = GetComponent<BoxCollider2D>();
        BodyPolygon = new Polygon(boxCollider2D);

        Vector2 leftLedgeLocation = new Vector2(
            BodyPolygon.Center.x - BodyPolygon.Size.x / 2 - 0.07f,
            BodyPolygon.Center.y + BodyPolygon.Size.y / 2 - 0.07f
        );

        Vector2 rightLedgeLocation = new Vector2(
            BodyPolygon.Center.x + BodyPolygon.Size.x / 2 + 0.07f,
            BodyPolygon.Center.y + BodyPolygon.Size.y / 2 - 0.07f
        );

        LeftLedgePolygon = new Polygon(leftLedgeLocation, new Vector2(0.14f, 0.14f));
        RightLedgePolygon = new Polygon(rightLedgeLocation, new Vector2(0.14f, 0.14f));
    }
    
    public void Move(Vector3 displacement)
    {
        transform.position += displacement;

        BodyPolygon.Move(displacement);
        LeftLedgePolygon.Move(displacement);
        RightLedgePolygon.Move(displacement);
    }

	void OnDrawGizmos()
	{
        if (DebugDraw)
        {
            Gizmos.color = new Color(1, 0.8f, 1, 0.1f);

            Gizmos.DrawCube(BodyPolygon.Center, BodyPolygon.Size);
            Gizmos.DrawCube(LeftLedgePolygon.Center, LeftLedgePolygon.Size);
            Gizmos.DrawCube(RightLedgePolygon.Center, RightLedgePolygon.Size);
        }
    }
}
