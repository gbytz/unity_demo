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

    public enum Step {FindSurfacePanel, FindingSurface, PlaceObjectPanel, PlacingObject, CompleteScanPanel, CompletedTutorial};
    public Step currentStep = Step.FindSurfacePanel;

    void Start(){
        focusSquare = GameObject.Find("FocusSquare").GetComponent<FocusSquare>();
        ux_workflow = FindObjectOfType<UX_Workflow>();
    }

    void Update()
    {
        switch(currentStep){
            case Step.FindingSurface:
                 if (!foundSurface && focusSquare.SquareState == FocusSquare.FocusState.Found){
                    foundSurface = true;
                    Invoke("NextStep", 2f);
                }
                break;

            case Step.PlacingObject:
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
            case Step.FindingSurface:
                imagePanels[currentPanel].SetActive(false);
                Invoke("ShowSurfaceTip", 8.0f);
                break;

            case Step.PlaceObjectPanel:
                imagePanels[++currentPanel].SetActive(true);
                addButton.SetActive(true);
                addButton.GetComponent<Button>().enabled = false;
                break;

            case Step.PlacingObject:
                imagePanels[currentPanel].SetActive(false);
                addButton.GetComponent<Button>().enabled = true;
                break;

            case Step.CompleteScanPanel:
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
