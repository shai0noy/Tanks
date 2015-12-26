using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour {


    public float EndTurnDuration = 3f;
    public float TurnDuration = 15f;

    private float turnStart;

    private SurfaceManager surfaceManager;
    private CameraManager camManager;
    private DayManager dayManager;

    public Hud hud;

    public int numTanks = 2;
    public GameObject tankPrefab;
    private TankController[] tanks;

    public float maxWindDerivative = 0.15f;
    public float maxWind = 55f;
    public float wind = 10;
    private float windDerivative = 0;

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
        updateGui();
    }

    internal void tankFired(TankController shootingTank) {
        //endTurn();
    }


    private void startNextTurn() {
        activateNextTank();
    }


    private IEnumerator GameLoop() {
        while (true) { //TODO: check winner and exit loop
            waitForTurnEnd();
            waitForEndturnEnd();
            activateNextTank();
        }
    }

    private IEnumerator waitForTurnEnd() {
        yield return new WaitForSeconds(TurnDuration - EndTurnDuration);
    }
    private IEnumerator waitForEndturnEnd() {
        yield return new WaitForSeconds(EndTurnDuration);
    }


    // Use this to find objects
    void Awake() {
        surfaceManager = GetComponentInChildren<SurfaceManager>();
        camManager = GetComponentInChildren<CameraManager>();
        dayManager = GetComponentInChildren<DayManager>();
    }

	// Use this for initialization
	void Start () {
        while (true) {
            IEnumerator e = waitForEndturnEnd();
            Debug.LogWarning(e.MoveNext());
            Debug.LogWarning(e.Current);
        }

        tanks = new TankController[numTanks];
        for (int i = 0; i < numTanks; i++) {
            GameObject newTank = GameObject.Instantiate(tankPrefab, surfaceManager.transform.position 
                + (30 * i * Vector3.left) + (5 * Vector3.up)  , Quaternion.identity) as GameObject;
            tanks[i] = newTank.GetComponent<TankController>();
        }
        activeTankId = 0;
        //TODO: Above line causes formating errors end stop[s this method execution!
	}
	
	// Update is called once per frame
	void Update () {
       updateGui();
	}

    void FixedUpdate() {
            if (Input.GetKeyDown("tab")) {
                activateNextTank();
            }

            windDerivative += UnityEngine.Random.Range(-maxWindDerivative / 30f, maxWindDerivative / 30f);
            windDerivative = Mathf.Clamp(windDerivative, -maxWindDerivative, maxWindDerivative);

            wind += windDerivative - wind / 600;
            wind = Mathf.Clamp(wind, -maxWind, maxWind);

            if (turnStart != 0 && Time.time >= turnStart) {
                startNextTurn();
            }

    }


    public void updateGui() {
        hud.life.set(activeTank.armor, activeTank.baseArmor);
        hud.cannonAngle.set(activeTank.aimAngle);
        hud.cannonStrength.set(100 * activeTank.shotPower / activeTank.baseMaxShotPower);

        hud.wind.set(Mathf.Abs(wind), wind > 0 ? "<<" : ">>");
        hud.time.set(dayManager.getHour(), dayManager.getMinute());
    }

}
