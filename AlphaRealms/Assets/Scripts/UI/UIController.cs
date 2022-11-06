using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour {

    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private InputManager inputManager;

    [Header("Gameplay UI References")]
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject interactText;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject damageOverlay;

    [Header("Pause Menu UI References")]
    [SerializeField] private GameObject pauseMenu;

    [Header("Settings Menu UI References")]
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private Toggle invertLookXToggle;
    [SerializeField] private Toggle invertLookYToggle;

    [Header("Pausing")]
    private bool pauseMenuOpen;
    private bool settingsMenuOpen;

    private void Start() {

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);

        crosshair.SetActive(true);
        interactText.SetActive(true);
        healthBar.SetActive(true);
        damageOverlay.SetActive(true);

    }

    public void CloseCurrentMenu() {

        if (settingsMenuOpen) {

            CloseSettingsMenu();
            OpenPauseMenu();
            return;

        }

        if (pauseMenuOpen) {

            ClosePauseMenu();
            pauseMenuOpen = false;
            return;

        }
    }

    public void OpenPauseMenu() {

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        inputManager.playerInput.Player.Disable();
        inputManager.playerInput.Weapon.Disable();
        inputManager.playerInput.Menu.Enable();

        PauseGame();

        crosshair.SetActive(false);
        interactText.SetActive(false);
        healthBar.SetActive(false);
        damageOverlay.SetActive(false);
        settingsMenu.SetActive(false);

        pauseMenu.SetActive(true);

    }

    public void ClosePauseMenu() {

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        inputManager.playerInput.Menu.Disable();
        inputManager.playerInput.Player.Enable();
        inputManager.playerInput.Weapon.Enable();

        ResumeGame();

        crosshair.SetActive(true);
        interactText.SetActive(true);
        healthBar.SetActive(true);
        damageOverlay.SetActive(true);

        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);

    }

    private void PauseGame() {

        Time.timeScale = 0f;

    }

    private void ResumeGame() {

        Time.timeScale = 1f;

    }

    public void OpenSettingsMenu() {

        crosshair.SetActive(false);
        interactText.SetActive(false);
        healthBar.SetActive(false);
        damageOverlay.SetActive(false);
        pauseMenu.SetActive(false);

        settingsMenu.SetActive(true);

        pauseMenuOpen = false;
        settingsMenuOpen = true;

    }

    public void CloseSettingsMenu() {

        crosshair.SetActive(false);
        interactText.SetActive(false);
        healthBar.SetActive(false);
        damageOverlay.SetActive(false);

        settingsMenu.SetActive(false);
        pauseMenu.SetActive(true);

        pauseMenuOpen = true;
        settingsMenuOpen = false;

    }

    public void UpdateInvertLookX() {

        playerController.invertLookX = invertLookXToggle.isOn;

    }

    public void UpdateInvertLookY() {

        playerController.invertLookY = invertLookYToggle.isOn;

    }

    public void QuitGame() {

        Application.Quit();

    }

    public void PlayClickSound() {

        FindObjectOfType<AudioManager>().PlaySound("Click");

    }
}
