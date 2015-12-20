using UnityEngine;
using System.Collections;

public class CannonManager : MonoBehaviour {

    public int explosionSize = 15;
    private ParticleEmitter explosion;

    // Use this to find objects
    void Awake() {
        explosion = transform.Find("Explosion").GetComponent<ParticleEmitter>();
    }

	// Use this for initialization
	void Start () {   
        explosion.emit = false;
	}
	

	public void setAngle(float angle) {
		transform.rotation = Quaternion.Euler(0, 0, angle);
	}

    public void shotEffect() {
        explosion.Emit(explosionSize); 
    }
}
