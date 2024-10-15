using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    #region PROPERITIES
    const float _hoursToDegrees = -30f, _minutesToDegrees = -6f, _secondsToDegrees = -6f;

    [SerializeField]
    Transform _hoursPivot, _minutesPivot, _secondsPivot;

    #endregion

    #region UNITY CORE
    // Start is called before the first frame update
    void Awake()
    {
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TimeSpan time = DateTime.Now.TimeOfDay;
        Debug.Log(time);
        _hoursPivot.localRotation = Quaternion.Euler(0f, 0f, _hoursToDegrees * (float)time.TotalHours);
        _minutesPivot.localRotation = Quaternion.Euler(0f, 0f, _minutesToDegrees * (float)time.TotalMinutes);
        _secondsPivot.localRotation = Quaternion.Euler(0f, 0f, _secondsToDegrees * (float)time.TotalSeconds);
    }
    #endregion

    #region MAIN

    #endregion
}
