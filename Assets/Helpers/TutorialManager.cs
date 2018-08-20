using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour {

    public GameObject[] imagePanels;

    private int currentImage = 0;

    public void NextStep(){
        imagePanels[currentImage].SetActive(false);
                                 
        currentImage++;
        if(currentImage > imagePanels.Length - 1 ){
            CompletedSteps();
        } else {
            imagePanels[currentImage].SetActive(true);
        }
    }

    public void PreviousStep()
    {
        currentImage--;
        if (currentImage > 0)
        {
            currentImage = 0;
        }
    }

    private void CompletedSteps(){
        
    }

}
