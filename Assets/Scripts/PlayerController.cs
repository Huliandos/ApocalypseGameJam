using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Controller vars")]
    [SerializeField]
    float _forwardSpeed = 1;
    [SerializeField]
    float _backwardSpeed = .5f, _sideSpeed = .75f, _sprintSpeedModifier = 2;

    [Header("Footstep audio")]
    [SerializeField]
    AudioClip[] _fastSteps, _slowSteps;
    [SerializeField]
    AudioSource _footstepAudio;

    [Header("Camera step movement simulation")]
    float _camWiggleValueDefault = 0.01f, _camWiggleValue = 0.01f;
    float _camCenterValue;
    float _camMovementInterpolationValue = 0.0005f, _camMovementInterpolationValueDefault = 0.0005f;
    bool _riseCamera = true;

    [Header("Input states")]
    //bool moveLeft, moveRight, moveUpwards, moveDownwards, sprint;
    bool _moveHorizontal, _moveVertical, _sprint;

    [Header("Auxiliary")]
    [SerializeField]
    Transform _cameraTransform;

    Rigidbody _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
        _camCenterValue = _cameraTransform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        //movement
        if (Input.GetAxis(Inputs.horizontal) != 0)
            _moveHorizontal = true;
        else
            _moveHorizontal = false;

        if (Input.GetAxis(Inputs.vertical) != 0)
            _moveVertical = true;
        else
            _moveVertical = false;


        if (Input.GetAxis(Inputs.sprint) > 0)
            _sprint = true;
        else
            _sprint = false;

        /* //ToDo:
        if (Input.GetAxis(Inputs.interact) > 0)
        {
            GetComponent<BoxCollider>().enabled = true;
        }
        else
        {
            GetComponent<BoxCollider>().enabled = false;
        }
        */

        //ToDo: only move camera with this
        //camera movement for first person view
        _cameraTransform.eulerAngles = new Vector3(_cameraTransform.eulerAngles.x - Input.GetAxis(Inputs.mouseY), _cameraTransform.eulerAngles.y + Input.GetAxis(Inputs.mouseX), 0);

        if (_cameraTransform.eulerAngles.x > 85 && _cameraTransform.eulerAngles.x < 180)
        {
            _cameraTransform.eulerAngles = new Vector3(85, _cameraTransform.eulerAngles.y, 0);
        }
        else if (_cameraTransform.eulerAngles.x < 275 && _cameraTransform.eulerAngles.x > 180)
        {
            _cameraTransform.eulerAngles = new Vector3(-85, _cameraTransform.eulerAngles.y, 0);
        }
    }

    private void FixedUpdate()
    {
        float verticalSpeed = _forwardSpeed;

        if (Input.GetAxis(Inputs.vertical) < 0)
            verticalSpeed = _backwardSpeed;

        Vector3 velocity = _cameraTransform.forward * verticalSpeed * _sprintSpeedModifier * Input.GetAxis(Inputs.vertical)
            + _cameraTransform.right * _sprintSpeedModifier * Input.GetAxis(Inputs.horizontal);

        if (_sprint)
            velocity *= _sprintSpeedModifier;

        //ToDo: Not a clean solution
        _rb.velocity = velocity;

        _camMovementInterpolationValue = _camMovementInterpolationValueDefault * _rb.velocity.magnitude * 8;
        _camWiggleValue = _camWiggleValueDefault * _rb.velocity.magnitude * 2f;

        //no movement
        if(!_moveVertical && !_moveHorizontal)
        {
            //ToDo: Not a clean solution
            _rb.velocity = new Vector3(0, 0, 0);
            _camWiggleValue = _camWiggleValueDefault;
            if (_camMovementInterpolationValue >= 0)
            {
                _camMovementInterpolationValue = _camMovementInterpolationValueDefault;
            }
            else
            {
                _camMovementInterpolationValue = -_camMovementInterpolationValueDefault;
            }
        }

        //camera wiggle up and down (imitates steps and breathing
        if (_cameraTransform.position.y > _camCenterValue + _camWiggleValue && _riseCamera)
        {
            _riseCamera = false;
        }
        else if (_cameraTransform.position.y < _camCenterValue - _camWiggleValue && !_riseCamera)
        {
            _riseCamera = true;

            //play a footstep sound if the player is moving AND the camera has reached it's low point
            if (_moveVertical || _moveHorizontal)
            {
                int random = Random.Range(0, _fastSteps.Length);

                if (_sprint)
                    _footstepAudio.PlayOneShot(_footstepAudio.clip = _fastSteps[random]);

                else
                    _footstepAudio.PlayOneShot(_footstepAudio.clip = _slowSteps[random]);
            }
        }

        if ((_riseCamera && _camMovementInterpolationValue < 0) || (!_riseCamera && _camMovementInterpolationValue > 0))
        {
            _camMovementInterpolationValue *= -1;
        }

        //camera wiggle for breathing, walking and sprinting
        _cameraTransform.position = new Vector3(_cameraTransform.position.x, _cameraTransform.position.y + _camMovementInterpolationValue, _cameraTransform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        /*
        if (other.gameObject.tag == Tags.door  && !other.gameObject.GetComponentInParent<Animation>().isPlaying) {
            if (other.gameObject.GetComponentInParent<DoorState>().opened == true)
                other.gameObject.GetComponentInParent<Animation>().Play("close");
            else
                other.gameObject.GetComponentInParent<Animation>().Play("open");

            int random = Random.Range(0, other.gameObject.GetComponentInParent<DoorState>().doorSounds.Length);
            other.gameObject.GetComponentInParent<AudioSource>().clip = other.gameObject.GetComponentInParent<DoorState>().doorSounds[random];
            other.gameObject.GetComponentInParent<AudioSource>().Play();
        }
        */
    }
}
