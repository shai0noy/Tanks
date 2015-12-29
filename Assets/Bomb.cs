using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour {

	public int blastDamage = 5;
	public int blastPush = 1000;
	public int blastRadius = 5;
	public int directHitDamage = 5;

	public int groundRemoveRadius = 5;

    public int timeToLive = 10;

    public GameObject explosion;

	SurfaceManager terrainManager;

	// Use this for initialization
	void Awake () {
		GameObject terrain = GameObject.Find("Terrain");
		terrainManager = terrain.GetComponent<SurfaceManager>(); 
	}

    void Start() {
        Destroy(gameObject, timeToLive);
        if (terrainManager.surfaceYAt(transform.position.x) > transform.position.y) {
            // bomb is below surface
            explode();
            Debug.LogWarning("bomb clipped");
        }
    }

	// Update is called once per frame
	void Update () {

	}
	
	void hit(TakesDamage damagable) {
		damagable.hit (directHitDamage);
	}

	void explode() {
		TakesDamage[] damagables = FindObjectsOfType<TakesDamage> ();
		foreach (TakesDamage damagable in damagables) {
			damagable.blasted (transform.position, blastRadius, blastDamage, blastPush);
		}
		terrainManager.explodeAt (transform.position, groundRemoveRadius);
        GameObject.Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy (gameObject);
	}


	void OnCollisionEnter2D (Collision2D collision) {
		TakesDamage damagable = collision.gameObject.GetComponent<TakesDamage> ();
		if (damagable != null) {
			hit (damagable);
		}
		explode ();
	}

}
