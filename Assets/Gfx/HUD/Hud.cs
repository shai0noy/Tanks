using UnityEngine;
using System.Collections;

public class Hud : MonoBehaviour {


    public TextDisplay life;
    public TextDisplay cannonAngle;
    public TextDisplay cannonStrength;

    public TextDisplay wind;
    public TextDisplay time;

	// Use this for initialization
	void Start () {
        life = GameObject.Find("Life").GetComponent<TextDisplay>();
        cannonAngle = GameObject.Find("CannonAngle").GetComponent<TextDisplay>();
        cannonStrength = GameObject.Find("CannonStrength").GetComponent<TextDisplay>();

        wind = GameObject.Find("Wind").GetComponent<TextDisplay>();
        time = GameObject.Find("Time").GetComponent<TextDisplay>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
