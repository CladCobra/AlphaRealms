using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

    [Header("States")]
    private string currentState;
    private StateMachine stateMachine;
    private NavMeshAgent agent;

    [Header("Route")]
    public Route route;

    public NavMeshAgent Agent {

        get => agent;

    }

    private void Start() {

        stateMachine = GetComponent<StateMachine>();
        agent = GetComponent<NavMeshAgent>();

        stateMachine.Initialize();

    }
}
