using System.Collections.Generic;
using UnityEngine;

namespace C0
{
	public class RectShape : MonoBehaviour
	{
		public bool Static = false;

		public Vector2 Center => Static ? center : GetCenter();
		public Vector2 Size => Static ? size : GetSize();
		public Vector2 Extents => Static ? extents : GetExtents();
		public List<Vector2> Vertices => Static ? vertices : GetVertices();
		public List<Vector2> Normals => Static ? normals : GetNormals();

		public Vector2 BottomLeft => Static ? vertices[0] : GetVertices()[0];
		public Vector2 TopLeft => Static ? vertices[1] : GetVertices()[1];
		public Vector2 TopRight => Static ? vertices[2] : GetVertices()[2];
		public Vector2 BottomRight => Static ? vertices[3] : GetVertices()[3];

		public Vector2 Min => BottomLeft;
		public Vector2 Max => TopRight;
		
		private BoxCollider2D boxCollider2D;

		private Vector2 center;
		private Vector2 size;
		private Vector2 extents;
		private List<Vector2> vertices;
		private List<Vector2> normals;

		void Awake()
		{
			boxCollider2D = GetComponent<BoxCollider2D>();

			center = GetCenter();
			size = GetSize();
			extents = GetExtents();
			vertices = GetVertices();
			normals = GetNormals();
		}

		private Vector2 GetCenter()
		{
			return boxCollider2D.transform.TransformPoint(boxCollider2D.offset);
		}

		private Vector2 GetSize()
		{
			return boxCollider2D.bounds.size;
		}

		private Vector2 GetExtents()
		{
			return boxCollider2D.bounds.extents;
		}

		private List<Vector2> GetVertices()
		{
			return new List<Vector2> 
			{
				new Vector2(Center.x - Extents.x, Center.y - Extents.y),
				new Vector2(Center.x - Extents.x, Center.y + Extents.y),
				new Vector2(Center.x + Extents.x, Center.y + Extents.y),
				new Vector2(Center.x + Extents.x, Center.y - Extents.y),
			};
		}

		private List<Vector2> GetNormals()
		{
			List<Vector2> vertices = Vertices;
			List<Vector2> edges = new List<Vector2>();

			for (int i = 0; i < vertices.Count; i++)
			{
				Vector2 edge = vertices[(i + 1) % vertices.Count] - vertices[i];
				edges.Add(edge);
			}

			List<Vector2> normals = new List<Vector2>();

			foreach (Vector2 edge in edges)
			{
				Vector2 normal = new Vector2(-edge.y, edge.x);
				Normals.Add(normal);
			}

			return normals;
		}
	}
}
