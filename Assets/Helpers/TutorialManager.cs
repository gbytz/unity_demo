using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour {

    public GameObject[] imagePanels;

    private int currentImage = 0;

    private bool IsMappingMode;

    private const string UserIDKey = "UserID";
    private const string MapIDKey = "MapID";
    private string defaultUserID = "demoUser";
    private string mapID = "";

    private const string characters = "1234567890";

    [SerializeField] private Animator _elipseAnimator = null;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("TutorialCompleted"))
        {
            int tutorialCompleted = PlayerPrefs.GetInt("TutorialCompleted");
            if (tutorialCompleted == 1)
            {
                SceneManager.LoadSceneAsync("MainScene");
            }
        }

        if (PlayerPrefs.HasKey(MapIDKey))
        {
            mapID = PlayerPrefs.GetString(MapIDKey);
        }
    }

    void Start()
    {
        Invoke("NextStep", 2.0f);
    }

    private void Update()
    {
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began){
            CancelInvoke();
            NextStep();
        }
    }

    public void NextStep(){
                                 
        if(currentImage == imagePanels.Length - 1 ){
            _elipseAnimator.SetTrigger("StopAnimating");
            //CompletedSteps();
        }
        else {
            imagePanels[++currentImage].SetActive(true);
            _elipseAnimator.SetTrigger("Animate");
            Invoke("NextStep", 5.0f);
        }
    }

    public void PreviousStep()
    {
        if (currentImage == 0)
        {
            return;
        } 

        imagePanels[currentImage].SetActive(false);
        imagePanels[--currentImage].SetActive(true);

    }

    public void StartNew()
    {
        
        mapID = GenerateMapID(4, "");

        if (string.IsNullOrEmpty(mapID) || string.IsNullOrEmpty(defaultUserID))
        {
            print("error: "+ mapID + " " + defaultUserID);
            return;
        }

        IsMappingMode = true;
        LoadNextScene();    
    }

    public void Reload()
    {
        if (string.IsNullOrEmpty(mapID) || string.IsNullOrEmpty(defaultUserID))
        {
            print(mapID);
            return;
        }

        IsMappingMode = false;
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        PlayerPrefs.SetInt("IsMappingMode", IsMappingMode ? 1 : 0);
        PlayerPrefs.SetString(MapIDKey, mapID);
        PlayerPrefs.SetString(UserIDKey, defaultUserID);

        SceneManager.LoadSceneAsync("ARScene_New");
    }

    private string GenerateMapID(int length, string ID)
    {
        if (ID.Length < length)
        {
            return (ID + characters[Random.Range(0, characters.Length - 1)] + GenerateMapID(length - 1, ID));
        }
        else
        {
            print("empty: " + ID.Length.ToString());
            return "";
        }

    }


}
