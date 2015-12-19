using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public int numTanks = 2;

    public GameObject tankPrefab;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < numTanks; i++) {
            GameObject nreTank = GameObject.Instantiate(tankPrefab);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
