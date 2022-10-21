using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility mono behaviour. Just to forward Trigger enter events to another class
/// //ToDo: Just works for PlayerController now. Make gloabally work later
/// </summary>
public class SendTriggerEventMonoBehaviour : MonoBehaviour
{
    [SerializeField]
    PlayerController _behaviourToSendTo;

    private void OnTriggerEnter(Collider other)
    {
        _behaviourToSendTo.OnTriggerEnterFromChild(other);
    }
}
