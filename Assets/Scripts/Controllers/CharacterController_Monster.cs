using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterController_Monster : MonoBehaviour
{
    //ToDo: Add screaming sometimes when hearing a sound before running towards it
    AnimationController_Monster _animationController;
    NavMeshAgent _agent;
    GameController _gameController;

    enum AI_States { LISTEN, STROLL_AROUND, CHASE, KILL };
    AI_States _myState = AI_States.LISTEN;

    [SerializeField][Tooltip("Sets range for idle time in seconds")]
    float _minTimeToIdle = 3, _maxTimeToIdle = 5;


    [SerializeField]
    [Tooltip("Sets range for strolling in Unity units")]
    float _maxRangeToStroll = 10;

    //For strolling and chasing
    [SerializeField]
    float _distanceGoalPosReached = .4f;
    Vector3 _goalPos;

    [Header("Audio")]
    [SerializeField]
    [Tooltip("Randomly selected audio clips for idle sounds")]
    AudioClip[] _idleSounds;

    [SerializeField]
    [Tooltip("Sets range for making idle sounds")]
    float _minTimeBetweenIdleSounds = 2, _maxTimeBetweenIdleSounds = 6;

    Coroutine _makeRandomSoundRoutine;

    [SerializeField]
    [Tooltip("Randomly selected audio clips for step sounds")]
    AudioClip[] _steps;

    AudioSource _audioSource;

    // Start is called before the first frame update
    void Awake()
    {
        _animationController = GetComponent<AnimationController_Monster>();
        _agent = GetComponent<NavMeshAgent>();
        _audioSource = GetComponent<AudioSource>();

        _gameController = FindObjectOfType<GameController>();

        SwapToListen();
    }

    // Update is called once per frame
    void Update()
    {
        switch (_myState) {
            case AI_States.LISTEN:
                //Do nothing on update
                break;
            case AI_States.STROLL_AROUND:
            case AI_States.CHASE:
                if (GoalReached())
                {
                    SwapToListen();
                }
                break;
            case AI_States.KILL:
                //Do nothing on update
                break;
        }
    }

    void SwapToListen()
    {
        _myState = AI_States.LISTEN;
        StartCoroutine(IdleTimeInRange());
        if(_makeRandomSoundRoutine == null)
            _makeRandomSoundRoutine = StartCoroutine(MakeRandomSound());
    }

    public void HearAudio(Vector3 position) {
        //Once the kill animation is initiated, we don't switch states anymore
        if (_myState == AI_States.KILL)
            return;

        //Don't have to set a new goal pos, if the next one is close to the current one
        if ((_goalPos - position).magnitude <= _distanceGoalPosReached)
            return;

        //Debug.Log("Stopping coroutine");
        //stop switch to strolling behaviour
        StopAllCoroutines();
        _makeRandomSoundRoutine = null;

        //Set AI state for FSM
        _myState = AI_States.CHASE;

        //cast to floor
        position.y = 0;

        GotoPosition(position);
    }

    IEnumerator IdleTimeInRange()
    {
        yield return new WaitForSeconds(Random.Range(_minTimeToIdle, _maxTimeToIdle));
        StrollToRandomPosition();
    }

    IEnumerator MakeRandomSound()
    {
        //Debug.Log("Starting coroutine");
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(_minTimeBetweenIdleSounds, _maxTimeBetweenIdleSounds));
            _audioSource.PlayOneShot(_idleSounds[Random.Range(0, _idleSounds.Length)], .6f);
        }
    }

    void StrollToRandomPosition()
    {
        //Set AI state for FSM
        _myState = AI_States.STROLL_AROUND;

        //Generate random pos in circle
        Vector3 position = MathFunctions.RandomPositionInCircle(transform.position, _maxRangeToStroll);

        GotoPosition(position);
    }

    void GotoPosition(Vector3 position)
    {
        NavMeshHit hit;
        NavMesh.SamplePosition(position, out hit, Mathf.Infinity, NavMesh.AllAreas);
        _goalPos = hit.position;

        _agent.SetDestination(hit.position);
    }

    bool GoalReached() {
        if ((transform.position - _goalPos).magnitude <= _distanceGoalPosReached)
            return true;
        return false;
    }

    void InitiatePlayerDeath()
    {
        //Let the agent stop
        _agent.SetDestination(transform.position);
        _myState = AI_States.KILL;
        _gameController.StartCoroutine(_gameController.DeathAnimation());
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if the monster touches the player, then it also initiates its killing blow
        if (collision.gameObject.tag == Tags.Player)
        {
            InitiatePlayerDeath();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Can only kill the player in range when chasing
        //ToDo: May have to add a flag here when the monster knows it's chasing the player
        if (_myState == AI_States.CHASE && other.tag == Tags.Player)
        {
            InitiatePlayerDeath();
        }
    }

    #region utility
    /// <summary>
    /// called by animation
    /// </summary>
    public void PlayRandomFootstepAudio() {
        _audioSource.PlayOneShot(_steps[Random.Range(0, _steps.Length)]);
    }

    #endregion
}
