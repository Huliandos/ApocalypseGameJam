using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Controller vars")]
    [SerializeField][Tooltip("Forwards walking speed of char")]
    float _forwardSpeed = 1.5f;
    [SerializeField]
    float _backwardSpeed = .75f, _sideSpeed = 1f, _sprintSpeedModifier = 2, _crouchSpeedModifier = .25f;
    [SerializeField]
    [Tooltip("Crouching height")]
    float _crouchingY = .75f;
    /// <summary>
    /// Set to transform position on Start. Used to uncrouch player
    /// </summary>
    float _standingY;

    [SerializeField]
    [Range(0, 1f)]
    [Tooltip("Lerp step size per frame for crouching and uncrouching")]
    float _crouchSpeed = .1f;

    /// <summary>
    /// Utility to stop crouching/uncrouching process, once the other one is demanded
    /// </summary>
    Coroutine _currentCrouchingCoroutine;

    [Header("Footstep audio")]
    [SerializeField][Tooltip("Randomly selected audio clips for running sounds")]
    AudioClip[] _fastSteps;
    [SerializeField][Tooltip("Randomly selected audio clips for walking sounds")]
    AudioClip[] _slowSteps;
    [SerializeField][Tooltip("Audio source attached as child to player for 3D audio")]
    AudioSource _footstepAudio;

    [Header("Camera steping/breathing movement simulation")]
    [SerializeField]
    const float _camMaxDistanceToStartDefault = 0.01f, _camMoveYIdle = 0.0005f;
    //These are getting adjusted according to the player movement speed
    float _camMaxDistanceToStart, _camMoveYNextFUpdate;
    float _camLocalCenterY;
    bool _raiseCamera = true, _breathingReset;

    [Header("Input states")]
    //Used to get input in update and calculate movement in fixed update
    bool _moveHorizontal, _moveVertical, _sprint, _crouch;

    [Header("Auxiliary")]
    [SerializeField]
    Transform _cameraTransform;

    [SerializeField]
    BoxCollider _interactionTriggerCollider;

    Rigidbody _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody>();

        _standingY = transform.position.y;

        _camLocalCenterY = _cameraTransform.localPosition.y;
        _camMaxDistanceToStart = _camMaxDistanceToStartDefault;
        _camMoveYNextFUpdate = _camMoveYIdle;
    }

    // Update is called once per frame
    void Update()
    {
        //movement
        if (Input.GetAxis(Inputs.Horizontal) != 0)
            _moveHorizontal = true;
        else
            _moveHorizontal = false;

        if (Input.GetAxis(Inputs.Vertical) != 0)
            _moveVertical = true;
        else
            _moveVertical = false;


        if (Input.GetAxis(Inputs.Sprint) > 0)
            _sprint = true;
        else
            _sprint = false;


        if (Input.GetAxis(Inputs.Interact) > 0)
            _interactionTriggerCollider.enabled = true;
        else
            _interactionTriggerCollider.enabled = false;


        if (Input.GetAxis(Inputs.Crouch) > 0 && !_crouch)
        {
            _crouch = true;

            //crouching "animation"
            if (_currentCrouchingCoroutine != null)
                StopCoroutine(_currentCrouchingCoroutine);
            _currentCrouchingCoroutine = StartCoroutine(SmoothCrouch(_crouchingY));
        }
        else if (Input.GetAxis(Inputs.Crouch) == 0 && _crouch) {
            //if nothing is above the player stand up ELSE stay crouched until that's the case
            if (!(Physics.Raycast(transform.position + transform.right * transform.localScale.x/2, transform.up, .5f) ||
            Physics.Raycast(transform.position - transform.right * transform.localScale.x/2, transform.up, .5f) ||
            Physics.Raycast(transform.position + transform.forward * transform.localScale.z/2, transform.up, .5f) ||
            Physics.Raycast(transform.position - transform.forward * transform.localScale.z/2, transform.up, .5f)))
            {
                _crouch = false;

                //crouching "animation"
                if (_currentCrouchingCoroutine != null)
                    StopCoroutine(_currentCrouchingCoroutine);
                _currentCrouchingCoroutine = StartCoroutine(SmoothCrouch(_standingY));
            }
        }

        //camera movement for first person view
        _cameraTransform.localEulerAngles = new Vector3(_cameraTransform.localEulerAngles.x - Input.GetAxis(Inputs.MouseY), _cameraTransform.localEulerAngles.y + Input.GetAxis(Inputs.MouseX), 0);
        //clamp camera angles 
        ClampEulerAngles();
    }

    private void FixedUpdate()
    {
        //movement
        float verticalSpeed = _forwardSpeed;
        if (Input.GetAxis(Inputs.Vertical) < 0)
            verticalSpeed = _backwardSpeed;

        Vector3 velocity = ClampToMaxSpeed(verticalSpeed,
            new Vector3(_cameraTransform.forward.x, 0, _cameraTransform.forward.z).normalized * verticalSpeed * Input.GetAxis(Inputs.Vertical)
            + new Vector3(_cameraTransform.right.x, 0, _cameraTransform.right.z).normalized * Input.GetAxis(Inputs.Horizontal));

        if (_sprint)
            velocity *= _sprintSpeedModifier;
        if (_crouch)
            velocity *= _crouchSpeedModifier;

        //ToDo: Not a clean solution
        _rb.velocity = velocity;


        //camera up and down movement
        _camMoveYNextFUpdate = _camMoveYIdle + _camMoveYIdle * _rb.velocity.magnitude * 8;
        _camMaxDistanceToStart = _camMaxDistanceToStartDefault + _camMaxDistanceToStartDefault * _rb.velocity.magnitude * 2;

        //if camera has reached its max distance from the original position
        if (_cameraTransform.localPosition.y > _camLocalCenterY + _camMaxDistanceToStart && _raiseCamera)
        {
            _raiseCamera = false;
        }
        else if (_cameraTransform.localPosition.y < _camLocalCenterY - _camMaxDistanceToStart && !_raiseCamera)
        {
            _raiseCamera = true;

            if (_moveVertical || _moveHorizontal)
            {
                int random = Random.Range(0, _fastSteps.Length);

                if (_sprint)
                    _footstepAudio.PlayOneShot(_footstepAudio.clip = _fastSteps[random]);

                else
                    _footstepAudio.PlayOneShot(_footstepAudio.clip = _slowSteps[random]);
            }
        }
        /*
        //ToDo: make sure this cleaner solution works
        if (Mathf.Abs(_cameraTransform.localPosition.y - _camLocalCenterY) > _camMaxDistanceToStart)
        {
            _raiseCamera = !_raiseCamera;

            //play a footstep sound if the player is moving AND the camera has reached its low point
            if (_raiseCamera && (_moveVertical || _moveHorizontal))
            {
                int random = Random.Range(0, _fastSteps.Length);

                if (_sprint)
                    _footstepAudio.PlayOneShot(_footstepAudio.clip = _fastSteps[random]);

                else
                    _footstepAudio.PlayOneShot(_footstepAudio.clip = _slowSteps[random]);
            }
        }
        */

        //if camera is supposed to be lowered
        if (!_raiseCamera)
            _camMoveYNextFUpdate *= -1;

        //camera wiggle for breathing, walking and sprinting
        _cameraTransform.localPosition = new Vector3(_cameraTransform.localPosition.x, _cameraTransform.localPosition.y + _camMoveYNextFUpdate, _cameraTransform.localPosition.z);
    }

    public void OnTriggerEnterFromChild(Collider other)
    {
        if (other.tag == Tags.Interactable) {
            other.GetComponent<Interactables>().Interact(gameObject);
        }
    }

    #region helper funcitons
    Vector3 ClampToMaxSpeed(float maxSpeed, Vector3 vectorToClamp) {
        //vector is 0, 0 ,0
        if (vectorToClamp.magnitude == 0)
            return vectorToClamp;
        //u=L/|v|*v
        return (maxSpeed / vectorToClamp.magnitude) * vectorToClamp;
    }

    void ClampEulerAngles() {
        if (_cameraTransform.localEulerAngles.x > 85 && _cameraTransform.localEulerAngles.x < 180)
        {
            _cameraTransform.localEulerAngles = new Vector3(85, _cameraTransform.localEulerAngles.y, 0);
        }
        else if (_cameraTransform.localEulerAngles.x < 275 && _cameraTransform.localEulerAngles.x > 180)
        {
            _cameraTransform.localEulerAngles = new Vector3(-85, _cameraTransform.localEulerAngles.y, 0);
        }
    }

    IEnumerator SmoothCrouch(float goalY) {
        //allows margin for error in lerping and for lerp to still terminate
        while (Mathf.Abs(transform.position.y-goalY)>.01f) {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, goalY, transform.position.z), _crouchSpeed);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        //set this to null once coroutine is done
        _currentCrouchingCoroutine = null;
    }
    #endregion
}
