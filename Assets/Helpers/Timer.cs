using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {

    public delegate void Alarm();
    public Alarm alarm;

    private float timeElapsed = 0;
    public float TimeElapsed {
        get{
            return timeElapsed;
        }
    }
    private bool timerOn;

    private bool alarmSet = false;
    private float alarmTime = 0;
    private bool repeatTimer = false;

	// Update is called once per frame
	void Update () {
        if(timerOn){
            timeElapsed += Time.deltaTime;
            if(alarmSet && timeElapsed > alarmTime){
                alarm();
                if(repeatTimer){
                    timeElapsed = 0;
                } else {
                    Destroy(this);
                }
            }
        }
	}

    public Alarm SetAlarm(float alarmTime, bool repeatTimer = false) {
        alarmSet = true;
        this.alarmTime = alarmTime;
        this.repeatTimer = repeatTimer;
        return alarm;
    }
}
