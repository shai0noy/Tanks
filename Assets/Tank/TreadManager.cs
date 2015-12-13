using UnityEngine;
using System.Collections;

public class TreadManager : MonoBehaviour {

	public bool isGrounded = false;

	private GameObject terrain;


	void Start() {
		terrain = GameObject.Find("Terrain");
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject == terrain) {
			isGrounded = true;
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject == terrain) {
			isGrounded = false;
		}
	}
}
