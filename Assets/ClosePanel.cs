using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosePanel : MonoBehaviour {

    public GameObject thisPanel;

    public void CloseThisPanel(){
        thisPanel.SetActive(false);
    }
}
