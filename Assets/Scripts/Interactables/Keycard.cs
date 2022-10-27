using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keycard : Interactables
{
    [SerializeField]
    Door[] _doorsBoundToKeycard;

    public override void Interact(GameObject interacter)
    {
        //ToDo: Play sound here

        foreach (Door door in _doorsBoundToKeycard)
            door.UnlockDoor();

        Destroy(gameObject);
    }
}
