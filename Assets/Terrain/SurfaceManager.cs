using UnityEngine;
using System.Collections;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]

public class SurfaceManager : MonoBehaviour {

    public int overlayResolution = 1000;
    private Texture2D overlayTexture;
	public Color InnerColor;
	
	public float groundRemoveDepthRatio = 0.4f;

	public int numSurfacePoints = 150;
	public float surfaceWidth = 150;
	
	public float surfaceSpread = 1.5f;

    public float surfaceSeed;
    public float surfaceFreq = 25f;
    public float surfaceMinY = -1.5f;
    public float surfaceMaxY = 2.5f;



	Vector2[] surfaceOriginal;
	Vector2[] surfaceCurrnet;


	int[] depthZs =         {50, 30, 10,    8,     6,     5,     3,    0,  0,  -3,      -5,      -6,   -8,   -10,  -30, -50};
    float[] modEffect =   {0,   0,    0,   0.1f,  0.2f, 0.5f, 0.8f,  1, 1,  0.8f,   0.5f,    0.2f,  0.1f,   0,     0,     0 };
	int[] depthHeightDeltas = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};

    int depth;
    float aspectRatio;
    
    // Use this for initialization
	void Start () {	
        depth = depthZs[0] - depthZs[depthZs.Length-1]; 
        aspectRatio = depth / surfaceWidth;
        overlayTexture = new Texture2D(overlayResolution, (int)aspectRatio * overlayResolution);
		buildSurface();
		buildMesh();	
	}
	

	public void explodeAt(Vector2 point, float radius) {
		for (int i = 0; i < numSurfacePoints; i++) {
			Vector2 delta = point - surfaceCurrnet[i];
			if (delta.magnitude < radius) {
				float newY = -Mathf.Sqrt( Mathf.Pow(radius,2) - Mathf.Pow(surfaceCurrnet[i].x, 2) +
						2 * surfaceCurrnet[i].x * point.x - Mathf.Pow(point.x, 2) ) * groundRemoveDepthRatio - Mathf.Abs(point.y);
				if (newY < surfaceCurrnet[i].y) {
					surfaceCurrnet[i].y = newY;	
				}
			}
		}

		buildMesh(); //Should be more efficient
	}

	
	void buildSurface() {
		surfaceOriginal = new Vector2[numSurfacePoints];
		surfaceCurrnet = new Vector2[numSurfacePoints];

		float surfaceXStep = surfaceWidth / numSurfacePoints;
		float surfaceXStart = -surfaceWidth / 2;

        float perlinStart = surfaceSeed * 10;
        float perlinStep = surfaceFreq / numSurfacePoints;
        float surfaceMaxDelta = surfaceMaxY - surfaceMinY;

		for (int i = 0; i < numSurfacePoints; i++) {
            float y = Mathf.PerlinNoise(perlinStart, perlinStart + i * perlinStep) * surfaceMaxDelta + surfaceMinY;
            surfaceOriginal[i] = new Vector2(surfaceXStart + (i * surfaceXStep), y);
			surfaceCurrnet[i] = surfaceOriginal[i];
		}

	}

	void buildMesh() {

		float surfaceSpreadPerDepthUnit = (surfaceSpread -1f) / depthZs[0];


		int depthSteps = depthZs.Length;
		
		int numVerts = numSurfacePoints * depthSteps;
		Vector3[] verts = new Vector3[numVerts];
		Vector2[] uv = new Vector2[numVerts];


		int baseVert;
		int iVert;
		for (int iSurface = 0; iSurface < numSurfacePoints; iSurface++) {
			baseVert = iSurface * depthSteps;
			for (int iDepth = 0; iDepth < depthSteps; iDepth++) {
				iVert = baseVert + iDepth;
				Vector2 suracePoint = surfaceCurrnet[iSurface] * modEffect[iDepth] + surfaceOriginal[iSurface] * (1-modEffect[iDepth]);
				verts[iVert] = new Vector3(suracePoint.x * (1 + surfaceSpreadPerDepthUnit * depthZs[iDepth]), suracePoint.y + depthHeightDeltas[iDepth], depthZs[iDepth]);
                uv[iVert] = new Vector2(verts[iVert].x, verts[iVert].z);
			}
		}
		
		int numTriangles = (numSurfacePoints - 1) * (depthSteps - 1) * 2;
		int[] triangles = new int[numTriangles * 3];
		
		int iTriangle = 0;
		for (int iSurface = 0; iSurface < numSurfacePoints - 1; iSurface++) {
			for (int iDepth = 0; iDepth < depthSteps - 1; iDepth++) {
				
				triangles[iTriangle + 0] = iSurface * depthSteps + iDepth;
				triangles[iTriangle + 1] = (iSurface + 1) * depthSteps + iDepth;
				triangles[iTriangle + 2] = iSurface * depthSteps + (iDepth + 1);
				
				triangles[iTriangle + 3] = (iSurface + 1) * depthSteps + (iDepth + 1);
				triangles[iTriangle + 4] = iSurface * depthSteps + (iDepth + 1);
				triangles[iTriangle + 5] = (iSurface + 1) * depthSteps + iDepth;
				
				iTriangle += 6;
			}
		}
		
		/* Set Mesh */		
		
		Mesh mesh = new Mesh();
		mesh.vertices = verts;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		mesh.uv = uv;
			
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		meshFilter.mesh = mesh;

		EdgeCollider2D collider = GetComponent<EdgeCollider2D>();
		collider.points = surfaceCurrnet;

		/* Set Texture */
		MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material.SetTexture(1, overlayTexture);
	}
	

}
