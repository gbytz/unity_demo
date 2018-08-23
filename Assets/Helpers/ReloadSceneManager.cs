using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReloadSceneManager : MonoBehaviour {

    public GameObject ReloadNewPanel;
    public GameObject ReloadOldPanel;
    public GameObject TutorialPanel;
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
        if (PlayerPrefs.HasKey(MapIDKey))
        {
            mapID = PlayerPrefs.GetString(MapIDKey);
            sceneIDText.text = mapID;
        }
    }

    public void StartNew(){
        mapID = GenerateMapID(4, mapID);
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

    }

    public void ShowReloadNewPanel(){
        ReloadOldPanel.SetActive(false);
        ReloadNewPanel.SetActive(true);
    }

    public void ShowTutorialPanel(){
        TutorialPanel.SetActive(true);   
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
}
