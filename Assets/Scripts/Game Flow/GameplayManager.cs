﻿using Assets.Scripts.Game_System;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager GM;
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
    private const float characterSwitchCooldown = 1.5f;

    private float characterSwitchTimer = 0.0f;
    private bool spawnedPlayer = false;

    public enum CHARACTERS
    {
        KNIGHT,
        WIZARD,
        ARCHER
    }

    // Numbers for the game
    public Dictionary<CHARACTERS, GameCharacter> numbersForCharacters { get; private set; }

    public Transform spawnPoint;
    private int? currentPlayer;
    // characters in inventory (gameobject = character values like currentHealth,... ; int the position of the character type in prefab list)
    private List<Tuple<GameObject, int>> characterInventory;
    // int here is the position of the character in formation, gameobject is the corresponding character id
    private Dictionary<int, int?> playerFormation;

    private void Awake()
    {
        if (GM == null)
        {
            var gameplayManagerObj = GameObject.FindGameObjectWithTag("GameController");
            GM = gameplayManagerObj.GetComponent<GameplayManager>();
            DontDestroyOnLoad(gameplayManagerObj);
        }
        
        characterInventory = new List<Tuple<GameObject, int>>();
        playerFormation = new Dictionary<int, int?>();
        numbersForCharacters = new Dictionary<CHARACTERS, GameCharacter>();

        //We add in numbers, the order have to be exactly the same as the character prefabs list
        // we will replace new operator when gameinfomanager is done and I can take values from it
        numbersForCharacters.Add(CHARACTERS.KNIGHT, new Knight());
        numbersForCharacters.Add(CHARACTERS.WIZARD, new Wizard());
        numbersForCharacters.Add(CHARACTERS.ARCHER, new Archer());

        // Lets say player have all characters in inventory, so we add all characters to inventory for easy initialization
        // null means the character have not been instantiated yet, when instantiating in spawnNewPlayer, we provide info to new gameobj later
        for(int i = 0; i < playerPrefabs.Count; i++)
        {
            characterInventory.Add(new Tuple<GameObject, int>(null, i));
        }

        //We add in formation slots, we now have 4 slots, for easy, we add the first 4 characters in our inventory 
        for(int i = 0; i < 4; i++)
        {
            if(i < characterInventory.Count)
            {
                // First i have value from 0-3 corresponding to the 4 slots
                playerFormation.Add(i, i);
                // Second i have value based on what character in inventory we have deployed
            }
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
    }

    private void SceneManager_sceneUnloaded(Scene arg0)
    {
        for (int i = 0; i < playerPrefabs.Count; i++)
        {
            characterInventory[i] = new Tuple<GameObject, int>(null, characterInventory[i].Item2);
        }
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        spawnedPlayer = false;
        spawnPoint = null;
    }

    private bool spawnPlayerNumber(int number, Vector3 position, Vector3? transformScale, int? playerIdInInventory)
    {
        GameObject player; 
        // First time initialization
        if (playerFormation.ContainsKey(number) && playerFormation[number].HasValue && !characterInventory[playerFormation[number].Value].Item1)
        {
            var character = characterInventory[playerIdInInventory.Value];
            
            GameObject newPlayer = Instantiate(playerPrefabs[character.Item2], position, new Quaternion());
            player = newPlayer;
            currentPlayer = number;
            // Change the record in inventory with new obj instantiated
            characterInventory[playerIdInInventory.Value] = new Tuple<GameObject, int>(newPlayer, character.Item2);
            playerFormation[number] = playerIdInInventory.Value;
        }
        else
        {
            if(!playerFormation.ContainsKey(number) || !playerFormation[number].HasValue)
            {
                return false;
            }
            player = characterInventory[playerFormation[number].Value].Item1;

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
            GameObject player = characterInventory[playerFormation[number].Value].Item1;
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

    private void checkCurrentPlayerDead_SpawnAnother()
    {
        //Check current player dead
        GameObject player = characterInventory[playerFormation[currentPlayer.Value].Value].Item1;

        CharacterScript characterScript = player.GetComponent<CharacterScript>();

        if (!characterScript || characterScript.isCharacterActuallyDead())
        {
            // He's dead
            Vector3 currentPosition = player.transform.position;
            Vector3 currentLocalScale = player.transform.localScale;

            int? newPlayer = null;
            foreach (var item in playerFormation)
            {
                if (item.Key != currentPlayer.Value)
                {
                    GameObject newChar = item.Value.HasValue ? characterInventory[item.Value.Value].Item1 : null;
                    if (newChar)
                    {
                        // If slot have character and character is already deployed more than 1 during game cycle
                        // (appeared on screen more than 1 time), we check if character is dead or not before allow switch
                        CharacterScript newCharScript = newChar.GetComponent<CharacterScript>();

                        if (newCharScript && !newCharScript.isCharacterActuallyDead())
                        {
                            newPlayer = item.Key;
                            break;
                        }
                    }
                    else
                    {
                        // If slot have character but character never deployed on screen (although still in formation)
                        // we allow switch right away
                        if (item.Value.HasValue)
                        {
                            newPlayer = item.Key;
                            break;
                        }
                    }
                }
            }
            if (newPlayer.HasValue)
            {
                int? playerIdInInventory = null;
                if (playerFormation.ContainsKey(newPlayer.Value))
                {
                    playerIdInInventory = playerFormation[newPlayer.Value];
                }
                spawnPlayerNumber(newPlayer.Value, currentPosition, currentLocalScale, playerIdInInventory);
                characterSwitchTimer = characterSwitchCooldown;
            }
            else
            {
                Debug.Log("Everyone is dead");
                currentPlayer = newPlayer;
            }
        }
    }

    private void CharacterSwitchMechanics()
    {
        if (currentPlayer.HasValue && playerFormation.ContainsKey(currentPlayer.Value) && playerFormation[currentPlayer.Value] != null)
        {
            checkCurrentPlayerDead_SpawnAnother();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // only change character if there is a slot and that slot has a character assigned
            if (playerFormation.ContainsKey(0) && playerFormation[0].HasValue)
            {
                doChangeCharacter(0, playerFormation[0]);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // only change character if there is a slot and that slot has a character assigned
            if (playerFormation.ContainsKey(1) && playerFormation[1].HasValue)
            {
                doChangeCharacter(1, playerFormation[1]);
            }
        }else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // only change character if there is a slot and that slot has a character assigned
            if (playerFormation.ContainsKey(2) && playerFormation[2].HasValue)
            {
                doChangeCharacter(2, playerFormation[2]);
            }
        }
    }

    private void Update()
    {
        // spawn point prefabs only get dragged in in game level scenes not the UIs scenes, so when a game level ends, 
        // a spawn point destroy itself and it will automatically updates the game manager to remove itself => character not spawning on UI Scenes
        if (spawnPoint)
        {
            if (!spawnedPlayer)
            {
                if (currentPlayer.HasValue)
                {
                    spawnPlayerNumber(currentPlayer.Value, spawnPoint.position, null, playerFormation[currentPlayer.Value]); 
                }
                // if team have no current player we spawn into slot 1 the first character in inventory
                else
                {
                    spawnPlayerNumber(0, spawnPoint.position, null, 0);
                }
                spawnedPlayer = true;
            }
            if (characterSwitchTimer <= 0)
            {
                CharacterSwitchMechanics();
            }
            else
            {
                characterSwitchTimer -= Time.deltaTime;
            }
        }
    }

    private void doChangeCharacter(int number, int? playerIdInInventory)
    {
        if (currentPlayer.HasValue && currentPlayer.Value != number && playerFormation.ContainsKey(currentPlayer.Value) 
            && playerFormation[currentPlayer.Value].HasValue)
        {
            GameObject player = characterInventory[playerFormation[currentPlayer.Value].Value].Item1;

            CharacterScript script = player.GetComponent<CharacterScript>();

            // if the player we switch to is dead, then we stop prematurely
            if (playerFormation.ContainsKey(number) && playerFormation[number].HasValue)
            {
                GameObject switchToPlayer = characterInventory[playerFormation[number].Value].Item1;
                if (switchToPlayer)
                {
                    CharacterScript newPlayerScript = switchToPlayer.GetComponent<CharacterScript>();
                    if (!newPlayerScript || newPlayerScript.isCharacterActuallyDead())
                    {
                        return;
                    }
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

                spawnPlayerNumber(number, currentPosition, currentScale, playerIdInInventory);
                characterSwitchTimer = characterSwitchCooldown;
            }
        }
    }

    public GameObject getCooldownClockPrefab()
    {
        return cooldownClock;
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
}