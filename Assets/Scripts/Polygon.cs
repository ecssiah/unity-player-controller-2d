using System.Collections.Generic;
using UnityEngine;

public struct Polygon
{
	public Vector2 Center { get; private set; }

	private List<Vector2> vertices;
	public List<Vector2> Vertices { get => vertices; }
	public List<Vector2> Edges { get; private set; }
	public List<Vector2> Normals { get; private set; }

	public Polygon(BoxCollider2D boxCollider2D)
	{
		Center = boxCollider2D.transform.position;

		vertices = new List<Vector2>
		{
			boxCollider2D.transform.TransformPoint(
				new Vector3(
					boxCollider2D.offset.x - boxCollider2D.size.x / 2,
					boxCollider2D.offset.y - boxCollider2D.size.y / 2,
					0
				)
			),
			boxCollider2D.transform.TransformPoint(
				new Vector3(
					boxCollider2D.offset.x - boxCollider2D.size.x / 2,
					boxCollider2D.offset.y + boxCollider2D.size.y / 2,
					0
				)
			),
			boxCollider2D.transform.TransformPoint(
				new Vector3(
					boxCollider2D.offset.x + boxCollider2D.size.x / 2,
					boxCollider2D.offset.y + boxCollider2D.size.y / 2,
					0
				)
			),
			boxCollider2D.transform.TransformPoint(
				new Vector3(
					boxCollider2D.offset.x + boxCollider2D.size.x / 2,
					boxCollider2D.offset.y - boxCollider2D.size.y / 2,
					0
				)
			),
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
			Normals.Add(new Vector2(-edge.y, edge.x));
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
