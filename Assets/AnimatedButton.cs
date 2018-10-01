using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class AnimatedButton : MonoBehaviour {

    public Animator Anim;
    public Button TargetButton;

    //public UnityEvent Down;
    public UnityEvent OnClick;

    private void OnValidate()
    {
        if(GetComponent<Animator>())
            Anim = GetComponent<Animator>();

        if (GetComponent<Button>())
            TargetButton = GetComponent<Button>();
    }

    private void Start()
    {
        TargetButton.onClick.AddListener(OnClickDown);
    }

    public void OnClickDown()
    {
        Anim.SetTrigger("Down");
        Invoke("OnClickCall", 0.04f);
    }

    private void OnClickCall ()
    {
        OnClick.Invoke();
    }
}
