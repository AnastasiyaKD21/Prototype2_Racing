using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float timeStart = 60;
    public Text timerText;
    bool timerRunning = true;
    [SerializeField] private GameObject losePanel;

    void Start()
    {
        timerText.text = ": " + timeStart.ToString();
    }

    void Update()
    {
        if(timerRunning == true)
        {
            timeStart -= Time.deltaTime;
            timerText.text = Mathf.Round(timeStart).ToString();
        }

        if(timeStart < 0)
        {
            timeStart = 0;
            losePanel.SetActive(true);
            Time.timeScale = 0;  
        }
    }
}
