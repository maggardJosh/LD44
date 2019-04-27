using UnityEngine;
using System.Collections;

public class NpcStaticController : Npc
{
    private Rigidbody2D rigidBody;
    private Animator animator;

    private State currentState;

    private enum State
    {
        Waiting,
        Interacting,
        InteractingComplete
    }


    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        EvaluateStates();
    }

    private void EvaluateStates()
    {
        switch (currentState)
        {
            case State.Waiting:
                ContinueWaiting();
                break;
            case State.Interacting:
                ContinueInteracting();
                break;
            case State.InteractingComplete:
                ChangeState(State.Waiting);
                break;
            default:
                //reset if screwy
                ChangeState(State.Waiting);
                break;
        }
    }

    private void ChangeState(State stateToChangeTo)
    {
        currentState = stateToChangeTo;
    }

    private void ContinueInteracting()
    {
        animator.speed = .2f;
    }

    private void ContinueWaiting()
    {
        animator.speed = 1;
    }

    public override void Interact(GameObject player)
    {
        ChangeState(State.Interacting);
    }

    public override void StopInteracting()
    {
        ChangeState(State.InteractingComplete);
    }
}
