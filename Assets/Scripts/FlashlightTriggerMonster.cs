using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightTriggerMonster : MonoBehaviour
{
    [SerializeField]
    float _range = 8;

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        Physics.Raycast(transform.position, transform.forward, out hit, _range);
        if (DidRaycastHit(hit))
            return;

        Physics.Raycast(transform.position + transform.up, transform.forward, out hit, _range);
        if (DidRaycastHit(hit))
            return;

        Physics.Raycast(transform.position - transform.up, transform.forward, out hit, _range);
        if (DidRaycastHit(hit))
            return;

        Physics.Raycast(transform.position + transform.right, transform.forward, out hit, _range);
        if (DidRaycastHit(hit))
            return;

        Physics.Raycast(transform.position + transform.right, transform.forward, out hit, _range);
        if (DidRaycastHit(hit))
            return;
    }

    bool DidRaycastHit(RaycastHit hit)
    {
        if (hit.transform == null)
            return false;

        //monster hit by light
        if (hit.transform.gameObject.tag == Tags.Enemy)
            hit.transform.GetComponent<CharacterController_Monster>().HearAudio(transform.position);

        return hit.transform.gameObject.tag == Tags.Enemy;
    }
}
