using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour {

    [Header("Interacting")]
    public string interactMessage;

    [Header("Events")]
    public bool useEvents;

    public void BaseInteract() {

        if (useEvents) {

            GetComponent<InteractionEvent>().onInteract.Invoke();

        }

        Interact();

    }

    protected virtual void Interact() {

    }
}
