using UnityEngine;
using System.Collections;
using System;

public class TerrainManager : MonoBehaviour {
	
	private Terrain terrain;
	private EdgeCollider2D collider2D;
	private Vector2[] gameSection;

	public float groundRemoveDepthRatio = 0.4f;

	public float width {
		get { return terrain.terrainData.size.x ; }
	}

	public float startX {
		get { return transform.position.x ; }
	}

	private int heightmapVOfGameSection;

	/****************/
	

	public void explodeAt(Vector2 point, float radius) {
		Vector2 localPoint = point - (Vector2)(transform.position);
		for (int i = 0; i < gameSection.Length; i++) {
			float newY = -Mathf.Sqrt( Mathf.Pow(radius,2) - Mathf.Pow(gameSection[i].x-localPoint.x, 2))*groundRemoveDepthRatio + localPoint.y;
			if ( ! float.IsNaN(newY) && newY < gameSection[i].y) {
				gameSection[i].y = newY;	
			}
		}
		collider2D.points = gameSection;


		int heightmapURadius = (int) Mathf.Ceil(radius / terrain.terrainData.heightmapScale.x);
		int heightmapVRadius = (int) Mathf.Ceil(radius / terrain.terrainData.heightmapScale.z);
		int heightmapUCenterPos = (int) (localPoint.x / terrain.terrainData.heightmapScale.x);
		int heightmapVCenterPos = heightmapVOfGameSection;

		int heightmapUStartPos = Math.Max(0, heightmapUCenterPos-heightmapURadius);
		int heightmapVStartPos = Math.Max(0, heightmapVCenterPos-heightmapVRadius);
		int heightmapUEndPos = Math.Min(terrain.terrainData.heightmapWidth-1, heightmapUCenterPos+heightmapURadius);
		int heightmapVEndPos = Math.Min(terrain.terrainData.heightmapHeight-1, heightmapVCenterPos+heightmapVRadius);

		float[,] heights = terrain.terrainData.GetHeights(heightmapUStartPos, heightmapVStartPos, heightmapUEndPos-heightmapUStartPos, heightmapVEndPos-heightmapVStartPos);
		for (int du = 0; du < heights.GetLength(1); du++) {
			float x = (heightmapUStartPos + du) * terrain.terrainData.heightmapScale.x - localPoint.x;
			for (int dv = 0; dv < heights.GetLength(0); dv++) {
				float z = (heightmapVStartPos + dv) * terrain.terrainData.heightmapScale.z + transform.position.z;
				float newWorldY = (float) (localPoint.y - groundRemoveDepthRatio*Math.Sqrt(Mathf.Pow(radius,2f) - Mathf.Pow(x,2f) - Mathf.Pow(z,2f)));
				if ( ! float.IsNaN(newWorldY)) {
					newWorldY = newWorldY / terrain.terrainData.heightmapScale.y;
					if (heights[dv,du] > newWorldY) {
						heights[dv,du] = newWorldY;
					}
				}
			}
		}
		terrain.terrainData.SetHeights(heightmapUStartPos, heightmapVStartPos, heights);
	}

	void calculateStartSurface() {
		float[,] heights = new float[terrain.terrainData.heightmapHeight,terrain.terrainData.heightmapWidth];
		for (int u = 0; u < heights.GetLength(1); u++) {
			for (int v = 0; v < heights.GetLength(0); v++) {
					heights[v,u] = 0.5f;
			}
		}
		terrain.terrainData.SetHeights(0,0,heights);

		gameSection = new Vector2[terrain.terrainData.heightmapWidth];
		heightmapVOfGameSection =  - (int)(transform.position.z / terrain.terrainData.heightmapScale.z);
		for (int ix = 0; ix < gameSection.Length; ix++) {
			gameSection[ix] = new Vector2(ix * terrain.terrainData.heightmapScale.x, terrain.terrainData.GetHeight(ix, heightmapVOfGameSection));
		}
		collider2D.points = gameSection;
	}


	// Use this for initialization
	void Start () {
		terrain = Terrain.activeTerrain;
		collider2D = GetComponent<EdgeCollider2D>();
		calculateStartSurface();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
