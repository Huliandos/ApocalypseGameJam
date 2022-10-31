using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_SlidingAnimation : Door
{
    [SerializeField]
    AudioClip[] _openingSounds, _closingSounds;
    [SerializeField]
    AudioClip _doorLockedSound;

    AudioSource _audioSource;

    int _charsInTrigger = 0;

    [SerializeField]
    Animator _animator;

    const string _animatorParam_move = "Move";

    private void Start()
    {
        _soundCaster = GetComponent<SoundCaster>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == Tags.Player || other.tag == Tags.Enemy) {
            if (_doorIsLocked)
            {
                _audioSource.PlayOneShot(_doorLockedSound);

                return;
            }

            _charsInTrigger++;

            //only if a character trigger and no other is in it play anim
            if (_charsInTrigger == 1)
            {
                _animator.SetTrigger(_animatorParam_move);
                //opening door triggers monster
                _soundCaster.PlayAudio(_openingSounds[Random.Range(0, _openingSounds.Length)], _openingRange);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == Tags.Player || other.tag == Tags.Enemy)
        {
            if (_doorIsLocked)
                return;

            _charsInTrigger--;

            if (_charsInTrigger == 0)
            {
                _animator.SetTrigger(_animatorParam_move);
                //closing not
                _audioSource.PlayOneShot(_closingSounds[Random.Range(0, _closingSounds.Length)]);
            }
        }
    }

    public override void Interact(GameObject interacter)
    {
        //doesn't do anything for sliding doors
    }
}
