using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour {

    public GameObject[] imagePanels;

    private int currentImage = 0;

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
            //CompletedSteps();
        } else {
            imagePanels[++currentImage].SetActive(true);
            Invoke("NextStep", 6.0f);
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

    public void CompletedSteps(){
        SceneManager.LoadSceneAsync("ARScene");
    }

}
