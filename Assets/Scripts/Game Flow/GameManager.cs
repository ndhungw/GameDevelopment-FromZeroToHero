using Assets.Scripts.Game_System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;
    [SerializeField]
    private GameObject playerDamageTextPrefab;
    [SerializeField]
    private GameObject enemyDamageTextPrefab;
    [SerializeField]
    private List<GameObject> playerPrefabs;
    
    
    [SerializeField]
    //Prefab clock to spawn on top of character when skill cooldown
    private GameObject cooldownClock;

    // Constssssssssss
    private const float characterSwitchCooldown = 2.0f;

    private float characterSwitchTimer = 0.0f; 

    public enum CHARACTERS
    {
        KNIGHT,
        WIZARD
    }

    // Numbers for the game
    public Dictionary<CHARACTERS, GameCharacter> numbersForCharacters { get; private set; }

    public Transform spawnPoint;
    private int? currentPlayer;
    private Dictionary<int, GameObject> playerFormation;

    private void Awake()
    {
        if (GM == null)
        {
            GM = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>(); 
        }
        if(playerPrefabs.Count > 0)
        {
            currentPlayer = 0;
        }
        playerFormation = new Dictionary<int, GameObject>();
        numbersForCharacters = new Dictionary<CHARACTERS, GameCharacter>();

        //We add in numbers
        numbersForCharacters.Add(CHARACTERS.KNIGHT, new Knight());
        numbersForCharacters.Add(CHARACTERS.WIZARD, new Wizard());

        //We add in formation slots
        playerFormation.Add(0, null);
        playerFormation.Add(1, null);
    }

    private void Start()
    {
        if (spawnPoint)
        {
            if (currentPlayer.HasValue)
            {
                bool spawnResult = spawnPlayerNumber(currentPlayer.Value, spawnPoint.position, null, 0);
            }
        }
    }

    private bool spawnPlayerNumber(int number, Vector3 position, Vector3? transformScale, int? playerPrefabToSpawn)
    {
        GameObject player; 
        if (!playerFormation.ContainsKey(number) || playerFormation[number] == null)
        {
            if(!playerPrefabToSpawn.HasValue || playerPrefabToSpawn.Value >= playerPrefabs.Count)
            {
                return false;
            }
            GameObject newPlayer = Instantiate(playerPrefabs[playerPrefabToSpawn.Value], position, new Quaternion());
            playerFormation[number] = newPlayer;
            player = newPlayer;
            currentPlayer = number;
        }
        else
        {
            player = playerFormation[number];
            CharacterScript script = player.GetComponent<CharacterScript>();
            if (!script || script.isCharacterActuallyDead())
            {
                return false;
            }
            player.transform.position = position;
            player.SetActive(true);
            currentPlayer = number;
        }
        if (transformScale.HasValue)
        {
            player.transform.localScale = transformScale.Value;
        }
        var cinemachineCamera = GameObject.FindGameObjectWithTag("CinemachineCamera");
        if (cinemachineCamera) {
            var cinemachineCameraScript = cinemachineCamera.GetComponent<CinemachineCameraScript>();
            if (cinemachineCameraScript)
            {
                cinemachineCameraScript.setFollowPlayer(player.transform);
            }
        }
        return true;
    }

    private void despawnPlayerNumber(int number)
    {
        if (playerFormation.ContainsKey(number))
        {
            GameObject player = playerFormation[number];
            var cinemachineCamera = GameObject.FindGameObjectWithTag("CinemachineCamera");
            if (cinemachineCamera)
            {
                var cinemachineCameraScript = cinemachineCamera.GetComponent<CinemachineCameraScript>();
                if (cinemachineCameraScript)
                {
                    cinemachineCameraScript.setFollowPlayer(null);
                }
            }
            player.SetActive(false);
        }
    }

    public void CreatePlayerDamageText(long damage, GameObject enemy)
    {
        GameObject playerDamage = Instantiate(playerDamageTextPrefab, new Vector3(enemy.GetComponent<Rigidbody2D>().position.x, enemy.GetComponent<Collider2D>().bounds.max.y, 0), new Quaternion());
        playerDamage.transform.GetChild(0).GetComponent<TextMeshPro>().SetText(damage.ToString());
    }
    public void CreateEnemyDamageText(long damage, GameObject player)
    {
        GameObject enemyDamage = Instantiate(enemyDamageTextPrefab, new Vector3(player.GetComponent<Rigidbody2D>().position.x, player.GetComponent<Collider2D>().bounds.max.y, 0), new Quaternion());
        enemyDamage.transform.GetChild(0).GetComponent<TextMeshPro>().SetText(damage.ToString());
    }

    private void Update()
    {
        if (characterSwitchTimer <= 0) {
            if(currentPlayer.HasValue && playerFormation.ContainsKey(currentPlayer.Value) && playerFormation[currentPlayer.Value] != null)
            {
                //Check current player dead
                GameObject player = playerFormation[currentPlayer.Value];

                CharacterScript characterScript = player.GetComponent<CharacterScript>();

                if(!characterScript || characterScript.isCharacterActuallyDead())
                {
                    // He's dead
                    Vector3 currentPosition = player.transform.position;
                    Vector3 currentLocalScale = player.transform.localScale;

                    int? newPlayer = null;
                    foreach(var item in playerFormation)
                    {
                        if(item.Key != currentPlayer.Value)
                        {
                            GameObject newChar = item.Value;
                            if (newChar)
                            {
                                CharacterScript newCharScript = newChar.GetComponent<CharacterScript>();

                                if (newCharScript && !newCharScript.isCharacterActuallyDead())
                                {
                                    newPlayer = item.Key;
                                }
                            } else
                            {
                                newPlayer = item.Key;
                            }
                        }
                    }
                    if (newPlayer.HasValue)
                    {
                        spawnPlayerNumber(newPlayer.Value, currentPosition, currentLocalScale, newPlayer.Value);
                        characterSwitchTimer = characterSwitchCooldown;
                    }
                    else
                    {
                        Debug.Log("Everyone is dead");
                        currentPlayer = newPlayer;
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                doChangeCharacter(0);
            } else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                doChangeCharacter(1);
            }          
        }
        else
        {
            characterSwitchTimer -= Time.deltaTime;
        }
    }

    private void doChangeCharacter(int number)
    {
        if (currentPlayer.HasValue && currentPlayer.Value != number && playerFormation.ContainsKey(currentPlayer.Value))
        {
            GameObject player = playerFormation[currentPlayer.Value];

            CharacterScript script = player.GetComponent<CharacterScript>();

            // if the player we switch to is dead, then we stop prematurely
            if (playerFormation.ContainsKey(number) && playerFormation[number] != null)
            {
                GameObject switchToPlayer = playerFormation[number];
                CharacterScript newPlayerScript = switchToPlayer.GetComponent<CharacterScript>();
                if (!newPlayerScript || newPlayerScript.isCharacterActuallyDead())
                {
                    return;
                }
            }

            // otherwise carry on
            if (script && !script.isCharacterActuallyDead())
            {
                script.startIFraming();

                //Get current position despawn and spawn new character
                Vector3 currentPosition = player.transform.position;
                Vector3 currentScale = player.transform.localScale;
                despawnPlayerNumber(currentPlayer.Value);

                script.endIFraming();

                spawnPlayerNumber(number, currentPosition, currentScale, number);
                characterSwitchTimer = characterSwitchCooldown;
            }
        }
    }

    public GameObject getCooldownClockPrefab()
    {
        return cooldownClock;
    }
}
