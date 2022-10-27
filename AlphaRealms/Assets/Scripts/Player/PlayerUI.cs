using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour {

    [Header("Interacting")]
    [SerializeField] private TextMeshProUGUI interactText;

    public void UpdateInteractText(string newText) {

        interactText.text = newText;

    }
}
