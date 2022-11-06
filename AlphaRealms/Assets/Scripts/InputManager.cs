using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {

    [Header("References")]
    [SerializeField] private GunController gunController;
    [SerializeField] private UIController UIController;
    public PlayerInput playerInput;
    private PlayerController playerController;

    [Header("Input Actions")]
    private InputActionMap playerMap;
    private InputActionMap weaponMap;
    private InputActionMap menuMap;

    private void OnEnable() {

        playerMap = playerInput.Player;
        weaponMap = playerInput.Weapon;
        menuMap = playerInput.Menu;

        playerMap.Enable();
        weaponMap.Enable();
        menuMap.Disable();

    }

    private void OnDisable() {

        playerMap.Disable();
        weaponMap.Disable();
        menuMap.Disable();

    }

    private void Awake() {

        playerInput = new PlayerInput();

    }

    private void Start() {

        playerController = GetComponent<PlayerController>();

        playerInput.Player.Sprint.performed += ctx => playerController.ToggleSprint();
        playerInput.Player.Sprint.canceled += ctx => playerController.ToggleSprint();
        playerInput.Player.Jump.performed += ctx => playerController.Jump();
        playerInput.Player.Crouch.performed += ctx => playerController.ToggleCrouch();
        playerInput.Weapon.Shoot.performed += ctx => gunController.Shoot();
        playerInput.Weapon.ADS.performed += ctx => gunController.ToggleADS();
        playerInput.Weapon.ADS.canceled += ctx => gunController.ToggleADS();
        playerInput.Weapon.Reload.performed += ctx => gunController.Reload();
        playerInput.Player.PauseGame.performed += ctx => UIController.OpenPauseMenu();
        playerInput.Menu.CloseMenu.performed += ctx => UIController.CloseCurrentMenu();
        playerInput.Menu.Click.performed += ctx => UIController.PlayClickSound();

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
