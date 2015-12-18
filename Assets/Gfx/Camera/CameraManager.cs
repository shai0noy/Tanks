using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

    public GameObject defaultTarget;
    public GameObject mainTarget;
    public GameObject secondaryTarget = null;

    public float lookSpeed = 1.5f;
    public float moveSpeed = 0.5f;
    public float zoomSpeed = 1f;
    public float zoomPerVelocity = 1;
    public float baseFOV = 40;
    public float maxFOV = 90;

    private Camera thisCamera;

	// Use this for initialization
	void Start () {
        transform.LookAt(mainTarget.transform);
        thisCamera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        if (mainTarget == null) {
            mainTarget = defaultTarget;
        }
        lerpLookAt(mainTarget.transform.position);
        lerpInfront(mainTarget.transform.position);
        lerpZoom(mainTarget.GetComponent<Rigidbody2D>().velocity);
	}

    private void lerpLookAt(Vector3 target) {
        Vector3 pos = target - transform.position;
        Quaternion newRot = Quaternion.LookRotation(pos);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, lookSpeed * Time.deltaTime);
    }

    private void lerpInfront(Vector3 target) {
        Vector3 targetPos = transform.position;
        targetPos.x = target.x;
        transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }

    private void lerpZoom(Vector2 targetVelocity)
    {
        float fov = baseFOV + (Mathf.Abs(targetVelocity.x) + targetVelocity.y / 2f) * zoomPerVelocity;
        fov = Mathf.Min(maxFOV, fov);
        thisCamera.fieldOfView += (fov - thisCamera.fieldOfView) * zoomSpeed * Time.deltaTime;
    }
}
