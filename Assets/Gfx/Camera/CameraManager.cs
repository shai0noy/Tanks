using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

    public GameObject defaultTarget;
    public GameObject mainTarget;
    public GameObject secondaryTarget = null;

    public float targetDistance = 40;
    public float lookSpeed = 1.5f;
    public float moveSpeed = 0.5f;
    public float zoomSpeed = 1f;
    public float zoomPerVelocity = 1;
    public float baseFOV = 40;
    public float maxFOV = 90;
    public float minAngle = 30;

    private Camera thisCamera;
    private SurfaceManager surfaceManager;

    public void setDefaultTarget(GameObject go) {
        mainTarget = go;
        defaultTarget = go;
    }

    // Use this to find objects
    void Awake() {
        thisCamera = GetComponent<Camera>();
        surfaceManager = GameObject.FindObjectOfType<SurfaceManager>();
    }

	// Use this for initialization
	void Start () {
    //transform.position.Set(transform.position.x, 50, transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
        if (mainTarget == null) {
            mainTarget = defaultTarget;
        }
        float ang = surfaceManager.getMinViewAngleFromEdge(mainTarget.transform.position) + 5; // +5 in order to be slightly above the minimal visibility angle
        ang = Mathf.Max(ang, minAngle);
        lerpInfront(mainTarget.transform.position, ang);
        lerpLookAt(mainTarget.transform.position);
        lerpZoom(mainTarget.GetComponent<Rigidbody2D>().velocity);
	}

    private void lerpLookAt(Vector3 target) {
        Vector3 pos = target - transform.position;
        Quaternion newRot = Quaternion.LookRotation(pos);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, lookSpeed * Time.deltaTime);
    }

    private void lerpInfront(Vector3 target, float viewAngle) {
        Vector3 targetPos = target - Quaternion.AngleAxis(viewAngle, Vector3.right) * Vector3.forward * targetDistance;
        transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }

    private void lerpZoom(Vector2 targetVelocity)
    {
        float fov = baseFOV + (Mathf.Abs(targetVelocity.x) + targetVelocity.y / 2f) * zoomPerVelocity;
        fov = Mathf.Min(maxFOV, fov);
        thisCamera.fieldOfView += (fov - thisCamera.fieldOfView) * zoomSpeed * Time.deltaTime;
    }
}
