using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GravityController : MonoBehaviour
{
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

    private void Awake()
    {
        controls = new GlobalControls();
        controls.Rotate.SelectDirection.started += ctx => SelectDirection(ctx);
        controls.Rotate.ConfirmRotate.started += ctx => ConfirmRotate(ctx);
        controls.Rotate.DisableRotate.started += ctx => DisableRotate(ctx);
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        /*
        Vector3 rotateDirection = new Vector3();

        if (Input.GetKeyDown(KeyCode.LeftArrow)) rotateDirection = ProcessDirection(-playerTransform.right);
        else if (Input.GetKeyDown(KeyCode.RightArrow)) rotateDirection = ProcessDirection(playerTransform.right);
        else if (Input.GetKeyDown(KeyCode.UpArrow)) rotateDirection = ProcessDirection(playerTransform.forward);
        else if (Input.GetKeyDown(KeyCode.DownArrow)) rotateDirection = ProcessDirection(-playerTransform.forward);

        //if (!isRotating && Input.GetMouseButtonDown(0) &&)
        //{
        //    SwitchGravity(rotateDirection);
        //}

        if (!isRotating)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) SwitchGravity(ProcessDirection(-playerTransform.right));
            else if (Input.GetKeyDown(KeyCode.RightArrow)) SwitchGravity(ProcessDirection(playerTransform.right));
            else if (Input.GetKeyDown(KeyCode.UpArrow)) SwitchGravity(ProcessDirection(playerTransform.forward));
            else if (Input.GetKeyDown(KeyCode.DownArrow)) SwitchGravity(ProcessDirection(-playerTransform.forward));
        }
        */
    }

    private Vector3 ProcessDirection(Vector3 direction)
    {
        direction.Normalize(); // Ensure it's a unit vector

        // Find the dominant axis
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

    private void SwitchGravity(Vector3 newUp)
    {
        // if (currentUp == newUp) return;
        currentUp = newUp;
        StartCoroutine(RotateWorld(newUp));
    }

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
                kvp.Key.transform.position = currentRotation * (kvp.Value.Item1 - transform.position) + transform.position;
                kvp.Key.transform.rotation = Quaternion.Slerp(kvp.Value.Item2, targetRotation * kvp.Value.Item2, t);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        foreach (var kvp in initialTransforms)
        {
            kvp.Key.transform.position = targetRotation * (kvp.Value.Item1 - transform.position) + transform.position;
            kvp.Key.transform.rotation = targetRotation * kvp.Value.Item2;
        }

        isRotating = false;
    }

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

    private void SelectDirection(InputAction.CallbackContext ctx)
    {
        rotateMode = true;
        hologramObject.SetActive(true);
        switch (ctx.control.name)
        {
            case "upArrow":
                rotateDirection = playerTransform.forward;
                // hologramObject.transform.rotation = Quaternion.Euler(0, -90, 90);
                // hologramObject.transform.localEulerAngles = new Vector3(0, -90, -90);
                break;
            case "downArrow":
                rotateDirection = -playerTransform.forward;
                // hologramObject.transform.rotation = Quaternion.Euler(0, 90, 90);
                // hologramObject.transform.localEulerAngles = new Vector3(0, 90, 90);
                break;
            case "leftArrow":
                rotateDirection = -playerTransform.right;
                // hologramObject.transform.rotation = Quaternion.Euler(0, 0, 90);
                // hologramObject.transform.localEulerAngles = new Vector3(0, 0, -90);
                break;
            case "rightArrow":
                rotateDirection = playerTransform.right;
                // hologramObject.transform.rotation = Quaternion.Euler(0, 0, -90);
                // hologramObject.transform.localEulerAngles = new Vector3(0, 0, 90);
                break;
        }

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

        // AlignDown(ProcessDirection(rotateDirection));
        
        Debug.Log(ProcessDirection(rotateDirection));
        StartCoroutine(FadeCanvasGroup(rotationPromptPanel, 1, 0.2f));
    }

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

    void AlignDown(Vector3 downVector)
    {
        Vector3 forward = hologramObject.transform.forward; // Preserve local forward
        hologramObject.transform.rotation = Quaternion.LookRotation(forward, -downVector);
    }

    private void DisableRotate(InputAction.CallbackContext ctx)
    {
        rotateMode = false;
        hologramObject.SetActive(false);
        StartCoroutine(FadeCanvasGroup(rotationPromptPanel, 0, 0.2f));
    }

    private void OnDisable()
    {
        controls.Rotate.Disable();
    }

    private void OnEnable()
    {
        controls.Rotate.Enable();
    }
}