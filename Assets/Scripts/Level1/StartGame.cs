using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    #region Public Fields
    public static StartGame Instance;
    public float startDelay = 3f, totalTime = 120f, gameClock;
    #endregion

    #region Serialized Fields
    [SerializeField] private TextMeshProUGUI timerText, gameClockText;
    [SerializeField] private GameObject startPrompt;
    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private Controller controller;
    [SerializeField] private InteractController interactController;
    [SerializeField] private GameObject pauseMenuObject;
    #endregion

    #region Private Fields
    public float timeElapsed;
    private GlobalControls controls;
    public bool isPaused = false;
    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        // Initialize input controls
        controls = new GlobalControls();
        controls.Pause.PauseAction.started += ctx => Pause(ctx);

        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Initialize game clock and UI elements
        gameClock = totalTime;
        timeElapsed = startDelay;
        gameClockText.text = totalTime.ToString();
    }

    private void Update()
    {
        // Handle start delay countdown
        if (timeElapsed > 0)
        {
            timeElapsed -= Time.deltaTime;
            timerText.text = Mathf.Ceil(timeElapsed).ToString();
            fadeGroup.alpha = Mathf.Sqrt(Mathf.Sqrt(timeElapsed / startDelay));
            controller.canMove = false;
        }
        else
        {
            // Enable player movement and disable UI elements after countdown
            timerText.gameObject.SetActive(false);
            startPrompt.SetActive(false);
            fadeGroup.gameObject.SetActive(false);
            controller.canMove = true;

            // Handle game timer countdown
            if (gameClock > 0)
            {
                gameClock -= Time.deltaTime;
                gameClockText.text = Mathf.Ceil(gameClock).ToString();
            }

            // Change timer text color when close to expiration
            if (gameClock <= 15)
            {
                gameClockText.color = Color.red;
            }

            // End game when timer reaches zero
            if (gameClock <= 0)
            {
                interactController.canInteract = false;
                EndGame.Instance.GameEnd(0, interactController.points, "-1");
            }
        }
    }
    #endregion

    #region Pause and Resume Methods
    // Toggle pause menu and game state
    private void Pause(InputAction.CallbackContext ctx)
    {
        if (gameClock > 0 && timeElapsed <= 0 && !EndGame.Instance.gameEnded)
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

    // Resume game from pause menu
    public void Resume()
    {
        pauseMenuObject.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    #endregion

    #region Scene Management
    // Return to the main menu
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        isPaused = false;
        Time.timeScale = 1;
    }

    // Quit the application
    public void Quit()
    {
        Application.Quit();
    }
    #endregion

    #region Input Handling
    private void OnEnable()
    {
        controls.Pause.Enable();
    }

    private void OnDisable()
    {
        controls.Pause.Disable();
    }
    #endregion
}
