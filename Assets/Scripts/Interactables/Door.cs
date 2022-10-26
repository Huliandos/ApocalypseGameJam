using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Door : Interactables
{
    [Header("Audio casting to monster")]
    [SerializeField]
    [Tooltip("So monster can hear door")]
    float _openingRange = 4;

    [Header("Sounds")]
    [SerializeField]
    protected AudioClip[] _doorSounds;
    protected SoundCaster _soundCaster;

    protected void PlayRandomAudioClip()
    {
        _soundCaster.PlayAudio(_doorSounds[Random.Range(0, _doorSounds.Length)], _openingRange);
    }
}
