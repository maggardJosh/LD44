using UnityEngine;

public class NpcMovingController : Npc
{
    private TopDownController controller;

    private enum State
    {
        Waiting,
        TransitionToWaiting,
        Walking,
        TransitionToWalking,
        Interacting,
        InteractingComplete
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
            case State.Interacting:
                ContinueInteracting();
                break;
            case State.InteractingComplete:
                ChangeState(State.TransitionToWaiting);
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

    private void ContinueInteracting()
    {
        //do idle animations during this time maybe
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.otherCollider.CompareTag("Player"))
            return;

        Bonk((collision.collider.transform.position - collision.otherCollider.transform.position).normalized);
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
    

    public void Bonk(Vector2 collisionNormal)
    {
        Vector2 moveVector = new Vector2(xMoveNpc, yMoveNpc);
        Vector2 result = Vector2.Reflect(moveVector, collisionNormal);
        xMoveNpc = result.x;
        yMoveNpc = result.y;
    }

    public override void Interact(GameObject player)
    {
        controller.FacePosition(player.transform.position);
        ChangeState(State.Interacting);
    }

    public override void StopInteracting()
    {
        ChangeState(State.InteractingComplete);
    }
}
