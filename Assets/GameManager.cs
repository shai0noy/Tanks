using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour {

    private SurfaceManager surfaceManager;
    private CameraManager camManager;


    public int numTanks = 2;
    public GameObject tankPrefab;
    private TankController[] tanks;

    private int _activeTankId;
    public int activeTankId {
        get {
            return _activeTankId;
        }
        private set {
            TankController prevActive = activeTank;
            _activeTankId = value;
            _onActiveTankChanged(activeTank, prevActive);
        }
    }
    public TankController activeTank {
        get {
            return tanks[_activeTankId];
        }
        set {
            activeTankId = Array.IndexOf(tanks, value);
        }
    }
    public void activateNextTank() {
        activeTankId = (activeTankId + 1) % numTanks; 
    }

    private void _onActiveTankChanged(TankController activeTank, TankController prevActiveTank) {
        if (prevActiveTank != null)
            prevActiveTank.Deactivate();
        activeTank.Activate();
        camManager.setDefaultTarget(activeTank.gameObject);
    }

    // Use this to find objects
    void Awake() {
        surfaceManager = GetComponentInChildren<SurfaceManager>();
        camManager = GetComponentInChildren<CameraManager>();
    }

	// Use this for initialization
	void Start () {
        tanks = new TankController[numTanks];
        for (int i = 0; i < numTanks; i++) {
            GameObject newTank = GameObject.Instantiate(tankPrefab, surfaceManager.transform.position 
                + (30 * i * Vector3.left) + (3 * Vector3.up)  , Quaternion.identity) as GameObject;
            tanks[i] = newTank.GetComponent<TankController>();
        }
        activeTankId = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate() {
            if (Input.GetKeyDown("tab")) {
                activateNextTank();
            }
    }

}
