using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour {

    [Header("References")]
    [SerializeField] private Camera camera;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private Sprite crosshairImage;
    [SerializeField] private Sprite interactCrosshairImage;
    private PlayerUI playerUI;
    private InputManager inputManager;

    [Header("Interacting")]
    [SerializeField][Range(0, 10)] private float interactDistance;
    [SerializeField] private LayerMask interactMask;

    private void Start() {

        playerUI = GetComponent<PlayerUI>();
        inputManager = GetComponent<InputManager>();

    }

    private void Update() {

        crosshair.GetComponent<Image>().sprite = crosshairImage;
        playerUI.UpdateInteractText(string.Empty);

        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, interactDistance, interactMask)) {

            if (hitInfo.collider.GetComponent<Interactable>() != null) {

                crosshair.GetComponent<Image>().sprite = interactCrosshairImage;

                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();

                playerUI.UpdateInteractText(interactable.interactMessage);

                if (inputManager.playerInput.Player.Interact.triggered) {

                    interactable.BaseInteract();

                }
            }
        }
    }
}
