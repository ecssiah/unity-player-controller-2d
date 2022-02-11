using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RectShape : MonoBehaviour
{
	public Vector2 Center { get; private set; }
	public Vector2 Size { get; private set; }

	public Vector2 BottomLeft => Vertices[0];
	public Vector2 TopLeft => Vertices[1];
	public Vector2 TopRight => Vertices[2];
	public Vector2 BottomRight => Vertices[3];

	public Vector2 Min => BottomLeft;
	public Vector2 Max => TopRight;

	public List<Vector2> Vertices;
	public List<Vector2> Edges;
	public List<Vector2> Normals;

	private RectTransform rectTransform;

	void Awake()
	{
		rectTransform = GetComponent<RectTransform>();

		Center = rectTransform.position;
		Size = rectTransform.rect.size;

		Vertices = new List<Vector2>();
		Edges = new List<Vector2>();
		Normals = new List<Vector2>();

		CalculateVertices();
		CalculateEdges();
		CalculateNormals();
	}

	private void CalculateVertices()
	{
		Vertices.Clear();

		Vector3[] vertices = new Vector3[4];
		rectTransform.GetWorldCorners(vertices);

		for (int i = 0; i < vertices.Length; i++)
		{
			Vertices.Add(vertices[i]);
		}
	}

	private void CalculateEdges()
	{
		for (int i = 0; i < Vertices.Count; i++)
		{
			Vector2 edge = Vertices[(i + 1) % Vertices.Count] - Vertices[i];
			Edges.Add(edge);
		}
	}

	private void CalculateNormals()
	{
		foreach (Vector2 edge in Edges)
		{
			Vector2 normal = new Vector2(-edge.y, edge.x);
			Normals.Add(normal);
		}
	}

	public void ResetPosition()
	{
		Center = rectTransform.position;
		Size = rectTransform.rect.size;

		CalculateVertices();
	}
}
