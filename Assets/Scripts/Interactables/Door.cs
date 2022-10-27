using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Door : Interactables
{
    [Header("Audio casting to monster")]
    [SerializeField]
    [Tooltip("So monster can hear door")]
    protected float _openingRange = 4;

    [Header("Sounds")]
    [SerializeField][Tooltip("Not used for sliding doors")]
    protected AudioClip[] _doorSounds;
    protected SoundCaster _soundCaster;

    [SerializeField]
    protected bool _doorIsLocked;

    protected void PlayRandomAudioClip()
    {
        _soundCaster.PlayAudio(_doorSounds[Random.Range(0, _doorSounds.Length)], _openingRange);
    }

    /// <summary>
    /// Called by keycards bound to this door
    /// </summary>
    public void UnlockDoor() {
        _doorIsLocked = false;
    }
}
