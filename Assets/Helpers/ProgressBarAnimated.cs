using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarAnimated : MonoBehaviour {


    private float maxWidth = 1;
    private Color32[] colors = { new Color32(230, 80, 72, 255), new Color32(253, 112, 63, 255), new Color32(253, 166, 63, 255), new Color32(243, 238, 63, 255), new Color32(132, 241, 110, 255) };
    public GameObject progressBar;
    private Image _progressBarImage;
    //public Text progressText;

    public List<Image> ProgressionMarkers = new List<Image>();

    public float DebugProgress;
    private float _targetFillAmount;

    private void Start()
    {
        _progressBarImage = progressBar.GetComponent<Image>();
    }

    public void AddProgress(int progress){
        progressBar.GetComponent<Image>().color = colors[progress-1];
        float xSize = (float)progress * maxWidth / 5.0f;
        progressBar.GetComponent<RectTransform>().sizeDelta = new Vector2(xSize, progressBar.GetComponent<RectTransform>().sizeDelta.y);

        CheckForThreshold(0);
    }

    private void Update()
    {
        //Update the progress bar
        CheckForThreshold((int)(DebugProgress));

    }

    private void CheckForThreshold (int currentProgress)
    {
        _progressBarImage.fillAmount = Mathf.Lerp(_progressBarImage.fillAmount, _targetFillAmount, 5.0f * Time.deltaTime);

        if (currentProgress == 1)
        {
            _targetFillAmount = 0.235f;

            if (_progressBarImage.fillAmount > _targetFillAmount * 0.95f)
            {
                ProgressionMarkers[0].gameObject.SetActive(true);
                ProgressionMarkers[0].color = colors[1];
            }
            //_progressBarImage.color = colors[1];
        }
        else if (currentProgress == 2)
        {
            _targetFillAmount = 0.5f;

            if (_progressBarImage.fillAmount > _targetFillAmount * 0.95f)
            {
                ProgressionMarkers[1].gameObject.SetActive(true);
                ProgressionMarkers[1].color = colors[2];
            }
            //_progressBarImage.color = colors[2];
        }
        else if (currentProgress == 3)
        {
            _targetFillAmount = 0.75f;

            if (_progressBarImage.fillAmount > _targetFillAmount * 0.95f)
            {
                ProgressionMarkers[2].gameObject.SetActive(true);
                ProgressionMarkers[2].color = colors[3];
            }
            //_progressBarImage.color = colors[3];
        }
        else if(currentProgress == 4)
        {
            _targetFillAmount = 1.0f;

            if (_progressBarImage.fillAmount > _targetFillAmount * 0.95f)
            {
                ProgressionMarkers[3].gameObject.SetActive(true);
                ProgressionMarkers[3].color = colors[4];
            }
            //_progressBarImage.color = colors[4];
        }
    }
}
