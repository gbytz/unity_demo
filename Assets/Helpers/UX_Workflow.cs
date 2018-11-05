using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.iOS;

public class UX_Workflow : MonoBehaviour {

    public Tutorial_Workflow tutorial;
    public GameObject exitPanel;
    public Text exitPanelText;
    public GameObject notificationPanel;
    public Text notification;
    public GameObject helpPanel;
    public GameObject addButton;
    private MapSession mapSession;
    private bool tutorialCompleted = false;

    public int progress = 0;
    private MapMode mapMode;
    private bool placedObject = false;
    public bool objectReloaded = false;
    private bool tutorialWorkflowDone = false;

    private Timer tipTimer;
    private int tipCount;

    void Start(){
        mapSession = GameObject.Find("MapSession").GetComponent<MapSession>();

        if(PlayerPrefs.GetInt("IsMappingMode") == 1){
            mapMode = MapMode.MapModeMapping;
        } else {
            mapMode = MapMode.MapModeLocalization;
        }

        if (PlayerPrefs.HasKey("TutorialCompleted"))
        {
            int tut = PlayerPrefs.GetInt("TutorialCompleted");
            if (tut == 1)
            {
                tutorialWorkflowDone = true;
                tutorialCompleted = true;

                if(mapMode == MapMode.MapModeMapping){
                    addButton.SetActive(true);
                }

            } else {
                tutorial.StartTutorial(mapMode);
            }
        } else {
            tutorial.StartTutorial(mapMode);
        }

        //TODO: In start this produces bug on reloaded ARScene
        UnityARSessionNativeInterface.ARFrameUpdatedEvent += ARFrameUpdated;
    }

    public void BackButton()
    {
        switch (mapMode)
        {
            case MapMode.MapModeMapping:
                switch (tutorialWorkflowDone)
                {
                    case false:
                        exitPanelText.text = "You still have not completed the tutorial. Are you sure you want to quit?";
                        exitPanel.SetActive(true);
                        break;

                    case true:
                        switch (progress)
                        {
                            case 0:
                                exitPanelText.text = "You still need to scan the space to save your scene. Are you sure you want to quit?";
                                exitPanel.SetActive(true);
                                break;

                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                float reloadOdds = (Random.Range((float)progress, (float)(progress + 1)) / 5f * 100f);
                                exitPanelText.text = "For best results you should keep scanning your around your area. Try walking around. Right now you only have a " + reloadOdds.ToString() + "% chance of reloading your scene.";
                                exitPanel.SetActive(true);
                                break;
                            case 5:
                                LeaveScene();
                                break;
                        }
                        break;
                }
                break;

            case MapMode.MapModeLocalization:
                switch (tutorialWorkflowDone)
                {
                    case false:
                        exitPanelText.text = "You still need to scan the space to save your scene. Are you sure you want to quit?";
                        exitPanel.SetActive(true);
                        break;

                    case true:
                        switch (objectReloaded)
                        {
                            case false:
                                switch (progress)
                                {
                                    case 0:
                                    case 1:
                                    case 2:
                                    case 3:
                                        exitPanelText.text = "Your scene hasn't reloaded yet. Try scanning your floor, walls, or the objects around you to get your bar green.";
                                        exitPanel.SetActive(true);
                                        break;

                                    case 4:
                                    case 5:
                                        exitPanelText.text = "Hmm, that's strange. It's possible that you are not in the same location that you placed your scene in, or your original scanning was not completed. Please report this to us at feedback@jidomaps.com";
                                        exitPanel.SetActive(true);
                                        break;
                                }
                                break;

                            case true:
                                switch (progress)
                                {
                                    case 0:
                                    case 1:
                                    case 2:
                                    case 3:
                                    case 4:
                                        exitPanelText.text = "Your result will continue to improve as your bar fills up, are you sure you want to exit?";
                                        exitPanel.SetActive(true);
                                        break;
                                    case 5:
                                        LeaveScene();
                                        break;
                                }
                                break;
                        }
                        break;
                }
                break;
        }
    }

    public void CloseExitPanel(){
        exitPanel.SetActive(false);
    }

    public void ToggleHelpPanel(){
        helpPanel.SetActive(!helpPanel.activeSelf);
        tutorial.gameObject.SetActive(!helpPanel.activeSelf);
    }

    public void SendFeedback(){

        string email = "info@jidomaps.com";
        string subject = MyEscapeURL("Feedback for Persistence Demo");
        string body = MyEscapeURL("I just ran the tutorial using map ID " +GetComponent<SceneControl>().mapID + ". My feedback is...");
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);

