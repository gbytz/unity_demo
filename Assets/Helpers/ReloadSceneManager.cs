using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReloadSceneManager : MonoBehaviour {

    public GameObject ReloadPanel;
    public Text sceneIDText;

    public InputField MapIDInput;
    private bool IsMappingMode;

    private const string UserIDKey = "UserID";
    private const string MapIDKey = "MapID";
    private string defaultUserID = "demoUser";
    private string mapID = "";

    private const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

    void Start()
    {
        if (PlayerPrefs.HasKey(MapIDKey))
        {
            mapID = PlayerPrefs.GetString(MapIDKey);
            sceneIDText.text = mapID;
        }
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

    public void ToggleReload()
    {
        ReloadPanel.SetActive(!ReloadPanel.activeSelf);
    }

    private void LoadNextScene()
    {
        PlayerPrefs.SetInt("IsMappingMode", IsMappingMode ? 1 : 0);
        PlayerPrefs.SetString(MapIDKey, mapID);
        PlayerPrefs.SetString(UserIDKey, defaultUserID);

        SceneManager.LoadSceneAsync("ARScene");
    }

    public void LoadStartScene(){
        SceneManager.LoadSceneAsync("StartScene");
    }
}
