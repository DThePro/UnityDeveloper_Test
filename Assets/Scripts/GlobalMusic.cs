using UnityEngine;

public class GlobalMusic : MonoBehaviour
{
    public static GlobalMusic Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
}
