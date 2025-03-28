using TMPro;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    #region Fields and Singleton Instance
    [SerializeField] GameObject endPanel;

    public static EndGame Instance;
    public bool gameEnded = false;
    #endregion

    private void Awake()
    {
        // Implement singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        gameEnded = false;
    }

    #region Public Methods
    // Trigger the end game sequence with different messages based on the state and points scored
    public void GameEnd(int state, int pointsScored, string time)
    {
        if (!gameEnded)
        {
            // State 0: Successful completion with varying ending messages
            if (state == 0)
            {
                endPanel.SetActive(true);
                // Display points collected
                if (time != "-1")
                    endPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "You collected " + pointsScored + " out of 18 plasma cubes in " + time + " seconds.";
                else
                    endPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "You collected " + pointsScored + " out of 18 plasma cubes.";

                TextMeshProUGUI ending = endPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

                if (pointsScored == 18)
                {
                    ending.text = "You're a hero. You've been immortalized.";
                }
                else if (pointsScored <= 17 && pointsScored > 12)
                {
                    ending.text = "That should be enough to get you home. Are you excited to see your fellow XRs again?";
                }
                else if (pointsScored <= 12 && pointsScored > 7)
                {
                    ending.text = "You somehow salvaged enough fuel to just reach home. But at what cost?";
                }
                else if (pointsScored <= 7 && pointsScored > 4)
                {
                    ending.text = "You will be able to survive for a few more months with the fuel you collected.";
                }
                else
                {
                    ending.text = "My battery is low and it's getting dark.";
                }
                gameEnded = true;
            }
            // State 1: Suit out of signal
            else if (state == 1)
            {
                endPanel.SetActive(true);
                endPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Your suit is out of signal. Gravity switch shutting down.";
                endPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Do you think humans will ever find your cold metal body?";
                gameEnded = true;
            }
        }
    }
    #endregion
}
