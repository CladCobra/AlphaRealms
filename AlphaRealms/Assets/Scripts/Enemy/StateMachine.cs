using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour {

    [Header("States")]
    [Range(0, 60)] public float patrolWaitTime;
    private BaseState activeState;
    private PatrolState patrolState;

    private void Update() {

        if (activeState != null) {

            activeState.Perform();

        }
    }

    public void Initialize() {

        patrolState = new PatrolState();
        ChangeState(patrolState);

    }

    public void ChangeState(BaseState state) {

        if (activeState != null) {

            activeState.Exit();

        }

        activeState = state;

        if (activeState != null) {

            activeState.stateMachine = this;
            activeState.enemy = GetComponent<Enemy>();
            activeState.Enter();

        }
    }
}
