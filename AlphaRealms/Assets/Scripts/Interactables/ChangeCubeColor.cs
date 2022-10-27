using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCubeColor : Interactable {

    [Header("Color Changing")]
    [SerializeField] private Material[] colors;
    private int color;

    private void Start() {

        color = 0;

        GetComponent<MeshRenderer>().material = colors[color];

    }

    protected override void Interact() {

        color++;

        if (color > colors.Length - 1) {

            color = 0;

        }

        GetComponent<MeshRenderer>().material = colors[color];

    }
}
