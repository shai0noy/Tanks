using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour {
    private class TurnTimedCaller : IEnumerator {
        private bool ended = false;
        private Action callback;
        private IEnumerator enumer;
        private float endTime;

        public TurnTimedCaller(Action onEndedCallback, float time) {
            callback = onEndedCallback;
            endTime = Time.time + time;
            enumer = timer(time);
        }

        public void end() {
            ended = true;
        }

        private IEnumerator timer(float time) {
            yield return new WaitForSeconds(time);
            if (! ended) {
                callback();
            }
        }

        public object Current {
            get { return enumer.Current; }
        }
        public bool MoveNext() {
            return enumer.MoveNext();
        }
        public void Reset() {
            enumer.Reset();
        }

        // in seconds
        public float timeLeft() {
            return endTime - Time.time;
        }
    }
    private TurnTimedCaller startTimer(Action onEndedCallback, float time) {
        TurnTimedCaller t = new TurnTimedCaller(onEndedCallback, time);
        StartCoroutine(t);
        return t;
    }

    public float EndturnDuration = 3f;
    public float TurnDuration = 15f;

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


    private TurnTimedCaller curTurnTimer = null;

    private int _activeTankId = -1;
    
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
            if (_activeTankId == -1)
                return null;
            return tanks[_activeTankId];
        }
        set {
            if (value != null) {
                activeTankId = Array.IndexOf(tanks, value);
            } else {
                activeTankId = -1;
            }
        }
    }
    public void activateNextTank() {
        activeTankId = (activeTankId + 1) % numTanks; 
    }
    public void activateFirstTank() {
        activeTankId = 0;
    }
    private void _onActiveTankChanged(TankController activeTank, TankController prevActiveTank) {
        if (prevActiveTank != null)
            prevActiveTank.Deactivate();
        activeTank.Activate();
        camManager.setDefaultTarget(activeTank.gameObject);
        updateGui();
    }


    internal void tankFired(TankController shootingTank) {
        stopMainTurn();
    }

    private void startFirstTurn() {
        startTurn();
        activateFirstTank();
    }
    private void startNextTurn() {
        activateNextTank();
        startTurn();
    }
    private void startTurn() {
        curTurnTimer =  startTimer(stopCompleteTurn, TurnDuration);
    }
    private void stopMainTurn() {
        curTurnTimer.end();
        activeTank.DisableFire();
        startEndturn();
    }
    private void startEndturn() {
        curTurnTimer = startTimer(stopEndturn, EndturnDuration);
    }
    private void stopEndturn()  {
        curTurnTimer.end();
        activeTank.Deactivate();
        stopCompleteTurn();
        
    }
    private void stopCompleteTurn() {
        curTurnTimer.end();
        startNextTurn();
    }

    // Use this to find objects
    void Awake() {
        surfaceManager = GetComponentInChildren<SurfaceManager>();
        camManager = GetComponentInChildren<CameraManager>();
        dayManager = GetComponentInChildren<DayManager>();
    }


	// Use this for initialization
	void Start () {
        surfaceManager.buildTerrain(); // Make sure terrain is built (it may alrady be built)

        tanks = new TankController[numTanks];
        for (int i = 0; i < numTanks; i++) {
            float x = 30 * i;
            float y = surfaceManager.surfaceYAt(x) + 10;
            GameObject newTank = GameObject.Instantiate(tankPrefab,  new Vector3(x,y,0)  , Quaternion.identity) as GameObject;
            tanks[i] = newTank.GetComponent<TankController>();
        }
        startFirstTurn();
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
    }


    public void updateGui() {
        hud.life.set(activeTank.armor, activeTank.baseArmor);
        hud.turnTime.setDisplayColor(activeTank.armor <= 15);

        hud.cannonAngle.set(activeTank.aimAngle);
        hud.cannonStrength.set(100 * activeTank.shotPower / activeTank.baseMaxShotPower);

        hud.wind.set(Mathf.Abs(wind), wind > 0 ? "<<" : ">>");
        hud.time.set(dayManager.getHour(), dayManager.getMinute());
        if (curTurnTimer == null) {
            hud.turnTime.set(0);
        } else {
            int time = Mathf.CeilToInt(curTurnTimer.timeLeft());
            hud.turnTime.set(time);
            hud.turnTime.setDisplayColor(time <= 5);
        }
    }

}
