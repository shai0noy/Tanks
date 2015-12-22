using UnityEngine;
using System.Collections;

public class DayManager : MonoBehaviour {

    public Light light;

    public float cycleAngle = 0;
    public float dayDuration = 60;

    private float startTime;

    private float timeOfDay() {
        return (Time.time - startTime) % dayDuration / dayDuration;
    }

    private int dayNumber() {
        return (int)((Time.time - startTime) / dayDuration);
    }


	// Use this for initialization
	void Start () {
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        float tod = timeOfDay();
        light.transform.rotation = Quaternion.Euler(tod * 360,cycleAngle,0);
        light.color = Color.Lerp(Color.white, Color.red, Mathf.Abs(tod-0.5f) * 2);
	}


}
