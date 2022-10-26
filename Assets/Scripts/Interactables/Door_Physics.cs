using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Physics : Door
{
    [SerializeField]
    float _force = 10;

    Rigidbody _rb;

    private void Start()
    {
        _soundCaster = GetComponent<SoundCaster>();
        _rb = GetComponent<Rigidbody>();
    }

    public override void Interact(GameObject interacter)
    {
        PlayRandomAudioClip();

        //find out which direction the interacter is interacting from 
        //from positive Z direction
        if ((transform.position - interacter.transform.position).z >= 0)
        {
            //ignore y direction, cause doors just move on xz-plane
            _rb.AddForce(transform.forward * _force, ForceMode.Impulse);
            return;
        }
        //from negative Z direction
        _rb.AddForce(-transform.forward * _force, ForceMode.Impulse);
    }
}
