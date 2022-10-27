using UnityEngine;

public abstract class BaseState {

    [Header("State Machine")]
    public StateMachine stateMachine;

    [Header("Enemy")]
    public Enemy enemy;

    public abstract void Enter();

    public abstract void Perform();

    public abstract void Exit();

}