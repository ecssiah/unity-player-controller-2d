using UnityEngine;

public class Surface : MonoBehaviour
{
    private BoxCollider2D boxCollider2D;
    public Polygon Polygon;

    public Polygon LeftLedgePolygon;
    public Polygon RightLedgePolygon;

	void Awake()
	{
        boxCollider2D = GetComponent<BoxCollider2D>();
        Polygon = new Polygon(boxCollider2D);


    }
    
    public void Move(Vector3 displacement)
    {
        transform.position += displacement;
        Polygon.Move(displacement);
    }

	void OnDrawGizmos()
	{
		
	}
}
