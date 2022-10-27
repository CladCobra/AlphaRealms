using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : BaseState {

    [Header("Waypoints")]
    private float waitTime;
    private int waypointIndex;
    private float waitTimer;

    public override void Enter() {

        waitTime = stateMachine.patrolWaitTime;

    }

    public override void Perform() {

        PatrolCycle();

    }

    public override void Exit() {

    }

    public void PatrolCycle() {

        if (enemy.Agent.remainingDistance < 0.2f) {

            waitTimer += Time.deltaTime;

            if (waitTimer > waitTime) {

                if (waypointIndex < enemy.route.waypoints.Count - 1) {

                    waypointIndex++;

                } else {

                    waypointIndex = 0;
                }

                enemy.Agent.SetDestination(enemy.route.waypoints[waypointIndex].position);
                waitTimer = 0;

            }
        }
    }
}
