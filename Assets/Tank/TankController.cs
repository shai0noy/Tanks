using UnityEngine;
using System.Collections;

public class TankController : TakesDamage {

	public TreadManager treads;
	public CannonManager cannon;

    public AudioSource cannonFireAudio;

	public float aimSpeed = 1;

	public float baseEnginePower = 200;
	public float enginePower;

	public float baseArmor = 100;
	public float armor;

	public float baseMaxShotPower = 20;
	public float maxShotPower = 20;
	private float _shotPower = 10;
	public float shotPower {
		get { return _shotPower; }
		set { 
			_shotPower = Mathf.Clamp(value, 0, maxShotPower);
		}
	}

	public float baseFuelTankIntegrity = 100;
	public float fuelTankIntegrity;


	private float _aimAngle = 45;
	public float aimAngle {
		get { return _aimAngle; }
		set { 
			_aimAngle = value;
			cannon.setAngle(value);
		}
	}


	public Bomb[] missiles;
	public int selectedMissileIndex;

    private CameraManager camManager;

    private Airborne airborneManager;
    private Rigidbody2D rigidBody;

    private bool canShoot = false;

	private enum SmokeStrength {
		none,
		idle,
		drive
	}
	private ParticleEmitter idleSmokeEmitter;
	private ParticleEmitter driveSmokeEmitter;
	private void setSmoke(SmokeStrength strength) {
		idleSmokeEmitter.emit = (strength == SmokeStrength.idle);
		driveSmokeEmitter.emit = (strength == SmokeStrength.drive);
	}

	public float mass
	{
		get { return rigidBody.mass; }
		set { rigidBody.mass = value; }
	}

	public float centerOfMassHeight 
	{
		get { return rigidBody.centerOfMass.y; }
		set { rigidBody.centerOfMass = new Vector2(0, value); }
	}


    public bool isActive {
        get;
        private set;
    }


    private GameManager gameManager;

	public TankController() {
		enginePower = baseEnginePower;
		armor = baseArmor;
		maxShotPower = baseMaxShotPower;
		fuelTankIntegrity = baseFuelTankIntegrity;
	}


	/* --- Behaviour - Passive --- */

	private void die() {
		Debug.Log ("Tank died!");
	}

	private void takeDamage(float damage) {
		//First, armor takes some of the damage
		float armorAbsorb = Mathf.Min(1f, armor / baseArmor) * damage;
		armor -= armorAbsorb;
		if (armor <= 0) {
			armor=0;
			die();
		}
		// Rest of damage is randomly distributed between engine, fuel tank and cannon
		damage -= armorAbsorb;
		//TODO: This should be random:
		enginePower -= damage / 3;
		shotPower -= damage / 3;
		fuelTankIntegrity -= damage / 3;

        //gameManager.updateGui();
	}


	// implement TakesDamage
	public override void hit (float damage) {
		takeDamage(damage);
	}
	
	public override void takeBlastPush(Vector2 force) {
		GetComponent<Rigidbody2D>().AddForce (force, ForceMode2D.Impulse);
	}
	
	public override void takeBlastDamage(float damage) {
		takeDamage(damage);
	}

	/* --- Behaviour - Active --- */

	private void shoot() {
        if (canShoot) {
            Quaternion aimRotation = Quaternion.Euler(0, 0, aimAngle);
            Bomb missile = Instantiate(missiles[selectedMissileIndex], transform.position + aimRotation * Vector3.up * 2f, aimRotation) as Bomb;
            missile.GetComponent<Rigidbody2D>().AddRelativeForce(_shotPower * Vector2.up, ForceMode2D.Impulse);
            cannonFireAudio.Play();
            cannon.shotEffect();
            // Set camera to folloe missile.
            camManager.mainTarget = missile.gameObject;
            canShoot = false;
            gameManager.tankFired(this);
        }
	}
    public void Activate() {
        isActive = true;
        canShoot = true;
    }
    public void DisableFire() {
        canShoot = false;
    }
    public void Deactivate() {
          isActive = false;
          canShoot = false;
          setSmoke(SmokeStrength.none); //Maybe idle smoke?
    }

    internal void groundedChanged(bool touchingGround) {
        this.airborneManager.inAir = !touchingGround;
    }
    
    // Use this to find objects
    void Awake() {
        idleSmokeEmitter = transform.Find("IdleSmoke").GetComponent<ParticleEmitter>();
        driveSmokeEmitter = transform.Find("DrivingSmoke").GetComponent<ParticleEmitter>();
        airborneManager = GetComponent<Airborne>();
        camManager = Camera.main.GetComponent<CameraManager>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
        rigidBody = GetComponent<Rigidbody2D>();
    }

	// Use this for initialization
	void Start () {
		centerOfMassHeight = 0.1f;
	}


	void FixedUpdate () {
        if (! isActive)
            return;

		/* Drive */
		float power = Input.GetAxis ("Horizontal") * enginePower;
		if (treads.isGrounded) {
			GetComponent<Rigidbody2D>().AddForce (transform.right * power);
		}
		if (power == 0) {
			setSmoke(SmokeStrength.idle);
		} else {
			setSmoke(SmokeStrength.drive);
		}

		/* Aim */

		float aimDelta = Input.GetAxis ("Vertical") * aimSpeed;
		aimAngle = Mathf.Clamp(aimAngle + aimDelta, -90, 90);

        if (Input.GetKey("=")) {
            shotPower += 1;
        }
        if (Input.GetKey("-")) {
            shotPower -= 1;
        }

		/* Shoot */

		if (Input.GetKeyDown("space")) {
			shoot();
		}
        
	}


	// Update is called once per frame
	void Update () {
	
	}
}
