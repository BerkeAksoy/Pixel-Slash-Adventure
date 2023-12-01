using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCounter : MonoBehaviour
{
    private static TimeCounter instance;

    public static TimeCounter Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("TimeCounter is null.");
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private float elapsedTime;
    private TimeSpan playTime;
    private bool timerOn;
    private string timeText;

    private void Start()
    {
        StartTimer();
    }

    private void Update()
    {
        if (timerOn)
        {
            elapsedTime += Time.deltaTime;
        }
        else
        {
            StopTimer();
        }
    }

    public void StartTimer()
    {
        timerOn = true;
        elapsedTime = 0;
    }

    public void StopTimer()
    {
        if (timerOn)
        {
            timerOn = false;
            playTime = TimeSpan.FromSeconds(elapsedTime);
            timeText = "Time: " + playTime.ToString("mm':'ss'.'ff");
            Debug.Log(timeText);
        }
    }
}
