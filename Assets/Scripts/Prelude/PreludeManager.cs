using NUnit.Framework.Internal.Commands;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PreludeManager : MonoBehaviour
{
    GlobalControls controls;

    private void Awake()
    {
        controls = new GlobalControls();

        controls.Skip.SkipAction.performed += ctx => Skip(ctx);
        controls.Skip.SkipAction.canceled += ctx => Skip(ctx);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Skip(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValueAsButton())
        {
            Time.timeScale = 25f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    private void OnEnable()
    {
        controls.Skip.Enable();
    }

    private void OnDisable()
    {
        controls.Skip.Disable();
    }
}
