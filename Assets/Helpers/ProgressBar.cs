﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {


    private float maxWidth = 800;
    private Color32[] colors = { new Color32(255, 0, 0, 255), new Color32(255, 183, 0, 255), new Color32(249, 255, 0, 255), new Color32(249, 255, 0, 255), new Color32(42, 255, 0, 255) };
    public GameObject progressBar;
    public Text progressText;

    public void AddProgress(int progress){
        progressBar.GetComponent<Image>().color = colors[progress-1];
        float xSize = (float)progress * maxWidth / 5.0f;
        progressBar.GetComponent<RectTransform>().sizeDelta = new Vector2(xSize, progressBar.GetComponent<RectTransform>().sizeDelta.y);
        UpdateProgressText(progress);
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
