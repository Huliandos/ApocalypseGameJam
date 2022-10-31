using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keycard : Interactables
{
    [SerializeField]
    Door[] _doorsBoundToKeycard;

    [SerializeField][Tooltip("Used for activating a monster spawn zone in the office")]
    GameObject[] _gosToSetActiveAfterCollection;

    GameController _gameController;

    [SerializeField]
    AudioClip _keycardPickupSound, _audioLogSound;

    private void Start()
    {
        _gameController = FindObjectOfType<GameController>();    
    }

    public override void Interact(GameObject interacter)
    {
        if(_gameController)
            _gameController.PlayGlobalOneshotAudio(_keycardPickupSound);

        //play audio log attached to this keycard, if an audio log is attached to it
        if (_audioLogSound != null) {
            _gameController.PlayGlobalAudio(_audioLogSound);
        }

        foreach (Door door in _doorsBoundToKeycard)
            door.UnlockDoor();

        foreach (GameObject go in _gosToSetActiveAfterCollection)
            go.SetActive(true);

        Destroy(gameObject);
    }
}
