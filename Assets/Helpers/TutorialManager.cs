using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour {

    public GameObject[] imagePanels;

    private int currentImage = 0;

    public void NextStep(){
                                 
        if(currentImage == imagePanels.Length - 1 ){
            CompletedSteps();
        } else {
            imagePanels[currentImage].SetActive(false);
            imagePanels[++currentImage].SetActive(true);
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

    private void CompletedSteps(){
        SceneManager.LoadSceneAsync("StartScene");
    }

}
