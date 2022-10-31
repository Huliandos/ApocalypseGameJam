using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMonsterTrigger : MonoBehaviour
{
    GameController _gameController;

    [SerializeField]
    Transform _monsterSpawn, _monsterGoto;

    private void Start()
    {
        _gameController = FindObjectOfType<GameController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == Tags.Player) {
            _gameController.SpawnMonster(this, _monsterSpawn.position, _monsterGoto.position);

            gameObject.SetActive(false);
        }
    }
}
