using UnityEngine;
using System.Collections;

public class NpcMovingController : MonoBehaviour
{
    private TopDownController controller;
    
    private enum State
    {
        Waiting,
        TransitionToWaiting,
        Walking,
        TransitionToWalking,
        InteractingWithPlayer
    }

    private State currentState;

    private float minWaitTime = 1;
    private float maxWaitTime = 5;
    private float minMoveTime = 2;
    private float maxMoveTime = 4;

    private float runningTime;
    private float targetTime;
    private float xMoveNpc;
    private float yMoveNpc;


    // Use this for initialization
    void Start()
    {
        controller = GetComponent<TopDownController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Check current state
        //If state is static, wait until they feel like moving
        //If they are moving, pick a direction and walk for a bit or until they hit a wall
        //Once they hit a wall, stop and wait
        //controller.xMove = 1;
        EvaluateStates();
    }

    private void EvaluateStates()
    {
        switch (currentState)
        {
            case State.TransitionToWaiting:
                ResetWaitingState();
                break;
            case State.Waiting:
                ContinueWaiting();
                break;
            case State.TransitionToWalking:
                ResetWalkingState();
                break;
            case State.Walking:
                ContinueWalking();
                break;
            default:
                //reset if screwy
                ChangeState(State.TransitionToWaiting);
                break;
        }
    }

    private void ContinueWalking()
    {
        if (targetTime <= 0)
        {
            targetTime = Random.Range(minMoveTime, maxMoveTime);
            xMoveNpc = Random.Range(-1f, 1f);
            yMoveNpc = Random.Range(-1f, 1f);
        }

        controller.xMove = xMoveNpc;
        controller.yMove = yMoveNpc;

        if (DidWePassTargetTime())
        {
            ChangeState(State.TransitionToWaiting);
            return;
        }
    }

    private void ContinueWaiting()
    {
        if (targetTime <= 0)
        {
            targetTime = Random.Range(minWaitTime, maxWaitTime);
            //Do nothing here
        }

        if (DidWePassTargetTime())
        {
            ChangeState(State.TransitionToWalking);
            return;
        }

    }

    private void ChangeState(State stateToChangeTo)
    {
        controller.xMove = controller.yMove = 0;
        currentState = stateToChangeTo;
    }

    private bool DidWePassTargetTime()
    {
        runningTime += Time.deltaTime;
        return runningTime > targetTime;
    }

    private void ResetWalkingState()
    {
        targetTime = runningTime = 0;
        xMoveNpc = yMoveNpc = 0;
        ChangeState(State.Walking);
    }

    private void ResetWaitingState()
    {
        targetTime = runningTime = 0;
        ChangeState(State.Waiting);
    }
}
