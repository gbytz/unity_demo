using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial_Workflow : MonoBehaviour
{
    public GameObject tutorialPanel;
    public GameObject[] imagePanels;
    public GameObject surfaceTipPanel;
    public GameObject reloadTutorialPanel;
    public GameObject addButton;

    private FocusSquare focusSquare;
    private UX_Workflow ux_workflow;

    private int currentPanel = 0;
    public bool foundSurface = false;
    private bool placedObject = false;
    public bool placedObjectFlag = false;
    public int progress = 0;

    public enum Step {FindSurfacePanel, PlaceObjectPanel, CompleteScanPanel, CompletedTutorial};
    private Step currentStep = Step.CompletedTutorial;

    void Start(){
        focusSquare = GameObject.Find("FocusSquare").GetComponent<FocusSquare>();
        ux_workflow = FindObjectOfType<UX_Workflow>();
    }

    void Update()
    {
        switch(currentStep){
            case Step.FindSurfacePanel:
                 if (!foundSurface && focusSquare.SquareState == FocusSquare.FocusState.Found){
                    foundSurface = true;
                    Invoke("NextStep", 2f);
                }
                break;

            case Step.PlaceObjectPanel:
                if(!placedObject && placedObjectFlag){
                    placedObject = true;
                    Invoke("NextStep", 2f);
                }
                break;
        }

    }

    public void StartTutorial(MapMode mapMode){
        switch(mapMode){
            case MapMode.MapModeMapping:
                currentStep = Step.FindSurfacePanel;
                tutorialPanel.SetActive(true);
                imagePanels[currentPanel].SetActive(true);
                Invoke("ShowSurfaceTip", 8.0f);
                break;

            case MapMode.MapModeLocalization:
                currentStep = Step.CompleteScanPanel;
                tutorialPanel.SetActive(true);
                reloadTutorialPanel.SetActive(true);
                break;

        }
        
    }

    public void NextStep()
    {

        currentStep++;

        switch(currentStep){

            case Step.PlaceObjectPanel:
                imagePanels[currentPanel].SetActive(false);
                imagePanels[++currentPanel].SetActive(true);
                addButton.SetActive(true);
                break;

            case Step.CompleteScanPanel:
                imagePanels[currentPanel].SetActive(false);
                imagePanels[++currentPanel].SetActive(true);
                break;

            case Step.CompletedTutorial:
                imagePanels[currentPanel].SetActive(false);
                reloadTutorialPanel.SetActive(false);
                ux_workflow.CompleteTutorial();
                break;
                
        }

    }   

    public void ShowSurfaceTip(){
        if(foundSurface){
            return;
        }
        surfaceTipPanel.SetActive(true);
        Invoke("HideSurfaceTip", 4.0f);

        Invoke("ShowSurfaceTip", 12.0f);
    }

    public void HideSurfaceTip(){
        surfaceTipPanel.SetActive(false);
    }

}
