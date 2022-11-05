using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {

    [Header("References")]
    [SerializeField] private GunController gunController;
    public PlayerInput playerInput;
    private PlayerController playerController;

    [Header("Input Actions")]
    private InputActionMap playerMap;
    private InputActionMap weaponMap;

    private void OnEnable() {

        playerMap = playerInput.Player;
        weaponMap = playerInput.Weapon;

        playerMap.Enable();
        weaponMap.Enable();

    }

    private void OnDisable() {

        playerMap.Disable();
        weaponMap.Disable();

    }

    private void Awake() {

        playerInput = new PlayerInput();

    }

    private void Start() {

        playerController = GetComponent<PlayerController>();

        playerInput.Player.Sprint.performed += ctx => playerController.StartSprint();
        playerInput.Player.Sprint.canceled += ctx => playerController.StopSprint();
        playerInput.Player.Jump.performed += ctx => playerController.Jump();
        playerInput.Player.Crouch.performed += ctx => playerController.Crouch();
        playerInput.Weapon.Shoot.performed += ctx => gunController.Shoot();
        playerInput.Weapon.ADS.performed += ctx => gunController.ToggleADS();
        playerInput.Weapon.ADS.canceled += ctx => gunController.ToggleADS();
        playerInput.Weapon.Reload.performed += ctx => gunController.Reload();

    }

    private void FixedUpdate() {

        playerController.Move(playerInput.Player.Movement.ReadValue<Vector2>());

        if (gunController.isAutomatic) {

            if (playerInput.Weapon.Shoot.IsPressed()) {

                gunController.Shoot();

            }
        } else {

            playerInput.Weapon.Shoot.performed += ctx => gunController.Shoot();

        }
    }

    private void LateUpdate() {

        playerController.Look(playerInput.Player.Look.ReadValue<Vector2>());

    }
}
