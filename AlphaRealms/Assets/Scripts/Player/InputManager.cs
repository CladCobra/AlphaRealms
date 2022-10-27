using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {

    [Header("References")]
    public PlayerInput.OnFootActions onFoot;
    private PlayerController playerController;
    private PlayerInput playerInput;

    private void OnEnable() {

        onFoot.Enable();

    }

    private void OnDisable() {

        onFoot.Disable();

    }

    private void Awake() {

        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;

    }

    private void Start() {

        playerController = GetComponent<PlayerController>();

        onFoot.Sprint.performed += ctx => playerController.StartSprint();
        onFoot.Sprint.canceled += ctx => playerController.StopSprint();
        onFoot.Jump.performed += ctx => playerController.Jump();
        // onFoot.Crouch.performed += ctx => playerController.Crouch();

    }

    private void FixedUpdate() {

        playerController.Move(onFoot.Movement.ReadValue<Vector2>());

    }

    private void LateUpdate() {

        playerController.Look(onFoot.Look.ReadValue<Vector2>());

    }
}
