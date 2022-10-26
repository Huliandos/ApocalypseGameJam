using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterController_Monster : MonoBehaviour
{
    //ToDo: Add screaming sometimes when hearing a sound before running towards it
    AnimationController_Monster _animationController;
    NavMeshAgent _agent;

    enum AI_States { LISTEN, STROLL_AROUND, CHASE, KILL };
    AI_States _myState = AI_States.LISTEN;

    [SerializeField][Tooltip("Sets range for idle time in seconds")]
    float _minTimeToIdle = 3, _maxTimeToIdle = 8;

    [SerializeField]
    [Tooltip("Sets range for strolling in Unity units")]
    float _maxRangeToStroll = 10;

    bool _waitingForIdleToFinish = false;

    //For strolling and chasing
    [SerializeField]
    float _distanceGoalPosReached = .4f;
    Vector3 _goalPos;

    // Start is called before the first frame update
    void Start()
    {
        _animationController = GetComponent<AnimationController_Monster>();
        _agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (_myState) {
            //can be optimized here, we don't need this state currently, as its just being called once
            case AI_States.LISTEN:
                if (!_waitingForIdleToFinish)
                    StartCoroutine(IdleTimeInRange());
                break;
            case AI_States.STROLL_AROUND:
            case AI_States.CHASE:
                if (GoalReached())
                    _myState = AI_States.LISTEN;
                break;
            case AI_States.KILL:
                //ToDo: Create Kill animation. Initiate it here
                break;
        }
    }

    public void HearAudio(Vector3 position) {
        //Once the kill animation is initiated, we don't switch states anymore
        if (_myState == AI_States.KILL)
            return;

        //Don't have to set a new goal pos, if the next one is close to the current one
        if ((_goalPos - position).magnitude <= _distanceGoalPosReached)
            return;

        //stop switch to strolling behaviour
        StopAllCoroutines();
        _waitingForIdleToFinish = false;

        //Set AI state for FSM
        _myState = AI_States.CHASE;

        //cast to floor
        position.y = 0;

        GotoPosition(position);
    }

    IEnumerator IdleTimeInRange() {
        _waitingForIdleToFinish = true;
        yield return new WaitForSeconds(Random.Range(_minTimeToIdle, _maxTimeToIdle));
        _waitingForIdleToFinish = false;
        StrollToRandomPosition();
    }

    void StrollToRandomPosition()
    {
        //Set AI state for FSM
        _myState = AI_States.STROLL_AROUND;

        //Generate random pos in circle
        float radius = _maxRangeToStroll * Mathf.Sqrt(Random.Range(0f, 1));
        float theta = Random.Range(0f, 1) * 2 * Mathf.PI;
        Vector3 position = new Vector3(radius * Mathf.Cos(theta), 0, radius * Mathf.Sin(theta));

        GotoPosition(position);
    }

    void GotoPosition(Vector3 position)
    {
        NavMeshHit hit;
        NavMesh.SamplePosition(position, out hit, Mathf.Infinity, NavMesh.AllAreas);
        Debug.Log("Going to: " + hit.position);
        _goalPos = hit.position;
        _agent.SetDestination(hit.position);
    }

    bool GoalReached() {
        if ((transform.position - _goalPos).magnitude <= _distanceGoalPosReached)
            return true;
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if the monster touches the player, then it also initiates its killing blow
        if (collision.gameObject.tag == Tags.Player)
        {
            //Let the agent stop
            _agent.SetDestination(transform.position);
            _myState = AI_States.KILL;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Can only kill the player in range when chasing
        //ToDo: May have to add a flag here when the monster knows it's chasing the player
        if (_myState == AI_States.CHASE && other.tag == Tags.Player)
        {
            //Let the agent stop
            _agent.SetDestination(transform.position);
            _myState = AI_States.KILL;
        }
    }
}
