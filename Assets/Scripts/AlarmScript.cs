using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utils;

public class AlarmScript : MonoBehaviour
{
    public float maxValue = 1f;
    public float maxValueDuration = 5f;

    [ReadOnly]
    public float currentValue = 0f;

    public UnityEvent onAlarmConditionReady;
    public UnityEvent onAlarmEnter;
    public UnityEvent onAlarmExit;

    private Timer alarmTimer = new Timer(1f);

    private bool alarmActive = false;
    private bool alarmConditionReady = false;

    // Start is called before the first frame update
    void Start()
    {
        alarmTimer.Reset(maxValueDuration);
    }

    private void OnDestroy()
    {
        if(onAlarmExit != null)
            onAlarmExit.Invoke();
    }

    public void SetValue(float value)
    {
        currentValue = value;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentValue >= maxValue)
        {
            if(!alarmConditionReady)
            {
                if(onAlarmConditionReady != null)
                    onAlarmConditionReady.Invoke();

                Debug.Log("Alarm Condition Ready");
                alarmConditionReady = true;
            }

            if(alarmTimer.Update(Time.deltaTime))
            {
                // OnEnter
                Debug.Log("Alarm OnEnter");
                alarmActive = true;

                if(onAlarmEnter != null)
                    onAlarmEnter.Invoke();
            }
        }
        else
        {
            if(alarmActive || alarmConditionReady)
            {
                // OnExit
                Debug.Log("Alarm OnExit");
                alarmTimer.Reset();
                alarmActive = false;
                alarmConditionReady = false;

                if(onAlarmExit != null)
                    onAlarmExit.Invoke();
            }

        }
    }
}
