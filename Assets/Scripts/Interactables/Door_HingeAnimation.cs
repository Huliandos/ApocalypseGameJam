using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_HingeAnimation : Door
{
    [SerializeField][Tooltip("Should animation be mirrored?")]
    bool _mirror = false;
    bool _opened = false;
    bool _animationPlaying = false;

    Animator _animator;
    
    const string _animatorParam_move = "Move";
    const string _animatorParam_mirror = "Mirror";

    const string _animatorState_doNothing = "DoNothing";
    const string _animatorState_doorOpen = "DoorOpening";
    const string _animatorState_doorClosed = "DoorClosing";

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();

        _animator.SetBool(_animatorParam_mirror, _mirror);
    }

    public override void Interact(GameObject interacter)
    {
        //can't interact with door again, while its animation is playing
        if (_animationPlaying)
            return;

        //start animation trough trigger
        _animator.SetTrigger(_animatorParam_move);

        PlayRandomAudioClip();

        _animationPlaying = true;
        //reverse opening state
        _opened = !_opened;

        StartCoroutine(WaitForAnimationToFinish());
    }

    IEnumerator WaitForAnimationToFinish()
    {
        //Unity doesn't immediately switch animations, 
        //so we wait until the animation is swapped and then wait again until its finished, before reenabling interaction with this door

        //wait if either the animator state is do nothing,
        //or the door state is opened, but the animation played is still the closed door
        //or the door state is closed, but the animation played is still the opened door
        while (_animator.GetCurrentAnimatorStateInfo(0).IsName(_animatorState_doNothing) ||
            (_opened && _animator.GetCurrentAnimatorStateInfo(0).IsName(_animatorState_doorClosed))
            || (!_opened && _animator.GetCurrentAnimatorStateInfo(0).IsName(_animatorState_doorOpen))) 
        {
            yield return 0;
        }

        AnimatorStateInfo animStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        float normalizedTime = animStateInfo.normalizedTime;

        //once nomalized time exceeds 1, or subceeds -1 animation finished or looped
        while (normalizedTime < 1 && normalizedTime > 0)
        {
            animStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            normalizedTime = animStateInfo.normalizedTime;
            yield return 0;
        }

        //animation finished
        _animationPlaying = false;
    }
}
