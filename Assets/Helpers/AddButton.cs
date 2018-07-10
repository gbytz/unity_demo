using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddButton : MonoBehaviour {

    public Sprite[] arrows;
    public GameObject addPanel;

    public void TogglePanel(){
        int active = (addPanel.activeSelf) ? 1 : 0;
        print("toglle " + active);
        addPanel.SetActive(!(addPanel.activeSelf));
        GetComponent<Image>().sprite = arrows[active];
    }
}
