using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : Interactables
{
    [SerializeField]
    float _throwingForce = 450;

    [SerializeField]
    AudioClip _hitClip;
    [SerializeField]
    float _range = 4;

    SoundCaster _soundCaster;
    Rigidbody _rb;

    bool _grabbed, _thrown;

    private void Start()
    {
        _soundCaster = GetComponent<SoundCaster>();
        _rb = GetComponent<Rigidbody>();
    }

    public override void Interact(GameObject interacter)
    {
        //first interaction, grab the object
        if (!_grabbed) {
            transform.parent = Camera.main.transform;
            //_rb.isKinematic = true;
            _rb.constraints = RigidbodyConstraints.FreezeAll;

            transform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.forward;

            _grabbed = true;

            return;
        }
        //second interaction, throw object
        transform.parent = null;
        //_rb.isKinematic = false;
        _rb.constraints = RigidbodyConstraints.None;

        _grabbed = false;
        _thrown = true;

        _rb.AddForce(Camera.main.transform.forward * _throwingForce, ForceMode.Force);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //play audio when this hits something, but only if thrown
        if (_thrown)
        {
            _soundCaster.PlayAudio(_hitClip, _range);
            _thrown = false;
        }
    }
}
