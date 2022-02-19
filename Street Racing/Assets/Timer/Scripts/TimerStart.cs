using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerStart : MonoBehaviour
{
    public float timeStart = 3;
    public Text timeText;
    bool timerRunning = true;
    [SerializeField] private GameObject timerStart;
    
    void Start()
    {
        timeText.text = timeStart.ToString();
    }

    void Update()
    {
        if(timerRunning == true)
        {
            timeStart -= Time.deltaTime;
            timeText.text = Mathf.Round(timeStart).ToString();
        }

        if(timeStart < 1)
        {
            timeStart = 1;
            timeText.text = "Start!";
            Invoke("HidePanel", 1f);
        }
    }

    void HidePanel()
    {
        timerStart.SetActive(false);
    }
}
