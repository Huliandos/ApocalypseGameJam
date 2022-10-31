using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to send sounds to the monster
/// </summary>
public class SoundCaster : MonoBehaviour
{
    CharacterController_Monster _monster;
    AudioSource _audioSource;

    [Tooltip("Set for all sound casters. The distance in Unity units for a radius around this object's position. " +
        "Gets normalized against the range of the sound. Used to determine a random position within the vicinity of the cast sound.")]
    static float _minPrecisionAtMaxRange = 2;
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        //There's only one monster in the scene, so all sound casters cast their sounds to the monster
        _monster = FindObjectOfType<CharacterController_Monster>(true);
    }

    public void PlayAudio(AudioClip clip, float range) {
        _audioSource.PlayOneShot(clip);

        //only if the monster is actually spawned
        if (_monster == null || !_monster.gameObject.activeSelf)
            return;

        float distanceToMonster = (_monster.transform.position - transform.position).magnitude;

        //monster is in hearing range
        if (distanceToMonster <= range)
        {
            //send the monster to a random position within a circle that gets bigger the further away this sound is from the monster
            float scale = _minPrecisionAtMaxRange / range;

            _monster.HearAudio(MathFunctions.RandomPositionInCircle(transform.position, distanceToMonster * scale));
        }
    }
}
