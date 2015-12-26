using UnityEngine;
using System.Collections;

public class DayManager : MonoBehaviour {

    public Light light;

    public float cycleAngle = 0;
    public float dayDuration = 60;

    public Color[] dayLightColors;

    private float startTime;

    /// <summary>
    /// 0 -> 6 AM
    /// 0.5 -> 6 PM
    /// 0.999 -> 5:59 AM
    /// </summary>
    /// <returns>0 to 1 (not including 1)</returns>
    private float timeOfDay() {
        return (Time.time - startTime) % dayDuration / dayDuration;
    }

    private int dayNumber() {
        return (int)((Time.time - startTime) / dayDuration);
    }


    internal int getHour() {
        return ((int) (timeOfDay() * 24) + 6) % 24; // Since timeOfDay start at 6AM
    }
    internal int getMinute() {
        return (int) (timeOfDay() * 24 * 60 % 60);
    }

	// Use this for initialization
	void Start () {
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        float tod = timeOfDay();
        light.transform.rotation = Quaternion.Euler(0, tod * 360, 0) * Quaternion.Euler(20, 0, 0);
        float todColorVal = tod * dayLightColors.Length;
        int colorIndex = (int)todColorVal;
        light.color = Color.Lerp(dayLightColors[colorIndex], dayLightColors[(colorIndex + 1) % dayLightColors.Length], todColorVal - colorIndex);
	}

}
