using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateCube : Interactable {

    [Header("References")]
    [SerializeField] private Transform cube;

    [Header("Opening")]
    private bool isRotating;

    protected override void Interact() {

        isRotating = !isRotating;
        cube.GetComponent<Animator>().SetBool("isRotating", isRotating);

    }
}
