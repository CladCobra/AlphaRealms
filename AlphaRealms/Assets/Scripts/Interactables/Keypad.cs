using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keypad : Interactable {

    [Header("References")]
    [SerializeField] private Transform door;

    [Header("Opening")]
    private bool doorOpen;

    protected override void Interact() {

        doorOpen = !doorOpen;
        door.GetComponent<Animator>().SetBool("isOpen", doorOpen);

    }
}
