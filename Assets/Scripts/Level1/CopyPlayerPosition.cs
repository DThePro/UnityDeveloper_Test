using UnityEngine;

public class CopyPlayerPosition : MonoBehaviour
{
    [SerializeField] private GameObject playerObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = playerObject.transform.position;
    }
}
