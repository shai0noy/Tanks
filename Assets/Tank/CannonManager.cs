using UnityEngine;
using System.Collections;

public class CannonManager : MonoBehaviour {

    public int explosionSize = 15;
    private ParticleEmitter explosion;

	// Use this for initialization
	void Start () {
        explosion = GameObject.Find("Explosion").GetComponent<ParticleEmitter>();
        explosion.emit = false;
	}
	

	public void setAngle(float angle) {
		transform.rotation = Quaternion.Euler(0, 0, angle);
	}

    public void startShotEffect() {
        explosion.Emit(explosionSize); 
    }
}
