using System.Collections.Generic;
using UnityEngine;

public struct BoxShape
{
	private readonly BoxCollider2D boxCollider2D;

	public Vector2 Center { get; private set; }
	public Vector2 Size { get; private set; }

	public Vector2 BottomLeft => vertices[0];
	public Vector2 TopLeft => vertices[1];
	public Vector2 TopRight => vertices[2];
	public Vector2 BottomRight => vertices[3];

	private List<Vector2> vertices;
	public List<Vector2> Vertices { get => vertices; }
	public List<Vector2> Edges { get; private set; }
	public List<Vector2> Normals { get; private set; }

	public BoxShape(BoxCollider2D _boxCollider2D)
	{
		boxCollider2D = _boxCollider2D;

		Center = boxCollider2D.transform.TransformPoint(boxCollider2D.offset);
		Size = boxCollider2D.size * boxCollider2D.transform.localScale;

		float minX = boxCollider2D.offset.x - boxCollider2D.size.x / 2;
		float minY = boxCollider2D.offset.y - boxCollider2D.size.y / 2;
		float maxX = boxCollider2D.offset.x + boxCollider2D.size.x / 2;
		float maxY = boxCollider2D.offset.y + boxCollider2D.size.y / 2;

		vertices = new List<Vector2>
		{
			boxCollider2D.transform.TransformPoint(minX, minY, 0),
			boxCollider2D.transform.TransformPoint(minX, maxY, 0),
			boxCollider2D.transform.TransformPoint(maxX, maxY, 0),
			boxCollider2D.transform.TransformPoint(maxX, minY, 0),
		};

		Edges = new List<Vector2>();

		for (int i = 0; i < vertices.Count; i++)
		{
			Vector2 edge = vertices[(i + 1) % vertices.Count] - vertices[i];
			Edges.Add(edge);
		}

		Normals = new List<Vector2>();

		foreach (Vector2 edge in Edges)
		{
			Vector2 normal = new Vector2(-edge.y, edge.x);
			Normals.Add(normal);
		}
	}

	public void ResetPosition()
	{
		Center = boxCollider2D.transform.TransformPoint(boxCollider2D.offset);
		Size = boxCollider2D.size * boxCollider2D.transform.localScale;

		float minX = boxCollider2D.offset.x - boxCollider2D.size.x / 2;
		float minY = boxCollider2D.offset.y - boxCollider2D.size.y / 2;
		float maxX = boxCollider2D.offset.x + boxCollider2D.size.x / 2;
		float maxY = boxCollider2D.offset.y + boxCollider2D.size.y / 2;

		vertices = new List<Vector2>
		{
			boxCollider2D.transform.TransformPoint(minX, minY, 0),
			boxCollider2D.transform.TransformPoint(minX, maxY, 0),
			boxCollider2D.transform.TransformPoint(maxX, maxY, 0),
			boxCollider2D.transform.TransformPoint(maxX, minY, 0),
		};
	}
}
