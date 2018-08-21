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
    private bool timerOn = false;

    private bool alarmSet = false;
    private float alarmTime = 0;
    private bool repeatTimer = false;

    void Start (){
        timerOn = true;
    }

	// Update is called once per frame
	void Update () {
        print(timeElapsed.ToString());
        if(timerOn){
            timeElapsed += Time.deltaTime;
            if(alarmSet && timeElapsed > alarmTime){
                alarm();
                print("Alarm");
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

    public void TimerOn(bool timerOn){
        this.timerOn = timerOn;
    }

    public void ResetTimer(){
        timeElapsed = 0;
        timerOn = true;
    }
}
