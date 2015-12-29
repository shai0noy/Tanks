using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

    public GameObject defaultTarget;
    public GameObject mainTarget;
    public GameObject secondaryTarget = null;

    public GameObject userLookTarget;

    public float targetDistance = 40;
    public float lookSpeed = 1.5f;
    public float moveSpeed = 0.5f;
    public float zoomSpeed = 1f;
    public float zoomPerVelocity = 1;
    public float baseFOV = 40;
    public float maxFOV = 90;
    public float minAngle = 30;
    public float addedY = 7;

    private bool userControl = false;

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


    void FixedUpdate() {

        const int borderLookDistance = 22;

        Vector3 userMove = Vector3.zero;
        if (Input.mousePosition.x < borderLookDistance) {
            userMove.x = borderLookDistance / (-Input.mousePosition.x -1);
        } else if (Input.mousePosition.x > Display.main.systemWidth - borderLookDistance) {
            userMove.x = borderLookDistance / (Display.main.systemWidth - Input.mousePosition.x +1);
        }
        if (Input.mousePosition.y < borderLookDistance) {
            userMove.y = borderLookDistance / (-Input.mousePosition.y -1);
        } else if (Input.mousePosition.y > Display.main.systemHeight - borderLookDistance) {
            userMove.y = borderLookDistance / (Display.main.systemHeight - Input.mousePosition.y +1);
        }

        userMove.x /= borderLookDistance;
        userMove.y /= borderLookDistance;

        userControl = (userMove != Vector3.zero);
        if (userControl) {
            mainTarget = userLookTarget;
            userLookTarget.transform.Translate(userMove);
            userLookTarget.SetActive(true);
        } else {
            userLookTarget.transform.position = mainTarget.transform.position;
            userLookTarget.SetActive(false);
        }
    }

	void Update () {

        // Auto Move

        if (mainTarget == null) {
            mainTarget = defaultTarget;
        }
        float ang = surfaceManager.getMinViewAngleFromEdge(mainTarget.transform.position) + 5; // +5 in order to be slightly above the minimal visibility angle
        ang = Mathf.Max(ang, minAngle);
        lerpInfront(mainTarget.transform.position, ang);
        lerpLookAt(mainTarget.transform.position);
        Rigidbody2D targetBody = mainTarget.GetComponent<Rigidbody2D>();
        lerpZoom(targetBody != null ? targetBody.velocity : Vector2.zero);

	}



    private void lerpLookAt(Vector3 target) {
        Vector3 pos = target - transform.position + addedY * Vector3.up;
        Quaternion newRot = Quaternion.LookRotation(pos);
        float speed = userControl ? lookSpeed * 2 : lookSpeed;
        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, lookSpeed * Time.deltaTime);
    }

    private void lerpInfront(Vector3 target, float viewAngle) {
        Vector3 targetPos = target - Quaternion.AngleAxis(viewAngle, Vector3.right) * Vector3.forward * targetDistance + addedY * Vector3.up;
        float speed = userControl ? moveSpeed * 3 : moveSpeed;
        transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
    }

    private void lerpZoom(Vector2 targetVelocity)
    {
        float fov = baseFOV + (Mathf.Abs(targetVelocity.x) + targetVelocity.y / 2f) * zoomPerVelocity;
        fov = Mathf.Min(maxFOV, fov);
        thisCamera.fieldOfView += (fov - thisCamera.fieldOfView) * zoomSpeed * Time.deltaTime;
    }
}
