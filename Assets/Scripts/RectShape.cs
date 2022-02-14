using System.Collections.Generic;
using UnityEngine;

public class RectShape : MonoBehaviour
{
	public Vector2 Center => rectTransform.position;
	public Vector2 Size => rectTransform.rect.size;
	public Vector2 Extents => rectTransform.rect.size / 2;

	public Vector2 BottomLeft => new Vector2(Center.x - Extents.x, Center.y - Extents.y);
	public Vector2 TopLeft => new Vector2(Center.x - Extents.x, Center.y + Extents.y);
	public Vector2 TopRight => new Vector2(Center.x + Extents.x, Center.y + Extents.y);
	public Vector2 BottomRight => new Vector2(Center.x + Extents.x, Center.y - Extents.y);

	public Vector2 Min => BottomLeft;
	public Vector2 Max => TopRight;

	public List<Vector2> Vertices
	{
		get
		{
			return new List<Vector2>
			{
				new Vector2(Center.x - Extents.x, Center.y - Extents.y),
				new Vector2(Center.x - Extents.x, Center.y + Extents.y),
				new Vector2(Center.x + Extents.x, Center.y + Extents.y),
				new Vector2(Center.x + Extents.x, Center.y - Extents.y),
			};
		}
	}

	public List<Vector2> Normals { get; private set; }

	private RectTransform rectTransform;

	void Awake()
	{
		rectTransform = GetComponent<RectTransform>();

		CalculateNormals();
	}

	private void CalculateNormals()
	{
		List<Vector2> vertices = Vertices;
		List<Vector2> edges = new List<Vector2>();
		
		for (int i = 0; i < vertices.Count; i++)
		{
			Vector2 edge = vertices[(i + 1) % vertices.Count] - vertices[i];
			edges.Add(edge);
		}

		Normals = new List<Vector2>();

		foreach (Vector2 edge in edges)
		{
			Vector2 normal = new Vector2(-edge.y, edge.x);
			Normals.Add(normal);
		}
	}
}
