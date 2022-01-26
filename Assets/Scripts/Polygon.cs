using System.Collections.Generic;
using UnityEngine;

public struct Polygon
{
	public Vector2 Center { get; private set; }
	public Vector2 Size { get; }

	private readonly List<Vector2> vertices;
	public List<Vector2> Vertices { get => vertices; }
	public List<Vector2> Edges { get; private set; }
	public List<Vector2> Normals { get; private set; }

	public Polygon(Vector2 center, Vector2 size)
	{
		Center = center;
		Size = size;

		float minX = center.x - size.x / 2;
		float minY = center.y - size.y / 2;
		float maxX = center.x + size.x / 2;
		float maxY = center.y + size.y / 2;

		vertices = new List<Vector2>
		{
			new Vector2(minX, minY),
			new Vector2(minX, maxY),
			new Vector2(maxX, maxY),
			new Vector2(maxX, minY),
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

	public Polygon(BoxCollider2D boxCollider2D)
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

	public void Move(Vector2 displacement)
	{
		Center += displacement;

		for (int i = 0; i < vertices.Count; i++)
		{
			vertices[i] += displacement;
		}
	}
}
