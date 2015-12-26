using UnityEngine;
using System.Collections;

public class Airborne : MonoBehaviour {

    const float WIND_BASE = 0.2f;

    public bool inAir = true;
    /// <summary>
    /// 1 to 0
    /// 1 = Not affected by wind; 0 = Full wind effect
    /// </summary>
    public float windResistance = 0;

    private GameManager gameManager;
    private Rigidbody2D rigidBody;

	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody2D>();
        gameManager = FindObjectOfType<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate() {
        if (inAir) {
            float windForce = gameManager.wind * (1 - windResistance);
            rigidBody.AddForce(Vector3.left * WIND_BASE * windForce, ForceMode2D.Force);
        }
    }

}
