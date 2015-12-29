using UnityEngine;
using System.Collections;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class SurfaceManager : MonoBehaviour {
    // This class creates a mesh with size of 1*1*1
    // It's then sets the object's scale and position according to parameters
    private Vector2 YZ(this Vector3 v) {
        return new Vector2(v.y, v.z);
    }

    public int overlayResolution = 1000;
    private Texture2D overlayTexture;
    public Color InnerColor;

    public float groundRemovalEffect = 1f;

    /// <summary>
    /// 0 to 1
    /// </summary>
    private int surfaceZ = 50;
    public float surfacePlaneRelativePos {
        get { return surfaceZ / depthResolution; }
        set { surfaceZ = (int) (value * depthResolution); }
    }


    public int surfaceResolution = 150;
    public int depthResolution = 30;
    public float mapWidth = 150;
    public float mapDepth = 100;

    public float surfaceSeed;
    public float surfaceFreq = 25f;
    public float surfaceMinY = -1.5f;
    public float surfaceMaxY = 2.5f;

    private float mapHeight {
        get { return surfaceMaxY - surfaceMinY; }
    }


    Vector2[] surfaceOriginal;
    Vector2[] surfaceCurrnet;

    Vector3[] vertices_data;
    private class Arr2DMapper<T> {
        private T[] arr;
        public int lenX;
        public int lenY;
        public Arr2DMapper(ref T[] arr, int lenX) {
                this.arr = arr;
                this.lenX = lenX;
                this.lenY = arr.Length / lenX;
        }
        public T this[int x, int y] {
            get { return arr[x * lenX + y]; }
            set { arr[x * lenX + y] = value; }
        }
    }
    Arr2DMapper<Vector3> vertices;

    private Vector3 lerpFromVertices(float x, float y) {
        int xIndex = (int)x;
        int yIndex = (int)y;

        float lerpValX = x - xIndex;
        float lerpValY = y - yIndex;

        Vector3 mid1 = Vector3.Lerp(vertices[xIndex, yIndex], vertices[xIndex + 1, yIndex], lerpValX);
        Vector3 mid2 = Vector3.Lerp(vertices[xIndex, yIndex+1], vertices[xIndex + 1, yIndex+1], lerpValX);

        return Vector3.Lerp(mid1, mid2, lerpValY);
    }


    // Use this for initialization
    void Start() {
  
        float aspectRatio = mapDepth / mapWidth;
        overlayTexture = new Texture2D(overlayResolution, (int)aspectRatio * overlayResolution);

        transform.localScale = new Vector3(mapWidth/surfaceResolution, mapHeight, mapDepth/depthResolution);
        float zOffset = transform.TransformVector(0, 0, surfacePlaneRelativePos).z;
        transform.position = new Vector3(mapWidth / 2, surfaceMinY, zOffset);

        buildMesh();
        buildSurface();
    }




    public void explodeAt(Vector2 worldPoint2d, float radius) {
        Vector3 localPoint = transform.InverseTransformPoint(worldPoint2d.x, worldPoint2d.y, 0); // localPoint.z will therfore be surfaceZ
        Vector3 localRadius = transform.InverseTransformVector(Vector3.one * radius);

        localRadius.y *= groundRemovalEffect;

        int startX = (int)(localPoint.x + localRadius.x);
        int endX = Mathf.CeilToInt(localPoint.x + localRadius.x);
        int startZ = (int)(localPoint.x + localRadius.x);
        int endZ = Mathf.CeilToInt(localPoint.z + localRadius.z);
        
        for (int x = startX; x < endX; x++) {
            for (int z = startZ; x < endZ; z++) {
                //Vector2 delta = localPoint - surfaceCurrnet[x];
                //if (delta.magnitude < localRadius) { // need to check if inside ellipse

                Vector3 delta = new Vector3(x, localPoint.y, z) - localPoint;
                delta = Vector3.Cross(delta , localRadius);
                float newYAtThisPoint = Mathf.Sqrt(1 - Mathf.Pow(delta.z, 2) - Mathf.Pow(delta.x, 2));

                if (newYAtThisPoint < vertices[x, z].y) {
                    vertices[x,z].Set(x, newYAtThisPoint,z);
                    //    }
                }
            }
        }

        buildMesh(); //Should be more efficient
    }


    void updateCollider() {
        EdgeCollider2D collider = GetComponent<EdgeCollider2D>();
        collider.points = surfaceCurrnet;
    }

    void buildSurface() {
        surfaceOriginal = new Vector2[surfaceResolution];
        surfaceCurrnet = new Vector2[surfaceResolution];

        for (int x = 0; x < surfaceResolution; x++) {
            surfaceOriginal[x] = new Vector2(x, vertices[x, surfaceZ].y);
            surfaceCurrnet[x] = surfaceOriginal[x];
        }
        surfaceOriginal.CopyTo(surfaceCurrnet, 0);

        updateCollider();
    }

    void buildMesh() {

        int numVerts = depthResolution * surfaceResolution;

        vertices_data = new Vector3[numVerts];
        Vector2[] uv = new Vector2[numVerts];

        vertices = new Arr2DMapper<Vector3>(ref vertices_data, surfaceResolution);
        if (vertices.lenY != depthResolution)
            Debug.LogErrorFormat("Error: vertices mapping got wrong depth: resoltion is {0}, but matrix lenY is {1}", depthResolution,vertices.lenY );

        float perlinStart = surfaceSeed * 10;
        float perlinStep = surfaceFreq / surfaceResolution;

        for (int x= 0; x < surfaceResolution; x++) {
            for (int z = 0; z < depthResolution; z++) {
                float y = Mathf.PerlinNoise(perlinStart + x * perlinStep, perlinStart + z * perlinStep);
                vertices[x, z] = new Vector3(x,y,z);
            }
        }

        for (int i = 0; i < vertices_data.Length; i++) {
            uv[i] = new Vector2(vertices_data[i].x, vertices_data[i].z);
        }


        // Build array of triangle data, elements are indexes in the vertices array.
        // each 3 elemnts (=indexes) in the array define a mesh triangle.
        int numTriangles = (surfaceResolution - 1) * (depthResolution - 1) * 2;
        int[] triangles = new int[numTriangles * 3];

        int iTriangle = 0;
        for (int iSurface = 0; iSurface < surfaceResolution - 1; iSurface++) {
            for (int iDepth = 0; iDepth < depthResolution - 1; iDepth++) {

                triangles[iTriangle + 0] = iSurface * depthResolution + iDepth;
                triangles[iTriangle + 1] = (iSurface + 1) * depthResolution + iDepth;
                triangles[iTriangle + 2] = iSurface * depthResolution + (iDepth + 1);

                triangles[iTriangle + 3] = (iSurface + 1) * depthResolution + (iDepth + 1);
                triangles[iTriangle + 4] = iSurface * depthResolution + (iDepth + 1);
                triangles[iTriangle + 5] = (iSurface + 1) * depthResolution + iDepth;

                iTriangle += 6;
            }
        }

        /* Set Mesh */

        Mesh mesh = new Mesh();
        mesh.vertices = vertices_data;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.uv = uv;

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;


        /* Set Texture */
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material.SetTexture(1, overlayTexture);
    }



    public float yAt(float worldX, float worldZ) {
        Vector3 local = transform.InverseTransformPoint(worldX, 0, worldZ);
        int xIndex = (int)local.x;
        float lerpVal = local.x - xIndex;
        return Mathf.Lerp(surfaceOriginal[xIndex].y, surfaceOriginal[xIndex + 1].y, lerpVal);
    }

    public float surfaceYAt(float worldX) {
        float localX = transform.InverseTransformPoint(worldX, 0, 0).x;
        int xIndex = (int) localX;
        float lerpVal = localX - xIndex;
        return Mathf.Lerp(surfaceOriginal[xIndex].y, surfaceOriginal[xIndex + 1].y, lerpVal);
    }


    public float getMinViewAngleFromEdge(Vector3 targetPos) {
        /*
        float max = 0; 
        targetLocalY = 
        for (int z = surfaceZ; z < depthResolution; z++) {
            max= Mathf.Max(max, (vertices[ - targetPos.y) / (z - targetPos.z))
        }
        return Mathf.Rad2Deg * Mathf.Atan();
         * */
        return 0;
    }

}


