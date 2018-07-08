using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {


    private float maxWidth = 800;
    private Color32[] colors = { new Color32(255, 0, 0, 255), new Color32(255, 183, 0, 255), new Color32(249, 255, 0, 255), new Color32(249, 255, 0, 255), new Color32(42, 255, 0, 255) };
    public GameObject progressBar;

    public void AddProgress(int progress){
        progressBar.GetComponent<Image>().color = colors[progress-1];
        float xSize = (float)progress * maxWidth / 5.0f;
        progressBar.GetComponent<RectTransform>().sizeDelta = new Vector2(xSize, progressBar.GetComponent<RectTransform>().sizeDelta.y);
    }

}
