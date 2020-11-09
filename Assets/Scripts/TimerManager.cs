using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    const string KEY_SAVE_DATE = "Date";
    const string KEY_SAVE_ENABLED = "Is enabled";

    public static Action<int> TimerEnded;
    public static Action<TimerData> CurrentTimerUpdated;

    [SerializeField] private TimeSetUIManager timeSetUIManager;
    [SerializeField] private MenuUIManager menuUIManager;
    [SerializeField] private AnimationCurve addSecondsCurve;
    [SerializeField] private int timerCount = 3;

    private List<TimerData> timerDatas = new List<TimerData>();

    private int currentTimerIndex = -1;

    private void OnEnable()
    {
        Application.targetFrameRate = 60;

        MenuUIManager.TimerSetStart += OnTimerSetStart;
        TimeSetUIManager.CurrentTimerChangeValuePressed += OnCurrentTimerChangeValuePressed;
        TimeSetUIManager.CurrentTimerChangeStateClicked += OnCurrentTimerChangeStateClicked;
    }

    private void OnDisable()
    {
        MenuUIManager.TimerSetStart -= OnTimerSetStart;
        TimeSetUIManager.CurrentTimerChangeValuePressed -= OnCurrentTimerChangeValuePressed;
        TimeSetUIManager.CurrentTimerChangeStateClicked -= OnCurrentTimerChangeStateClicked;
    }

    private void Start()
    {
        Initiate();
    }

    private void Update()
    {
        int dt = Mathf.FloorToInt(Time.unscaledDeltaTime * 1000);
        for (int i = 0; i < timerDatas.Count; i++)
        {
            if (timerDatas[i].IsEnabled)
            {
                timerDatas[i].TimeSpan -= new TimeSpan(0, 0, 0, 0, dt);

                if (timerDatas[i].TimeSpan.Ticks <= 0)
                {
                    timerDatas[i].IsEnabled = false;
                    timerDatas[i].TimeSpan = new TimeSpan(0);

                    if (TimerEnded != null)
                    {
                        TimerEnded(i);
                    }
                }

                if (currentTimerIndex >= 0 && currentTimerIndex == i)
                {
                    if (CurrentTimerUpdated != null)
                    {
                        CurrentTimerUpdated(timerDatas[currentTimerIndex]);
                    }
                }
            }
        }
    }

    private void Initiate()
    {
        for (int i = 0; i < timerCount; i++)
        {
            TimerData timerData = new TimerData(i);
            timerDatas.Add(timerData);
        }
        menuUIManager.InitiateButtons(timerCount);
    }

    private void OnTimerSetStart(int index)
    {
        currentTimerIndex = index;
    }

    private void OnCurrentTimerChangeValuePressed(bool needIncrease, float pressTime)
    {
        if (timerDatas[currentTimerIndex].IsEnabled)
        {
            timerDatas[currentTimerIndex].IsEnabled = false;
        }

        timerDatas[currentTimerIndex].TimeSpan += new TimeSpan(0, 0, (int)addSecondsCurve.Evaluate(pressTime) * (needIncrease ? 1 : -1));

        if (timerDatas[currentTimerIndex].TimeSpan.Ticks <= 0)
        {
            timerDatas[currentTimerIndex].TimeSpan = new TimeSpan(0);
        }

        if (CurrentTimerUpdated != null)
        {
            CurrentTimerUpdated(timerDatas[currentTimerIndex]);
        }
    }

    private void OnCurrentTimerChangeStateClicked()
    {
        if (timerDatas[currentTimerIndex].IsEnabled)
        {
            timerDatas[currentTimerIndex].IsEnabled = false;
        }
        else
        {
            if (timerDatas[currentTimerIndex].TimeSpan.Ticks > 0)
            {
                timerDatas[currentTimerIndex].IsEnabled = true;
            }
        }

        if (CurrentTimerUpdated != null)
        {
            CurrentTimerUpdated(timerDatas[currentTimerIndex]);
        }
    }

    public TimerData GetCurrentTimerData()
    {
        return timerDatas[currentTimerIndex];
    } 

    public static string TimeSpanToString(TimeSpan timeSpan)
    {
        return $"{timeSpan:hh}:{timeSpan:mm}:{timeSpan:ss}";
    }
}

[Serializable]
public class TimerData
{
    const string KEY_SAVE_ENABLED = "Is enabled ";
    const string KEY_SAVE_TIMER = "Timer ";

    private int _index;
    public int Index
    {
        get
        {
            return _index;
        }
    }

    private string KeySaveEnabled
    {
        get
        {
            return $"{KEY_SAVE_ENABLED}{Index}";
        }
    }

    private string KeySaveTimer
    {
        get
        {
            return $"{KEY_SAVE_TIMER}{Index}";
        }
    }

    private bool _isEnabled;
    public bool IsEnabled
    {
        get
        {
            return _isEnabled;
        }
        set
        {
            _isEnabled = value;
            PlayerPrefs.SetInt(KeySaveEnabled, value ? 1 : 0);
        }
    }

    private TimeSpan _timeSpan;
    public TimeSpan TimeSpan
    {
        get
        {
            return _timeSpan;
        }
        set
        {
            _timeSpan = value;
            PlayerPrefs.SetString(KeySaveTimer, value.Ticks.ToString());
        }
    }

    public TimerData() { }
    public TimerData(int newIndex)
    {
        _index = newIndex;
        _isEnabled = PlayerPrefs.GetInt(KeySaveEnabled, 0) == 1;

        long ticks = 0;
        string s_ticks = PlayerPrefs.GetString(KeySaveTimer, "0");
        long.TryParse(s_ticks, out ticks);
        _timeSpan = new TimeSpan(ticks);
    }
}