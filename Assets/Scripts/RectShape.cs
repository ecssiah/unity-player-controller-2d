using System.Collections.Generic;
using UnityEngine;

namespace C0
{
	public class RectShape : MonoBehaviour
	{
		public bool Static;

		public Vector2 Center => Static ? staticCenter : GetCenter();
		public Vector2 Size => Static ? staticSize : GetSize();
		public Vector2 Extents => Static ? staticExtents : GetExtents();

		public List<Vector2> Vertices => Static ? staticVertices : GetVertices();
		public List<Vector2> Normals => Static ? staticNormals : GetNormals();

		public Vector2 BottomLeft => Static ? staticVertices[0] : new Vector2(Center.x - Extents.x, Center.y - Extents.y);
		public Vector2 TopLeft => Static ? staticVertices[1] : new Vector2(Center.x - Extents.x, Center.y + Extents.y);
		public Vector2 TopRight => Static ? staticVertices[2] : new Vector2(Center.x + Extents.x, Center.y + Extents.y);
		public Vector2 BottomRight => Static ? staticVertices[3] : new Vector2(Center.x + Extents.x, Center.y - Extents.y);

		public Vector2 Min => BottomLeft;
		public Vector2 Max => TopRight;

		private BoxCollider2D boxCollider2D;

		private Vector2 staticCenter;
		private Vector2 staticSize;
		private Vector2 staticExtents;
		private List<Vector2> staticVertices;
		private List<Vector2> staticNormals;

		void Awake()
		{
			Static = false;

			boxCollider2D = GetComponent<BoxCollider2D>();

			staticCenter = GetCenter();
			staticSize = GetSize();
			staticExtents = GetExtents();
			staticVertices = GetVertices();
			staticNormals = GetNormals();
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
				new Vector2(Center.x + Extents.x, Center.y - Extents.y)
			};
		}

		private List<Vector2> GetNormals()
		{
			List<Vector2> vertices = GetVertices();
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
				normals.Add(normal);
			}

			return normals;
		}
	}
}