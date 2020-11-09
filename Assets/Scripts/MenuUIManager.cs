using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour
{
    public static Action<int> TimerSetStart;

    [SerializeField] private TimeSetUIManager timeSetUIManager;
    [SerializeField] private GameObject canvasObject;
    [SerializeField] private float animationDelay = 0.2f;
    [SerializeField] private float afterAnimationDelay = 1f;
    [Header("Buttons")]
    [SerializeField] private Transform buttonsListT;
    [SerializeField] private GameObject buttonsParentPrefab;

    private List<MenuButtonController> menuButtonControllers = new List<MenuButtonController>();

    private Coroutine animationCoroutine;

    private void Awake()
    {
        canvasObject.SetActive(false);
    }

    private void OnEnable()
    {
        MenuButtonController.ButtonClicked += OnMenuButtonClick;
        TimerManager.TimerEnded += OnTimerEnded;
    }

    private void OnDisable()
    {
        MenuButtonController.ButtonClicked -= OnMenuButtonClick;
        TimerManager.TimerEnded -= OnTimerEnded;
    }

    private void OnMenuButtonClick(int index)
    {
        if (TimerSetStart != null)
        {
            TimerSetStart(index);
        }
        HideMenu();
    }

    private void OnTimerEnded(int index)
    {
        menuButtonControllers[index].DoAnimationOnTimeEnd();
    }

    private IEnumerator AnimationRoutine(bool needShow)
    {
        if (needShow)
        {
            canvasObject.SetActive(true);
        }

        for (int i = 0; i < menuButtonControllers.Count; i++)
        {
            menuButtonControllers[i].SetInteractable(false);
        }

        for (int i = 0; i < menuButtonControllers.Count; i++)
        {
            menuButtonControllers[i].SetVisible(needShow);
            yield return new WaitForSeconds(animationDelay);
        }

        if (!needShow)
        {
            yield return new WaitForSeconds(afterAnimationDelay);
            canvasObject.SetActive(false);

            timeSetUIManager.ShowTimerSettings();
        }
        else
        {
            for (int i = 0; i < menuButtonControllers.Count; i++)
            {
                menuButtonControllers[i].SetInteractable(true);
            }
        }

        animationCoroutine = null;
    }

    public void InitiateButtons(int count)
    {
        for (int i = 0; i < count; i++)
        {
            MenuButtonController menuButtonController;
            if (buttonsListT.childCount >= i)
            {
                menuButtonController = buttonsListT.GetChild(i).GetComponentInChildren<MenuButtonController>();
            }
            else
            {
                GameObject buttonParent = Instantiate(buttonsParentPrefab, buttonsListT);
                menuButtonController = buttonParent.GetComponentInChildren<MenuButtonController>();
                buttonParent.GetComponentInChildren<Text>().text = $"Timer {i + 1}";
            }

            menuButtonController.Index = i;
            menuButtonControllers.Add(menuButtonController);
        }

        while(buttonsListT.childCount > count)
        {
            Destroy(buttonsListT.GetChild(buttonsListT.childCount - 1).gameObject);
        }

        ShowMenu();
    }

    public void ShowMenu()
    {
        if (animationCoroutine != null) 
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(AnimationRoutine(true));
    }

    public void HideMenu()
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(AnimationRoutine(false));
    }
}