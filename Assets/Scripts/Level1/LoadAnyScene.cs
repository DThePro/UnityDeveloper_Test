using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadAnyScene : MonoBehaviour
{
    public void LoadThisScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
