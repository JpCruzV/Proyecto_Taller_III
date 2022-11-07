using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    [SerializeField] Text timeText;
    [SerializeField] float timeRemaining = 10;

    public bool timerIsRunning = false;


    private void Start() {

        timerIsRunning = true;
    }


    void Update() {

        DisplayTime(timeRemaining);

        if (timerIsRunning) {

            if (timeRemaining > 0) {

                timeRemaining -= Time.deltaTime;
            }
            else {

                timeRemaining = 0;
                timerIsRunning = false;
            }
        }
    }


    void DisplayTime(float timeToDisplay) {

        float min = Mathf.FloorToInt(timeToDisplay / 60);
        float sec = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = string.Format("{0:00}:{1:00}", min, sec);
    }
}
