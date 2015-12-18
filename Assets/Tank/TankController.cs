using UnityEngine;
using System.Collections;

public class TankController : TakesDamage {

	public TreadManager treads;
	public CannonManager cannon;

	public float aimSpeed = 1;

	public float baseEnginePower = 200;
	public float enginePower;

	public float baseArmor = 100;
	public float armor;

	public float baseMaxShotPower = 20;
	public float maxShotPower;
	private float _shotPower = 10;
	public float shotPower {
		get { return _shotPower; }
		set { 
			_shotPower = Mathf.Clamp (value, 0, maxShotPower);
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

	private enum SmokeStrength {
		none,
		idle,
		drive
	}
	private ParticleEmitter idleSmokeEmitter;
	private ParticleEmitter driveSmokeEmitter;
	private void setSmoke(SmokeStrength strength) {
		idleSmokeEmitter.emit = strength == SmokeStrength.idle;
		driveSmokeEmitter.emit = strength == SmokeStrength.drive;
	}

	public float mass
	{
		get { return GetComponent<Rigidbody2D>().mass; }
		set { GetComponent<Rigidbody2D>().mass = value; }
	}

	public float centerOfMassHeight 
	{
		get { return GetComponent<Rigidbody2D>().centerOfMass.y; }
		set { GetComponent<Rigidbody2D>().centerOfMass = new Vector2(0, value); }
	}


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
		float armorAbsorb = Mathf.Min (1f, armor / baseArmor) * damage;
		armor -= armorAbsorb;
		if (armor <= 0) {
			armor=0;
			die();
		}
		// Rest of damage is randomly ditributed between engine, fuel tank and cannon
		damage -= armorAbsorb;
		//TODO: This should be random:
		enginePower -= damage / 3;
		shotPower -= damage / 3;
		fuelTankIntegrity -= damage / 3;
	}


	// implement TakesDamage
	public override void hit (float damage) {
		takeDamage (damage);
	}
	
	public override void takeBlastPush(Vector2 force) {
		GetComponent<Rigidbody2D>().AddForce (force, ForceMode2D.Impulse);
	}
	
	public override void takeBlastDamage(float damage) {
		takeDamage(damage);
	}

	/* --- Behaviour - Active --- */

	private void shoot() {
        Quaternion aimRotation = Quaternion.Euler (0,0,aimAngle);
        Bomb missile = Instantiate(missiles[selectedMissileIndex], transform.position + aimRotation * Vector3.up * 2f, aimRotation) as Bomb;
		missile.GetComponent<Rigidbody2D>().AddRelativeForce(_shotPower * Vector2.up, ForceMode2D.Impulse);
        cannon.startShotEffect();
        // Set camera to folloe missile.
        camManager.mainTarget = missile.gameObject;
	}


	/* --- User Control --- */


	// Use this for initialization
	void Start () {
		idleSmokeEmitter = GameObject.Find("IdleSmoke").GetComponent<ParticleEmitter>();
		driveSmokeEmitter = GameObject.Find("DrivingSmoke").GetComponent<ParticleEmitter>();
        camManager = Camera.main.GetComponent<CameraManager>();
		centerOfMassHeight = 0.1f;
	}


	void FixedUpdate () {

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

		/* Shoot */

		if (Input.GetKeyDown("space")) {
			shoot();
		}

	}


	// Update is called once per frame
	void Update () {
	
	}
}
