using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject startCanvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    public void Play()
    {
        startCanvas.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
