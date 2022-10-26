using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimationController_Monster : MonoBehaviour
{
    Animator _animator;
    NavMeshAgent _agent;

    const string _animatorParam_speed = "Speed";

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        SetSpeed();
    }

    void SetSpeed() {
        _animator.SetFloat(_animatorParam_speed, _agent.velocity.magnitude);
    }
}
