using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSetUIManager : MonoBehaviour
{
    public static Action<bool, float> CurrentTimerChangeValuePressed;
    public static Action CurrentTimerChangeStateClicked;

    [SerializeField] private MenuUIManager menuUIManager;
    [SerializeField] private TimerManager timerManager;
    [SerializeField] private GameObject canvasObject;
    [SerializeField] private Text timerVelueText;
    [SerializeField] private Text actionText;
    [SerializeField] private List<Image> buttonImages = new List<Image>();
    [SerializeField] private float buttonPressDelay;
    [Header("Animation")]
    [SerializeField] private RectTransform formRT;
    [SerializeField] private AnimationCurve xPositionCurve;
    [SerializeField] private float formOffset;

    private Coroutine animationCoroutine;
    private Coroutine changeTimerPlusCoroutine;
    private Coroutine changeTimerMinusCoroutine;

    private void OnEnable()
    {
        TimerManager.CurrentTimerUpdated += OnCurrentTimerUpdated;
    }

    private void OnDisable()
    {
        TimerManager.CurrentTimerUpdated -= OnCurrentTimerUpdated;
    }

    private void OnCurrentTimerUpdated(TimerData timerData)
    {
        timerVelueText.text = TimerManager.TimeSpanToString(timerData.TimeSpan);

        if (timerData.IsEnabled)
        {
            actionText.text = "пауза";
        }
        else
        {
            actionText.text = "старт";
        }
    }

    private IEnumerator AnimationTimerSettingsRoutine(bool needShow)
    {
        for (int i = 0; i < buttonImages.Count; i++)
        {
            buttonImages[i].raycastTarget = false;
        }

        if (needShow)
        {
            OnCurrentTimerUpdated(timerManager.GetCurrentTimerData());
            formRT.anchoredPosition = new Vector2(xPositionCurve.Evaluate(xPositionCurve[xPositionCurve.length - 1].time) * formOffset, formRT.anchoredPosition.y);
            canvasObject.SetActive(true);
        }

        float duration = xPositionCurve[xPositionCurve.length - 1].time;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float time = needShow ? t : duration - t;
            formRT.anchoredPosition = new Vector2(xPositionCurve.Evaluate(time) * formOffset, formRT.anchoredPosition.y);
            yield return null;
        }

        for (int i = 0; i < buttonImages.Count; i++)
        {
            buttonImages[i].raycastTarget = true;
        }

        animationCoroutine = null;

        if (!needShow)
        {
            canvasObject.SetActive(false);

            menuUIManager.ShowMenu();
        }
    }

    private IEnumerator ChangeTimerRoutine(bool isPositive)
    {
        float t = 0;
        while (true)
        {
            if (CurrentTimerChangeValuePressed != null)
            {
                CurrentTimerChangeValuePressed(isPositive, t);
            }
            t += buttonPressDelay;
            yield return new WaitForSeconds(buttonPressDelay);
        }
    }

    public void ClickAction()
    {
        if (CurrentTimerChangeStateClicked != null)
        {
            CurrentTimerChangeStateClicked();
        }
    }

    public void ShowTimerSettings()
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(AnimationTimerSettingsRoutine(true));
    }

    public void HideTimerSettings()
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(AnimationTimerSettingsRoutine(false));
    }

    public void ButtonMinusDown()
    {
        if (changeTimerPlusCoroutine != null)
        {
            StopCoroutine(changeTimerPlusCoroutine);
            changeTimerPlusCoroutine = null;
        }

        if (changeTimerMinusCoroutine != null)
        {
            StopCoroutine(changeTimerMinusCoroutine);
        }

        changeTimerMinusCoroutine = StartCoroutine(ChangeTimerRoutine(false));
    }

    public void ButtonMinusUp()
    {
        if (changeTimerMinusCoroutine != null)
        {
            StopCoroutine(changeTimerMinusCoroutine);
            changeTimerMinusCoroutine = null;
        }
    }

    public void ButtonPlusDown()
    {
        if (changeTimerMinusCoroutine != null)
        {
            StopCoroutine(changeTimerMinusCoroutine);
            changeTimerMinusCoroutine = null;
        }

        if (changeTimerPlusCoroutine != null)
        {
            StopCoroutine(changeTimerPlusCoroutine);
        }

        changeTimerPlusCoroutine = StartCoroutine(ChangeTimerRoutine(true));
    }

    public void ButtonPlusUp()
    {
        if (changeTimerPlusCoroutine != null)
        {
            StopCoroutine(changeTimerPlusCoroutine);
            changeTimerPlusCoroutine = null;
        }
    }
}