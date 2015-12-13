using UnityEngine;
using System.Collections;

public class CannonManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void setAngle(float angle) {
		transform.rotation = Quaternion.Euler(0, 0, angle);
	}

}
