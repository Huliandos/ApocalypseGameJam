using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Door : Interactables
{
    [SerializeField]
    protected AudioClip[] _doorSounds;
    protected AudioSource _audioSource;

    protected void PlayRandomAudioClip()
    {
        _audioSource.PlayOneShot(_doorSounds[Random.Range(0, _doorSounds.Length)]);
    }
}
