using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Workflow : MonoBehaviour {

    //public static class Notifications {
    //    public string Tutorial 
    //}
    public int progress = 0;
    private MapMode mapMode;
    public bool placedObject = false;
    public bool objectReloaded = false;
    public bool tutorialFinished = false;

    void Start(){
        if(PlayerPrefs.GetInt("IsMappingMode") == 1){
            mapMode = MapMode.MapModeMapping;
        } else {
            mapMode = MapMode.MapModeLocalization;
        }
    }

}
