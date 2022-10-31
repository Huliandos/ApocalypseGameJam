using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class GameController : MonoBehaviour
{
    AudioClip _tutorialSound;

    enum GameStates { INTRO, TUTORIAL, GAMEPLAY, DEATH, ENDING };
    GameStates _myGameState = GameStates.INTRO;

    [SerializeField]
    GameObject _gameOverCanvas, _victoryCanvas;

    [SerializeField]
    AudioClip _monsterHitAudio, _playerFallAudio, _monsterCrawlAudio, _monsterEatAudio;

    AudioSource _audioSource;

    PlayerController _player;
    CharacterController_Monster _monster;

    SpawnMonsterTrigger _lastSpawnTrigger;

    const string _animatorState_monsterDeathAnim1 = "DeathAnim1";
    const string _animatorState_monsterDeathAnim2 = "DeathAnim2";
    const string _animatorState_monsterDeathAnim3 = "DeathAnim3";

    // Start is called before the first frame update
    void Start()
    {
        _player = FindObjectOfType<PlayerController>();
        _monster = FindObjectOfType<CharacterController_Monster>(true);

        _audioSource = GetComponent<AudioSource>();

        PlayGlobalAudio(_tutorialSound);
    }

    // Update is called once per frame
    void Update()
    {
        switch (_myGameState) {
            //allow restarting or quitting the game here
            case GameStates.ENDING:
                if (Input.GetKeyDown(KeyCode.R))
                    //ToDo: Change to the correct scene here
                    SceneManager.LoadScene("UniCellLabs_props");
                if (Input.GetKeyDown(KeyCode.Escape))
                    Application.Quit();
                break;
        }
    }

    //ToDo: This is very jank rn, but maybe just enough for the game jam
    public IEnumerator DeathAnimation(bool withoutEnding = false) {
        #region setup
        _audioSource.Stop();

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
        Vector3 playerStartPosition = Camera.main.transform.position;
        Vector3 playerGoalPosition = new Vector3(playerStartPosition.x, 1.5f, playerStartPosition.z);

        Quaternion playerStartRotation = Camera.main.transform.rotation;
        Vector3 playerMonsterSameHeight = new Vector3(Camera.main.transform.position.x, _monster.transform.position.y, Camera.main.transform.position.z);
        Quaternion playerGoalRotation = Quaternion.LookRotation(_monster.transform.position - playerMonsterSameHeight);

        Quaternion monsterStartRotation = _monster.transform.rotation;
        Quaternion monsterGoalRotation = Quaternion.LookRotation(playerMonsterSameHeight - _monster.transform.position);

        float perFrameDistance = 5;

        while (t < 1)
        {
            Camera.main.transform.position = Vector3.Lerp(playerStartPosition, playerGoalPosition, t);
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
        yield return new WaitForSeconds(.5f);
        _audioSource.PlayOneShot(_monsterHitAudio);

        yield return new WaitForSeconds(.5f);

        #region player falling and flashlight turn off
        //player falling
        //also flashlight turns off
        Camera.main.GetComponentInChildren<Light>().enabled = false;

        perFrameDistance = 4;
        t = 0;
        playerStartPosition = Camera.main.transform.position;
        playerGoalPosition = new Vector3(playerStartPosition.x, .25f, playerStartPosition.z);

        playerStartRotation = Camera.main.transform.rotation;
        playerGoalRotation = Quaternion.Euler(45, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);

        while (t < 1)
        {
            Camera.main.transform.position = Vector3.Lerp(playerStartPosition, playerGoalPosition, t);
            Camera.main.transform.rotation = Quaternion.Lerp(playerStartRotation, playerGoalRotation, t);
            t += Time.deltaTime * perFrameDistance;    //ToDo: set to proper timescale
            yield return 0;
        }

        _audioSource.PlayOneShot(_playerFallAudio);
        #endregion

        #region get up and look at monster
        //getting up and looking at monster
        t = 0;
        playerStartPosition = Camera.main.transform.position;
        playerGoalPosition = new Vector3(playerStartPosition.x, .5f, playerStartPosition.z);

        playerStartRotation = Camera.main.transform.rotation;
        playerGoalRotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);

        perFrameDistance = 6;

        while (t < 1)
        {
            Camera.main.transform.position = Vector3.Lerp(playerStartPosition, playerGoalPosition, t);
            Camera.main.transform.rotation = Quaternion.Lerp(playerStartRotation, playerGoalRotation, t);
            t += Time.deltaTime * perFrameDistance;    //ToDo: set to proper timescale
            yield return 0;
        }
        #endregion

        #region monster crawling and player falling prone
        _audioSource.clip = _monsterCrawlAudio;
        _audioSource.Play();

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
        _audioSource.Stop();

        monsterAnimator.SetTrigger(_animatorState_monsterDeathAnim3);

        _audioSource.clip = _monsterEatAudio;
        _audioSource.Play();

        yield return new WaitForSeconds(.4f);

        if (!withoutEnding)
        {
            _gameOverCanvas.SetActive(true);

            _myGameState = GameStates.ENDING;
        }
        else
        {
            _victoryCanvas.SetActive(true);

            yield return new WaitForSeconds(4);

            SceneManager.LoadScene("EndingCutscene");
        }
    }

    public void VictoryAnimation() {
        //teleport monster behind player
        Vector3 playerPositionYZero = _player.transform.position, playerForwardYZero = Camera.main.transform.forward;
        playerPositionYZero.y = 0;
        playerForwardYZero.y = 0;

        _monster.transform.position = playerPositionYZero - playerForwardYZero.normalized;
        StartCoroutine(DeathAnimation(true));
    }

    public void SpawnMonster(SpawnMonsterTrigger spawnTrigger, Vector3 spawnLocation, Vector3 firstGotoLocation) {
        if (_lastSpawnTrigger != null)
            _lastSpawnTrigger.gameObject.SetActive(true);

        _lastSpawnTrigger = spawnTrigger;

        if (!_monster.gameObject.activeInHierarchy)
            _monster.gameObject.SetActive(true);

        _monster.GetComponent<NavMeshAgent>().Warp(spawnLocation);
        _monster.HearAudio(firstGotoLocation);
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
