using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_SlidingAnimation : MonoBehaviour
{
    [SerializeField]
    AudioClip[] _openingSounds, _closingSounds;
    AudioSource _audioSource;

    int _charsInTrigger = 0;

    [SerializeField]
    Animator _animator;

    const string _animatorParam_move = "Move";

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == Tags.Player || other.tag == Tags.Enemy) {
            _charsInTrigger++;

            //only if a character trigger and no other is in it play anim
            if (_charsInTrigger == 1)
            {
                _animator.SetTrigger(_animatorParam_move);
                _audioSource.PlayOneShot(_openingSounds[Random.Range(0, _openingSounds.Length)]);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == Tags.Player || other.tag == Tags.Enemy)
        {
            _charsInTrigger--;

            if (_charsInTrigger == 0)
            {
                _animator.SetTrigger(_animatorParam_move);
                _audioSource.PlayOneShot(_closingSounds[Random.Range(0, _closingSounds.Length)]);
            }
        }
    }
}
