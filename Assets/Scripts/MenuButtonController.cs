using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtonController : MonoBehaviour
{
    const string KEY_IS_VISIBLE = "IsVisible";
    const string KEY_ENDED = "Ended";

    public static Action<int> ButtonClicked;

    [SerializeField] private Animator animator;
    [SerializeField] private Button button;

    [HideInInspector] public int Index;

    public void SetInteractable(bool needInteractable)
    {
        button.interactable = needInteractable;
    }

    public void SetVisible(bool needVisible)
    {
        animator.SetBool(KEY_IS_VISIBLE, needVisible);
    }

    public void Click()
    {
        if (ButtonClicked != null)
            ButtonClicked(Index);
    }

    public void DoAnimationOnTimeEnd()
    {
        animator.SetTrigger(KEY_ENDED);
    }
}