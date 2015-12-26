using UnityEngine;
using System.Collections;

public class TreadManager : MonoBehaviour {

	public bool isGrounded = false;

	private GameObject terrain;
    public TankController tank;

	void Start() {
		terrain = GameObject.Find("Terrain");
        tank.groundedChanged(false);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject == terrain) {
			isGrounded = true;
            tank.groundedChanged(true);
        }
        
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject == terrain) {
			isGrounded = false;
            tank.groundedChanged(false);
		}
	}
}
