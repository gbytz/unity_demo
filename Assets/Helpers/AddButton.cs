using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddButton : MonoBehaviour {

    public Sprite[] arrows;
    public GameObject addPanel;

    public void TogglePanel(){
        addPanel.SetActive(!(addPanel.activeSelf));
        int active = (addPanel.activeSelf) ? 1 : 0;
        GetComponent<Image>().sprite = arrows[active];
    }
}
