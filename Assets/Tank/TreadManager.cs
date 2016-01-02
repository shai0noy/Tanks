using UnityEngine;
using System.Collections;

public class TreadManager : MonoBehaviour {

    private int numContactPoints = 0;

    public bool isGrounded;

    public Collider2D leftEdge;
    public Collider2D rightEdge;

	private GameObject terrain;
    public TankController tank;

	void Start() {
		terrain = GameObject.Find("Terrain");
        tank.groundedChanged(false);
	}

	void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject != terrain)
            Debug.LogWarning(other.gameObject);
        numContactPoints++;

        isGrounded = numContactPoints > 0;
        tank.groundedChanged(isGrounded); 
	}

	void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject != terrain)
            Debug.LogWarning(other.gameObject);
        numContactPoints--;

        isGrounded = numContactPoints > 0;
        tank.groundedChanged(isGrounded);
	}

}
