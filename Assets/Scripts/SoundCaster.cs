using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCaster : MonoBehaviour
{
    CharacterController_Monster _monster;
    AudioSource _audioSource;

    [SerializeField][Tooltip("Set for all sound casters. The distance in Unity units at which the monster can precisely hear the player")]
    static float _minPrecision;
    // Start is called before the first frame update
    void Start()
    {
        //There's only one monster in the scene, so all sound casters cast their sounds to the monster
        _monster = FindObjectOfType<CharacterController_Monster>(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAudio() { 
    
    }
}
