using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static bool isPaused = false;

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject victoryMenu;

    [SerializeField] HealthBar p1Health;
    [SerializeField] HealthBar p2Health;

    [SerializeField] Timer timer;

    [SerializeField] GameObject p1WonText;
    [SerializeField] GameObject p2WonText;

    bool p1Won = false;
    bool p2Won = false;

    private void Start() {

        Time.timeScale = 1f;

        pauseMenu.SetActive(false);
        victoryMenu.SetActive(false);

        p1WonText.SetActive(false);
        p2WonText.SetActive(false);
    }


    private void Update() {

        if (Input.GetKeyDown(KeyCode.Escape)) {

            if (isPaused) {

                Resume();
            }
            else {

                Pause();
            }
        }


        if (timer.timeRemaining <= 0) {

            if (p1Health.slider.value > p2Health.slider.value) {

                p1Won = true;
                Victory();
                Debug.Log("Player 1 Wins");
            }
            else if (p1Health.slider.value < p2Health.slider.value) {

                p2Won = true;
                Victory();
                Debug.Log("Player 2 Wins");
            }
            else {

                Debug.Log("Tie");
            }
        }
        else if (p1Health.slider.value <= 0) {

            p2Won = true;
            Victory();
            Debug.Log("Player 2 Wins");
        }
        else if (p2Health.slider.value <= 0) {

            p1Won = true;
            Victory();
            Debug.Log("Player 1 Wins");
        }
    }


    void Victory() {

        victoryMenu.SetActive(true);
        Time.timeScale = 0f;
        if (p1Won) {

            p1WonText.SetActive(true);
        }
        else if (p2Won) {

            p1WonText.SetActive(true);
        }
    }


    public void Resume() {

        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
    }


    void Pause() {

        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }


    public void Menu() {

        SceneManager.LoadScene("MainMenu");
    }


    public void RestartScene() {

        SceneManager.LoadScene("Level");
    }


    public void QuitGame() {

        Application.Quit();
    }
}
