using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReloadSceneManager : MonoBehaviour {

    public GameObject ReloadNewPanel;
    public GameObject ReloadOldPanel;
    public GameObject TutorialPanel;
    public GameObject FeedbackPanel;
    public GameObject DimmerPanel;
    private Animator _dimmerAnimator;
    public Text sceneIDText;

    public InputField MapIDInput;
    private bool IsMappingMode;

    private const string UserIDKey = "UserID";
    private const string MapIDKey = "MapID";
    private string defaultUserID = "demoUser";
    private string mapID = "";

    private const string characters = "1234567890";

    void Start()
    {
        _dimmerAnimator = DimmerPanel.GetComponent<Animator>();

        if (PlayerPrefs.HasKey(MapIDKey))
        {
            mapID = PlayerPrefs.GetString(MapIDKey);
            sceneIDText.text = mapID;
        }

        if (PlayerPrefs.HasKey("TutorialCompleted"))
        {
            int tutorialCompleted = PlayerPrefs.GetInt("TutorialCompleted");
            if (tutorialCompleted == 0)
            {
                FeedbackPanel.SetActive(true);
                ShowDimmer();
            }
        } else {
            FeedbackPanel.SetActive(true);
            ShowDimmer();
        }

    }

    public void StartNew(){
        mapID = GenerateMapID(4, "");
        IsMappingMode = true;
        LoadNextScene();
    }

    public void Reload()
    {
        if (string.IsNullOrEmpty(mapID) || string.IsNullOrEmpty(defaultUserID))
        {
            return;
        }

        IsMappingMode = false;
        LoadNextScene();
    }

    public void ReloadDifferent()
    {
        if (string.IsNullOrEmpty(MapIDInput.text) || string.IsNullOrEmpty(defaultUserID))
        {
            return;
        }

        mapID = MapIDInput.text;
        IsMappingMode = false;
        LoadNextScene();
    }

    public void ShowReloadPanel()
    {
        if(mapID != ""){
            ReloadOldPanel.SetActive(true);
        } else {
            ReloadNewPanel.SetActive(true);
        }

        ShowDimmer();
    }

    public void ShowReloadNewPanel(){
        ReloadOldPanel.SetActive(false);
        ReloadNewPanel.SetActive(true);

        ShowDimmer();
    }

    public void ToggleTutorialPanel(){
        TutorialPanel.SetActive(!TutorialPanel.activeSelf);   
    }

    public void SendFeedback(){

        string email = "info@jidomaps.com";
        string subject = MyEscapeURL("Feedback for Persistence Demo");
        string body = MyEscapeURL("I just ran the tutorial using map ID " + mapID + ". My feedback is...");
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);

        PlayerPrefs.SetInt("TutorialCompleted", 1);
        FeedbackPanel.SetActive(false);
        HideDimmer();
    }

    public void EndTutorial(){
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        FeedbackPanel.SetActive(false);
        HideDimmer();
    }

    private void LoadNextScene()
    {
        PlayerPrefs.SetInt("IsMappingMode", IsMappingMode ? 1 : 0);
        PlayerPrefs.SetString(MapIDKey, mapID);
        PlayerPrefs.SetString(UserIDKey, defaultUserID);

        SceneManager.LoadSceneAsync("ARScene");
    }

    public void LoadTutorial(){
        PlayerPrefs.SetInt("TutorialCompleted", 0);
        SceneManager.LoadSceneAsync("Tutorial");
    }

    private string GenerateMapID(int length, string ID)
    {
        if (ID.Length < length)
        {
            return (ID + characters[Random.Range(0, characters.Length - 1)] + GenerateMapID(length - 1, ID));
        }
        else
        {
            return "";
        }

    }

    string MyEscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }

    private void ShowDimmer ()
    {
        if(_dimmerAnimator)
            _dimmerAnimator.SetTrigger("FadeIn");
    }

    private void HideDimmer ()
    {
        if(_dimmerAnimator)
            _dimmerAnimator.SetTrigger("FadeOut");
    }
}
