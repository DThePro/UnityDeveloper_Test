using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    [SerializeField] float startDelay = 3f, totalTime = 120f;
    [SerializeField] TextMeshProUGUI timerText, gameClockText;
    
    [SerializeField] CanvasGroup fadeGroup;
    [SerializeField] Controller controller;
    [SerializeField] InteractController interactController;
    [SerializeField] GameObject pauseMenuObject;

    private float timeElapsed, gameClock;
    GlobalControls controls;
    bool isPaused = false;

    private void Awake()
    {
        controls = new GlobalControls();

        controls.Pause.PauseAction.started += ctx => Pause(ctx);
        // controls.Pause.PauseAction.canceled += ctx => Pause(ctx);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Time.timeScale = 0f;
        gameClock = totalTime;
        timeElapsed = startDelay;
        gameClockText.text = totalTime.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeElapsed > 0)
        {
            timeElapsed -= Time.deltaTime;
            timerText.text = Mathf.Ceil(timeElapsed).ToString();
            fadeGroup.alpha = Mathf.Sqrt(Mathf.Sqrt(timeElapsed / startDelay));
            controller.canMove = false;
        }
        else
        {
            timerText.gameObject.SetActive(false);
            fadeGroup.gameObject.SetActive(false);
            controller.canMove = true;

            
            if (gameClock > 0)
            {
                gameClock -= Time.deltaTime;
                gameClockText.text = Mathf.Ceil(gameClock).ToString();
                // CheckPause();
            }
            
            if (gameClock <= 15)
            {
                gameClockText.color = Color.red;
            }
            
            if (gameClock <= 0)
            {
                interactController.canInteract = false;
                // controller.canMove = false;

                EndGame.Instance.GameEnd(0, interactController.points);
            }
        }
    }

    private void Pause(InputAction.CallbackContext ctx)
    {
        if (gameClock > 0 && timeElapsed <= 0)
        {
            if (!isPaused)
            {
                pauseMenuObject.SetActive(true);
                Time.timeScale = 0;
                isPaused = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                pauseMenuObject.SetActive(false);
                Time.timeScale = 1;
                isPaused = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    public void Resume()
    {
        pauseMenuObject.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        isPaused = false;
        Time.timeScale = 1;
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void OnEnable()
    {
        controls.Pause.Enable();
    }

    private void OnDisable()
    {
        controls.Pause.Disable();
    }
}
