using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{

    public GameObject ReloadPanel;

    public InputField MapIDInput;
    private bool IsMappingMode;

    private const string UserIDKey = "UserID";
    private const string MapIDKey = "MapID";
    private string defaultUserID = "demoUser";
    private string mapID = "";

    private const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

    void Start()
    {
        //Generate unique map ID
        mapID = GenerateMapID(3, mapID);
        print(mapID);
    }

    public void StartNew()
    {
        if (string.IsNullOrEmpty(mapID))
        {
            return;
        }

        IsMappingMode = true;
        LoadNextScene();
    }

    public void Reload()
    {
        if (string.IsNullOrEmpty(MapIDInput.text) || string.IsNullOrEmpty(defaultUserID))
        {
            return;
        }

        mapID = MapIDInput.text;
        IsMappingMode = false;
        LoadNextScene();
    }

    public void ToggleReload(){
        ReloadPanel.SetActive(!ReloadPanel.activeSelf);
    }

    private void LoadNextScene()
    {
        PlayerPrefs.SetInt("IsMappingMode", IsMappingMode ? 1 : 0);
        PlayerPrefs.SetString(MapIDKey, mapID);
        PlayerPrefs.SetString(UserIDKey, defaultUserID);

        SceneManager.LoadSceneAsync("ARScene");
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
