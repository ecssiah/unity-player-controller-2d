using UnityEngine;
using UnityEngine.Tilemaps;

namespace C0
{
	public class MapSystem : MonoBehaviour
	{
		private GameSettings settings;

		private Tilemap overlayTilemap;
		private Tile boundaryTile;

		public void AwakeManaged()
		{
			settings = Resources.Load<GameSettings>("Settings/GameSettings");

			overlayTilemap = GameObject.Find("Overlay").GetComponent<Tilemap>();
			boundaryTile = Resources.Load<Tile>("Tiles/pink_diamond");
		}

		public void StartManaged()
		{
			DrawLevelBoundary();
		}

		private void DrawLevelBoundary()
		{
			for (float x = settings.LevelBounds.min.x; x <= settings.LevelBounds.max.x; x++)
			{
				for (float y = settings.LevelBounds.min.y; y <= settings.LevelBounds.max.y; y++)
				{
					bool onEdge = (
						x == settings.LevelBounds.min.x || x == settings.LevelBounds.max.x ||
						y == settings.LevelBounds.min.y || y == settings.LevelBounds.max.y
					);

					if (onEdge)
					{
						overlayTilemap.SetTile(new Vector3Int((int)x, (int)y, 0), boundaryTile);
					}
				}
			}
		}
	}
}