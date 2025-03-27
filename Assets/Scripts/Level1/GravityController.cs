using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GravityController : MonoBehaviour
{
    #region Fields and References
    [SerializeField] private List<GameObject> worldObjects;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float rotationDuration = 1f;
    [SerializeField] private GameObject hologramObject;
    [SerializeField] private CanvasGroup rotationPromptPanel;

    private Vector3 currentUp = Vector3.up;
    private bool isRotating = false;
    private GlobalControls controls;
    private Vector3 rotateDirection;
    private bool rotateMode = false;
    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        // Initialize input controls and set up event callbacks
        controls = new GlobalControls();
        controls.Rotate.SelectDirection.started += ctx => SelectDirection(ctx);
        controls.Rotate.ConfirmRotate.started += ctx => ConfirmRotate(ctx);
        controls.Rotate.DisableRotate.started += ctx => DisableRotate(ctx);
    }

    private void Start()
    {
        // Initialization for Start can be added here if needed
    }

    private void Update()
    {
        // No per-frame updates required here
    }

    private void OnEnable()
    {
        controls.Rotate.Enable();
    }

    private void OnDisable()
    {
        controls.Rotate.Disable();
    }
    #endregion

    #region Helper Methods
    // Normalize the input direction and return the dominant axis direction
    private Vector3 ProcessDirection(Vector3 direction)
    {
        direction.Normalize();

        float x = Mathf.Abs(direction.x);
        float y = Mathf.Abs(direction.y);
        float z = Mathf.Abs(direction.z);

        if (x > y && x > z)
            return new Vector3(Mathf.Sign(direction.x), 0, 0); // Left or Right
        else if (y > x && y > z)
            return new Vector3(0, Mathf.Sign(direction.y), 0); // Up or Down
        else
            return new Vector3(0, 0, Mathf.Sign(direction.z)); // Forward or Backward
    }

    // Switch the gravity direction and initiate world rotation
    private void SwitchGravity(Vector3 newUp)
    {
        currentUp = newUp;
        StartCoroutine(RotateWorld(newUp));
    }

    // Rotate all world objects smoothly to match the new gravity direction
    private IEnumerator RotateWorld(Vector3 newUp)
    {
        isRotating = true;
        Quaternion startRotation = Quaternion.identity;
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, newUp);
        Dictionary<GameObject, (Vector3, Quaternion)> initialTransforms = new Dictionary<GameObject, (Vector3, Quaternion)>();

        foreach (GameObject obj in worldObjects)
        {
            initialTransforms[obj] = (obj.transform.position, obj.transform.rotation);
        }

        float elapsedTime = 0f;
        while (elapsedTime < rotationDuration)
        {
            float t = elapsedTime / rotationDuration;
            Quaternion currentRotation = Quaternion.Slerp(startRotation, targetRotation, t);
            foreach (var kvp in initialTransforms)
            {
                // Update position relative to the pivot (this object's position)
                kvp.Key.transform.position = currentRotation * (kvp.Value.Item1 - transform.position) + transform.position;
                // Update rotation of each object smoothly
                kvp.Key.transform.rotation = Quaternion.Slerp(kvp.Value.Item2, targetRotation * kvp.Value.Item2, t);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final positions and rotations are set
        foreach (var kvp in initialTransforms)
        {
            kvp.Key.transform.position = targetRotation * (kvp.Value.Item1 - transform.position) + transform.position;
            kvp.Key.transform.rotation = targetRotation * kvp.Value.Item2;
        }

        isRotating = false;
    }

    // Confirm and execute the gravity switch if not currently rotating
    private void ConfirmRotate(InputAction.CallbackContext ctx)
    {
        if (!isRotating && rotateMode)
        {
            SwitchGravity(ProcessDirection(rotateDirection));
            rotateMode = false;
        }
        hologramObject.SetActive(false);
        Debug.Log(ctx.control.name);
        StartCoroutine(FadeCanvasGroup(rotationPromptPanel, 0, 0.2f));
    }

    // Activate rotation mode and set hologram orientation based on the selected direction
    private void SelectDirection(InputAction.CallbackContext ctx)
    {
        rotateMode = true;
        hologramObject.SetActive(true);
        switch (ctx.control.name)
        {
            case "upArrow":
                rotateDirection = playerTransform.forward;
                break;
            case "downArrow":
                rotateDirection = -playerTransform.forward;
                break;
            case "leftArrow":
                rotateDirection = -playerTransform.right;
                break;
            case "rightArrow":
                rotateDirection = playerTransform.right;
                break;
        }

        // Set the hologram rotation based on the dominant direction
        if (ProcessDirection(rotateDirection) == new Vector3(1, 0, 0))
        {
            hologramObject.transform.localEulerAngles = new Vector3(0, 0, -90);
        }
        else if (ProcessDirection(rotateDirection) == new Vector3(-1, 0, 0))
        {
            hologramObject.transform.localEulerAngles = new Vector3(0, 0, 90);
        }
        else if (ProcessDirection(rotateDirection) == new Vector3(0, 0, 1))
        {
            hologramObject.transform.localEulerAngles = new Vector3(0, 90, 90);
        }
        else
        {
            hologramObject.transform.localEulerAngles = new Vector3(0, 90, -90);
        }

        Debug.Log(ProcessDirection(rotateDirection));
        StartCoroutine(FadeCanvasGroup(rotationPromptPanel, 1, 0.2f));
    }

    // Fade a CanvasGroup to the target alpha over a specified duration
    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    // Align the hologram based on the provided down vector (currently unused)
    void AlignDown(Vector3 downVector)
    {
        Vector3 forward = hologramObject.transform.forward;
        hologramObject.transform.rotation = Quaternion.LookRotation(forward, -downVector);
    }

    // Disable rotation mode and hide the hologram
    private void DisableRotate(InputAction.CallbackContext ctx)
    {
        rotateMode = false;
        hologramObject.SetActive(false);
        StartCoroutine(FadeCanvasGroup(rotationPromptPanel, 0, 0.2f));
    }
    #endregion
}