        helpPanel.SetActive(false);
    }

    public void RestartTutorial(){
        PlayerPrefs.SetInt("TutorialCompleted", 0);
        SceneManager.LoadSceneAsync("Tutorial");

    }

    public void ObjectPlaced(){
        placedObject = true;
        tutorial.placedObjectFlag = true;
    }

    public void CompleteTutorial()
    {
        tutorialWorkflowDone = true;
        tipTimer = gameObject.AddComponent<Timer>();
        tipTimer.SetAlarm(10f, true);
        tipTimer.alarm += GiveTip;
        print(tipTimer.TimeElapsed);

    }

    public void IncrementProgress(int progress){
        this.progress = progress;
        switch(mapMode){
            case MapMode.MapModeMapping:
                if (!tutorialWorkflowDone)
                    return;
                switch (progress)
                {
                    case 3:
                        Toast("Great job so far, keep scanning to ensure you can reload your scene every time!", 4.0f);
                        break;

                    case 4:
                        Toast("Just a little more and your scan will go from Good to Great!", 4.0f);
                        break;

                    case 5:
                        Toast("You did it! When you are ready, press the arrow below to learn how you can reload your scene.", 4.0f);
                        break;
                }
                break;

            case MapMode.MapModeLocalization:
                break;
                
        }

        tipTimer.ResetTimer();
    }

    string[] ScanTipsMapping = {"Try scanning areas where there are objects or textured surfaces.",
                         "You're doing great so far, try moving around your space more and scanning a new area.", 
        "Try moving around your space more and scanning a new area.",
        "Scanning a table or patterned wall in your area could improve things.",
        "You can improve your scan by slowly turning around."};

    string[] ScanTipsLocalization = {"Pretty good. Make sure to scan the area where you placed your original scene.",
        "You're doing great so far, show your phone the original objects or structures it saw the first time.",
                         "Try moving around and scan a different part of your area.",
        "Scanning a table or patterned wall in your area could improve things.",
                         "You can improve your scan by slowly turning around."};

    public void GiveTip(){
        switch(mapMode){
            case MapMode.MapModeMapping:
                if (!tutorialWorkflowDone)
                    return;
                if(tipCount == 5){
                    switch(progress){
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                            Toast("It seems like you are having some difficulty scanning. We might be able to reload it anyways, let's give it a try!", 4.0f);
                            Invoke("LeaveScene", 4.0f);
                            break;

                        case 4:
                        case 5:
                            Toast("You've done a great job! Let's go reload your scene.", 4.0f);
                            Invoke("LeaveScene",4.0f);
                            break;
                    }
                } else {
                    switch (progress)
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                            Toast(ScanTipsMapping[tipCount], 3.0f);
                            break;
                    }
                    tipCount++;   
                }
                break;

            case MapMode.MapModeLocalization:
                if (!tutorialWorkflowDone || objectReloaded)
                    return;

                if (tipCount == 5)
                {
                    switch (progress)
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                            Toast("It seems like you are having some difficulty scanning. Let's try this again.", 4.0f);
                            Invoke("LeaveScene", 4.0f);
                            break;

                        case 4:
                        case 5:
                            Toast("Hmm, we would have expected a result by now. Are you sure you are scanning the same space? Let's go back and try again.", 4.0f);
                            Invoke("LeaveScene", 4.0f);
                            break;
                    }
                } else {
                    switch (progress)
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                            Toast(ScanTipsLocalization[tipCount], 4.0f);
                            break;
                    }
                    tipCount++;   
                }
                break;

        }

    }

    public void LeaveScene(){
        switch(mapMode){
            case MapMode.MapModeMapping:
                switch(tutorialCompleted){
                    case false:
                        mapSession.Dispose();
                        SceneManager.LoadSceneAsync("MidTutorial");
                        break;

                    case true:
                        mapSession.Dispose();
                        SceneManager.LoadSceneAsync("MainScene");
                        break;
                }
                break;


            case MapMode.MapModeLocalization:
                switch (objectReloaded)
                {
                    case false:
                        mapSession.Dispose();
                        SceneManager.LoadSceneAsync("MidTutorial");
                        break;

                    case true:
                        mapSession.Dispose();
                        SceneManager.LoadSceneAsync("MainScene");
                        break;
                }
                break;
        }
    }

    private void ARFrameUpdated(UnityARCamera cam)
   {
        //TODO: be smarter about this
        if(!tutorialWorkflowDone){
            return;
        }

        if (cam.trackingState == ARTrackingState.ARTrackingStateLimited) {
            print("AR Tracking limited");
            Toast("AR Tracking is limited. You need to be in a well lit area and minimize shaking your phone.", 4.0f);
       }
   }

    public void Toast(string message, float time)
    {
        notification.text = message;
        notificationPanel.SetActive(true);
        Invoke("ToastOff", time);
    }

    private void ToastOff()
    {
        notificationPanel.SetActive(false);
    }

    string MyEscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }

    private void OnDisable()
    {
        UnityARSessionNativeInterface.ARFrameUpdatedEvent -= ARFrameUpdated;
    }

}
