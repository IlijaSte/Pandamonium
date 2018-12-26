using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    private static Timer instance;

    public static Timer I
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Timer>();
            }
            return instance;
        }
    }

    public Action onFinished;

    private Coroutine activeTimer;

    private IEnumerator Countdown(int seconds)
    {

        while(seconds > 0)
        {

            GetComponent<Text>().text = (seconds / 60).ToString() + " : " + (seconds % 60).ToString();
            seconds--;

            yield return new WaitForSeconds(1);
        }

        if(onFinished != null)
        {
            onFinished();
        }

        GetComponent<Text>().enabled = false;
    }
    
    public void Stop()
    {
        if(activeTimer != null)
            StopCoroutine(activeTimer);

        GetComponent<Text>().enabled = false;
    }

    public void StartCountdown(int seconds)
    {
        GetComponent<Text>().enabled = true;

        if (activeTimer != null)
            Stop();

        activeTimer = StartCoroutine(Countdown(seconds));
    }

}
