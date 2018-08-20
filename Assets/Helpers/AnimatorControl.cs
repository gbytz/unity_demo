using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorControl : MonoBehaviour {

    public GameObject character;

    public void Animate(string triggerName){
        print(triggerName);
        character.GetComponentInChildren<Animator>().SetTrigger(triggerName);
    }
}
