using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Workflow : MonoBehaviour
{
    public GameObject tutorialPanel;
    public GameObject[] imagePanels;
    public GameObject surfaceTipPanel;
    public int currentPanel = 0;

    private FocusSquare focusSquare;

    public bool foundSurface = false;
    public bool placedObject = false;
    public int progress = 0;

    private enum Step {FindSurfacePanel, FindingSurface, PlaceObjectPanel, PlacingObject, CompleteScanPanel};
    private Step currentStep = Step.FindSurfacePanel;

    void Start(){
        focusSquare = GameObject.Find("FocusSquare").GetComponent<FocusSquare>();
    }

    private void Update()
    {
        print(currentStep.ToString());

        switch(currentStep){
            case Step.FindingSurface:
                if (focusSquare.SquareState == FocusSquare.FocusState.Found){
                    foundSurface = true;
                    NextStep();
                }
                break;

            case Step.PlacingObject:
                if(placedObject){
                    NextStep();
                }
                break;
        }

    }

    public void NextStep()
    {

        currentStep++;

        switch(currentStep){
            case Step.FindingSurface:
                imagePanels[currentPanel].SetActive(false);
                tutorialPanel.SetActive(false);
                Invoke("ShowSurfaceTip", 8.0f);
                break;

            case Step.PlaceObjectPanel:
                tutorialPanel.SetActive(true);
                imagePanels[currentPanel++].SetActive(true);
                break;

            case Step.PlacingObject:
                tutorialPanel.SetActive(false);
                imagePanels[currentPanel].SetActive(false);
                break;

            case Step.CompleteScanPanel:
                tutorialPanel.SetActive(true);
                imagePanels[currentPanel++].SetActive(true);
                break;

            default:
                CompletedSteps();
                break;
                
        }

    }   

    public void ShowSurfaceTip(){
        if(foundSurface){
            return;
        }
        tutorialPanel.SetActive(true);
        surfaceTipPanel.SetActive(true);
        Invoke("HideSurfaceTip", 4.0f);

        Invoke("ShowSurfaceTip", 12.0f);
    }

    public void HideSurfaceTip(){
        surfaceTipPanel.SetActive(false);
    }
    private void CompletedSteps(){
        
    }





}
