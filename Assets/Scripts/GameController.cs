using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    enum Game_States { INTRO, TUTORIAL, GAMEPLAY, DEATH, ENDING };

    [SerializeField]
    GameObject _gameOverCanvas;

    AudioSource _audioSource;

    PlayerController _player;
    CharacterController_Monster _monster;

    const string _animatorState_monsterDeathAnim1 = "DeathAnim1";
    const string _animatorState_monsterDeathAnim2 = "DeathAnim2";
    const string _animatorState_monsterDeathAnim3 = "DeathAnim3";

    // Start is called before the first frame update
    void Start()
    {
        _player = FindObjectOfType<PlayerController>();
        _monster = FindObjectOfType<CharacterController_Monster>(true);

        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //ToDo: This is very jank rn, but maybe just enough for the game jam
    public IEnumerator DeathAnimation() {
        #region setup
        //disable all controller scripts
        _player.enabled = false;
        _monster.enabled = false;

        //disable colliders
        foreach (Collider col in _player.GetComponents<Collider>())
            col.enabled = false;
        foreach (Collider col in _monster.GetComponents<Collider>())
            col.enabled = false;

        //disable Rigidbodies
        _player.GetComponent<Rigidbody>().isKinematic = true;
        _monster.GetComponent<Rigidbody>().isKinematic = true;

        _monster.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        #endregion

        #region turn player towards monster
        //turn player towards monster
        float t = 0;
        Quaternion playerStartRotation = Camera.main.transform.rotation;
        Vector3 playerMonsterSameHeight = new Vector3(Camera.main.transform.position.x, _monster.transform.position.y, Camera.main.transform.position.z);
        Quaternion playerGoalRotation = Quaternion.LookRotation(_monster.transform.position - playerMonsterSameHeight);

        Quaternion monsterStartRotation = _monster.transform.rotation;
        Quaternion monsterGoalRotation = Quaternion.LookRotation(playerMonsterSameHeight - _monster.transform.position);

        float perFrameDistance = 5;

        while (t < 1) {
            Camera.main.transform.rotation = Quaternion.Lerp(playerStartRotation, playerGoalRotation, t);

            _monster.transform.rotation = Quaternion.Lerp(monsterStartRotation, monsterGoalRotation, t);
            t += Time.deltaTime * perFrameDistance;    //ToDo: set to proper timescale
            yield return 0;
        }
        #endregion

        //monster punching
        Animator monsterAnimator = _monster.GetComponent<Animator>();
        monsterAnimator.SetTrigger(_animatorState_monsterDeathAnim1);

        //got this number from testing. VERY unclean
        yield return new WaitForSeconds(1.3f);

        #region player falling and flashlight turn off
        //player falling
        //also flashlight turns off
        Camera.main.GetComponentInChildren<Light>().enabled = false;

        perFrameDistance = 4;
        t = 0;
        Vector3 playerStartPosition = Camera.main.transform.position;
        Vector3 playerGoalPosition = new Vector3(playerStartPosition.x, .25f, playerStartPosition.z);

        playerStartRotation = Camera.main.transform.rotation;
        playerGoalRotation = Quaternion.Euler(45, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);

        while (t < 1)
        {
            Camera.main.transform.position = Vector3.Lerp(playerStartPosition, playerGoalPosition, t);
            Camera.main.transform.rotation = Quaternion.Lerp(playerStartRotation, playerGoalRotation, t);
            t += Time.deltaTime * perFrameDistance;    //ToDo: set to proper timescale
            yield return 0;
        }
        #endregion

        #region get up and look at monster
        //getting up and looking at monster
        t = 0;
        playerStartPosition = Camera.main.transform.position;
        playerGoalPosition = new Vector3(playerStartPosition.x, .5f, playerStartPosition.z);

        playerStartRotation = Camera.main.transform.rotation;
        playerGoalRotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);

        perFrameDistance = 3;

        while (t < 1)
        {
            Camera.main.transform.position = Vector3.Lerp(playerStartPosition, playerGoalPosition, t);
            Camera.main.transform.rotation = Quaternion.Lerp(playerStartRotation, playerGoalRotation, t);
            t += Time.deltaTime * perFrameDistance;    //ToDo: set to proper timescale
            yield return 0;
        }
        #endregion

        #region monster crawling and player falling prone
        perFrameDistance = 1;
        t = 0;
        playerStartPosition = Camera.main.transform.position;
        playerGoalPosition = new Vector3(playerStartPosition.x, .025f, playerStartPosition.z);

        playerStartRotation = Camera.main.transform.rotation;
        playerGoalRotation = Quaternion.Euler(-90, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);

        Vector3 monsterStartPosition = _monster.transform.position;
        Vector3 monsterGoalPosition = Camera.main.transform.position + _monster.transform.TransformDirection(new Vector3(-.32f, 0, -.31f));
        monsterGoalPosition.y = 0;

        //Start animation here
        monsterAnimator.SetTrigger(_animatorState_monsterDeathAnim2);

        while (t < 1)
        {
            //jank. To have the player look up quicker than the monster crawl towards it
            Camera.main.transform.position = Vector3.Lerp(playerStartPosition, playerGoalPosition, t*2);
            Camera.main.transform.rotation = Quaternion.Lerp(playerStartRotation, playerGoalRotation, t*2);

            _monster.transform.position = Vector3.Lerp(monsterStartPosition, monsterGoalPosition, t);

            t += Time.deltaTime * perFrameDistance;    //ToDo: set to proper timescale
            yield return 0;
        }
        #endregion

        yield return new WaitForSeconds(.1f);

        monsterAnimator.SetTrigger(_animatorState_monsterDeathAnim3);

        yield return new WaitForSeconds(.4f);

        _gameOverCanvas.SetActive(true);
    }


    /// <summary>
    /// Doesn't stop other clips from playing
    /// </summary>
    /// <param name="clip"></param>
    public void PlayGlobalOneshotAudio(AudioClip clip) {
        _audioSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Stops other audio clips from playing
    /// </summary>
    /// <param name="clip"></param>
    public void PlayGlobalAudio(AudioClip clip)
    {
        _audioSource.Stop();
        _audioSource.clip = clip;
        _audioSource.Play();
    }
}
