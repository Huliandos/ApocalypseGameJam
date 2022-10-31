using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnterEndgame : MonoBehaviour
{
    [SerializeField]
    GameObject _interactWithComputerTooltipCanvas;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == Tags.Player) 
            _interactWithComputerTooltipCanvas.SetActive(true);
    }
}
