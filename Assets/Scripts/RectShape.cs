using System.Collections.Generic;
using UnityEngine;

namespace C0
{
	public class RectShape : MonoBehaviour
	{
		public bool Static;

		public Vector2 Center => Static ? center : GetCenter();
		public Vector2 Size => boxCollider2D.bounds.size;
		public Vector2 Extents => boxCollider2D.bounds.extents;
		
		public List<Vector2> Vertices => Static ? vertices : GetVertices();
		public List<Vector2> Normals { get; private set; }

		public Vector2 BottomLeft => Static ? vertices[0] : new Vector2(Center.x - Extents.x, Center.y - Extents.y);
		public Vector2 TopLeft => Static ? vertices[1] : new Vector2(Center.x - Extents.x, Center.y + Extents.y);
		public Vector2 TopRight => Static ? vertices[2] : new Vector2(Center.x + Extents.x, Center.y + Extents.y);
		public Vector2 BottomRight => Static ? vertices[3] : new Vector2(Center.x + Extents.x, Center.y - Extents.y);

		public Vector2 Min => BottomLeft;
		public Vector2 Max => TopRight;

		private BoxCollider2D boxCollider2D;

		private Vector2 center;
		private List<Vector2> vertices;

		void Awake()
		{
			Static = false;

			boxCollider2D = GetComponent<BoxCollider2D>();

			center = GetCenter();
			vertices = GetVertices();

			CalculateNormals();
		}

		private Vector2 GetCenter()
		{
			return boxCollider2D.transform.TransformPoint(boxCollider2D.offset);
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
}