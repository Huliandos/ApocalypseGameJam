using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractVictory : Interactables
{
    GameController _gameController;

    [SerializeField]
    GameObject _interactWithComputerTooltipCanvas;

    // Start is called before the first frame update
    void Start()
    {
        _gameController = FindObjectOfType<GameController>();
    }


    public override void Interact(GameObject interacter)
    {
        _gameController.VictoryAnimation();

        _interactWithComputerTooltipCanvas.SetActive(false);
    }
}
