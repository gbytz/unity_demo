using System.Collections;
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
                progressText.text = "POOR";
                progressText.color = Color.red;
                break;

            case 2:
                progressText.text = "INSUFFICIENT";
                progressText.color = new Color(244f/ 256f, 143f/ 256f, 66f/ 256f, 1f);
                break;

            case 3:
                progressText.text = "NOT THERE YET";
                progressText.color = Color.yellow;
                break;

            case 4:
                progressText.text = "OKAY";
                progressText.color = new Color(166f/256f, 244f/ 256f, 65f/ 256f, 1f);
                break;

            case 5:
                progressText.text = "GREAT";
                progressText.color = Color.green;
                break;
        }
    }

}
