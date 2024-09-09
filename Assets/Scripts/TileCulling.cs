//using UnityEngine;
//using UnityEngine.Tilemaps;

//public class TileCulling : MonoBehaviour {
//	public Tilemap tilemap;  // Reference to your Tilemap
//	public Transform player; // Reference to the player

//	//void Update() {
//	//	Vector3 playerWorldPos = player.position; // Get player's world position
//	//	Vector3Int playerTilePos = tilemap.WorldToCell(playerWorldPos); // Convert to tilemap cell position

//	//	Debug.Log("Player is standing on tile at position: " + playerTilePos);
//	//}
//	//public Tilemap tilemap;   // Reference to your Tilemap
//	public Camera mainCamera; // Main camera reference
//	public int chunkSize = 32; // Chunk size (e.g., 32x32 tiles)

//	void Update() {
//		CullChunks();
//	}

//	void CullChunks() {
//		// Get camera bounds
//		Vector3 cameraPos = mainCamera.transform.position;
//		float cameraHeight = 2f * mainCamera.orthographicSize;
//		float cameraWidth = cameraHeight * mainCamera.aspect;
//		Bounds cameraBounds = new Bounds(cameraPos, new Vector3(cameraWidth, cameraHeight, 0));

//		// Loop over chunks in the tilemap
//		BoundsInt tilemapBounds = tilemap.cellBounds;

//		for (int x = tilemapBounds.xMin; x < tilemapBounds.xMax; x += chunkSize) {
//			for (int y = tilemapBounds.yMin; y < tilemapBounds.yMax; y += chunkSize) {
//				// Define the bounds of the current chunk
//				Vector3Int chunkPos = new Vector3Int(x, y, 0);
//				Bounds chunkBounds = new Bounds(tilemap.CellToWorld(chunkPos) + new Vector3(chunkSize / 2f, chunkSize / 2f, 0), new Vector3(chunkSize, chunkSize, 0));

//				// Check if the chunk intersects with the camera bounds
//				if (cameraBounds.Intersects(chunkBounds)) {
//					// If within camera bounds, ensure the chunk is visible
//					SetTilemapChunkActive(chunkPos, true);
//				}
//				else {
//					// If outside camera bounds, deactivate the chunk
//					SetTilemapChunkActive(chunkPos, false);
//				}
//			}
//		}
//	}

//	// Activate or deactivate a chunk of tiles
//	void SetTilemapChunkActive(Vector3Int chunkPos, bool active) {
//		for (int x = chunkPos.x; x < chunkPos.x + chunkSize; x++) {
//			for (int y = chunkPos.y; y < chunkPos.y + chunkSize; y++) {
//				Vector3Int tilePos = new Vector3Int(x, y, 0);

//				if (tilemap.HasTile(tilePos)) {
//					if (active) {
//						// Restore the tile (you could cache and restore original tiles if needed)
//						tilemap.SetTile(tilePos, tilemap.GetTile(tilePos));
//					}
//					else {
//						// Remove the tile to "inactivate" it
//						tilemap.SetTile(tilePos, null);
//					}
//				}
//			}
//		}
//	}
//}