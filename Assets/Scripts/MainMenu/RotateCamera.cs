using Unity.Cinemachine;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    [SerializeField] float rotateSpeed = 5f;

    CinemachineOrbitalFollow mainMenuCam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainMenuCam = GetComponent<CinemachineOrbitalFollow>();
    }

    // Update is called once per frame
    void Update()
    {
        mainMenuCam.HorizontalAxis.Value += rotateSpeed * Time.deltaTime;
    }
}
