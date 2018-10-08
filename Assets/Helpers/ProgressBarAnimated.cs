using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarAnimated : MonoBehaviour {


    private float maxWidth = 1;
    private Color32[] colors = { new Color32(255, 0, 0, 255), new Color32(255, 183, 0, 255), new Color32(243, 238, 63, 255), new Color32(243, 238, 63, 255), new Color32(42, 255, 0, 255) };
    public GameObject progressBar;
    private Image _progressBarImage;
    public Text progressText;

    public List<Image> ProgressionMarkers = new List<Image>();

    public float DebugProgress;

    private void Start()
    {
        _progressBarImage = progressBar.GetComponent<Image>();
    }

    public void AddProgress(int progress){
        progressBar.GetComponent<Image>().color = colors[progress-1];
        float xSize = (float)progress * maxWidth / 5.0f;
        progressBar.GetComponent<RectTransform>().sizeDelta = new Vector2(xSize, progressBar.GetComponent<RectTransform>().sizeDelta.y);

        UpdateProgressText(progress);

        _progressBarImage.fillAmount = DebugProgress/100;

        CheckForThreshold(_progressBarImage.fillAmount);
    }

    private void Update()
    {

        //For Testing
        _progressBarImage.fillAmount = Mathf.Clamp (DebugProgress / 100, 0, 0.98f);

        CheckForThreshold(Mathf.Clamp(DebugProgress / 100, 0, 1.0f));
    }

    private void CheckForThreshold (float currentProgress)
    {
       if (currentProgress > 0.23f && currentProgress < 0.47f)
        {
            ProgressionMarkers[0].gameObject.SetActive(true);
            ProgressionMarkers[0].color = colors[1];
            _progressBarImage.color = colors[1];
        }
        else if (currentProgress > 0.47f && currentProgress < 0.73f){

            ProgressionMarkers[1].gameObject.SetActive(true);
            ProgressionMarkers[1].color = colors[2];
            _progressBarImage.color = colors[2];
        }
        else if (currentProgress > 0.73f && currentProgress < 1.0f)
        {
            ProgressionMarkers[2].gameObject.SetActive(true);
            ProgressionMarkers[2].color = colors[3];
            _progressBarImage.color = colors[3];
        }
        else if(currentProgress >= 1.0f)
        {
            ProgressionMarkers[3].gameObject.SetActive(true);
            ProgressionMarkers[3].color = colors[4];
            _progressBarImage.color = colors[4];
        }
    }

    private void UpdateProgressText(int progress){
        switch(progress){
            case 1:
                progressText.text = "DREADFUL";
                progressText.color = colors[0];
                break;

            case 2:
                progressText.text = "APPALLING";
                progressText.color = colors[1];
                break;

            case 3:
                progressText.text = "ALMOST THERE!";
                progressText.color = colors[2];
                break;

            case 4:
                progressText.text = "GOOD";
                progressText.color = colors[3];
                break;

            case 5:
                progressText.text = "FANTASTIC!";
                progressText.color = colors[4];
                break;
        }
    }

}
