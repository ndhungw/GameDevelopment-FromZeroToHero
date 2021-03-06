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

    // int is the slot, 
    // this array have to have to same type order as the prefab array
    Dictionary<int, Type> characterInfos;

    public Transform spawnPoint;
    private int? currentPlayer;
    // characters in inventory (gameobject = character values like currentHealth,... ; int the position of the character type in prefab list,
    // this prefab list order is also the same order for the type dictionary)
    private List<Tuple<GameObject, int>> characterInventory;
    // int here is the position of the character in formation, gameobject is the corresponding character id
    //0: knight; 1:wizard; 2:archer
    /// <summary>
    /// key: 0 - knight; 1 - wizard; 2 - archer
    /// item: position of hero in characterInventory
    /// </summary>
    private Dictionary<int, int?> playerFormation;

    private void Awake()
    {
        if (GM == null)
        {
            var gameplayManagerObj = GameObject.FindGameObjectWithTag("GameController");
            GM = gameplayManagerObj.GetComponent<GameplayManager>();
            DontDestroyOnLoad(gameplayManagerObj);
        }

        characterInfos = new Dictionary<int, Type>();
        characterInventory = new List<Tuple<GameObject, int>>();
        playerFormation = new Dictionary<int, int?>();

        //Add in types relating to the prefabs
        characterInfos.Add(0, typeof(Knight));
        characterInfos.Add(1, typeof(Wizard));
        characterInfos.Add(2, typeof(Archer));

        // Lets say player have all characters in inventory, so we add all characters to inventory for easy initialization
        // null means the character have not been instantiated yet, when instantiating in spawnNewPlayer, we provide info to new gameobj later
        for (int i = 0; i < characterInfos.Count; i++)
        {
            characterInventory.Add(new Tuple<GameObject, int>(null, i));
        }

        // Set current player to the first in inventory
        if (characterInventory.Count > 0)
        {
            currentPlayer = 0;
        }

        //We add in formation slots, we now have 4 slots, for easy, we add the first 4 characters in our inventory 
        for (int i = 0; i < 4; i++)
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
        currentPlayer = 0;
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
            CharacterScript script = player.GetComponent<CharacterScript>();
            if (!script)
            {
                return false;
            }
            currentPlayer = number;
            // Change the record in inventory with new obj instantiated
            characterInventory[playerIdInInventory.Value] = new Tuple<GameObject, int>(newPlayer, character.Item2);
            playerFormation[number] = playerIdInInventory.Value;
        }
        else
        {   
            if (!playerFormation.ContainsKey(number) || !playerFormation[number].HasValue)
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

        if (player)
        {
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
                            var character = characterInventory[item.Key];
                            var characterType = characterInfos[character.Item2];
                            if (characterType == null)
                            {
                                characterInventory.Remove(characterInventory[item.Key]);
                                playerFormation[item.Key] = null;
                                return;
                            }
                            if (characterType == typeof(Knight))
                            {
                                if (GameInfoManager.knight.CurrentHealth <= 0)
                                {
                                    return;
                                }
                            }
                            else if (characterType == typeof(Archer))
                            {
                                if (GameInfoManager.archer.CurrentHealth <= 0)
                                {
                                    return;
                                }
                            }
                            else if (characterType == typeof(Wizard))
                            {
                                if (GameInfoManager.wizard.CurrentHealth <= 0)
                                {
                                    return;
                                }
                            }
                            else
                            {
                                characterInventory.Remove(characterInventory[item.Key]);
                                playerFormation[item.Key] = null;
                                return;
                            }
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
                    var characterPosInInventory = playerFormation[currentPlayer.Value];
                    // if we cant find any record relating currentplayer to a character in inventory, we find a new current player
                    if (!characterPosInInventory.HasValue || characterPosInInventory.Value < 0 || characterPosInInventory >= characterInventory.Count)
                    {
                        if (characterPosInInventory.Value < 0 || characterPosInInventory >= characterInventory.Count)
                        {
                            playerFormation[currentPlayer.Value] = null;
                        }
                        int? newCurrentPlayer = null;
                        foreach (var item in playerFormation)
                        {
                            if (item.Key != currentPlayer.Value && item.Value.HasValue)
                            {
                                newCurrentPlayer = item.Value;
                                break;
                            }
                        }
                        currentPlayer = newCurrentPlayer;
                        //Found no new currentplayer, party is empty!!!
                        if (currentPlayer == null)
                        {
                            Debug.Log("Your party is empty");
                        }
                    }
                    // this part is important, we have to connect to game info manager to get currentPlayer's health first
                    // if current player health is lower than 0, we change
                    else
                    {
                        var trueCharacter = characterInventory[characterPosInInventory.Value];
                        Type characterType = characterInfos[trueCharacter.Item2];
                        GameCharacter currentCharInfo = null;
                        if(characterType == typeof(Knight))
                        {
                            currentCharInfo = GameInfoManager.knight;
                        }else if (characterType == typeof(Wizard))
                        {
                            currentCharInfo = GameInfoManager.wizard;
                        }else if (characterType == typeof(Archer)){
                            currentCharInfo = GameInfoManager.archer;
                        }

                        // No char info of such type
                        if (currentCharInfo == null)
                        {
                            // Remove char from inventory and set formation ref of that char to null
                            characterInventory.Remove(characterInventory[characterPosInInventory.Value]);
                            playerFormation[currentPlayer.Value] = null;
                        }
                        else
                        {
                            //Check the health of the char
                            if (currentCharInfo.CurrentHealth <= 0)
                            {
                                int? replace = null;
                                foreach (var item in playerFormation)
                                {
                                    if (item.Key != currentPlayer.Value && item.Value.HasValue)
                                    {
                                        // Find replacement character for this one in the formation
                                        var replacementCharacter = characterInventory[item.Value.Value];
                                        Type replaceCharType = characterInfos[replacementCharacter.Item2];
                                        GameCharacter replaceCharInfo = null;
                                        if (replaceCharType == typeof(Knight))
                                        {
                                            replaceCharInfo = GameInfoManager.knight;
                                        }
                                        else if (replaceCharType == typeof(Wizard))
                                        {
                                            replaceCharInfo = GameInfoManager.wizard;
                                        }
                                        else if (replaceCharType == typeof(Archer))
                                        {
                                            replaceCharInfo = GameInfoManager.archer;
                                        }

                                        // Replace char have no info, this is an error
                                        if(replaceCharInfo == null)
                                        {
                                            // Remove char from inventory and set formation ref of that char to null
                                            characterInventory.Remove(characterInventory[item.Value.Value]);
                                            playerFormation[item.Key] = null;
                                        }
                                        else
                                        {
                                            if (replaceCharInfo.CurrentHealth > 0)
                                            {
                                                replace = item.Key;
                                                break;
                                            }
                                        }
                                    }
                                }
                                currentPlayer = replace;
                                //Found no new currentplayer, party have no suitable replacement, they are all dead!!!
                                if (currentPlayer == null)
                                {
                                    Debug.Log("Your party is all dead before going in this room");
                                }
                            }
                            else
                            {
                                //Spawn 
                                bool spawnResult = spawnPlayerNumber(currentPlayer.Value, spawnPoint.position, null, playerFormation[currentPlayer.Value]);
                                if (spawnResult)
                                {
                                    spawnedPlayer = true;
                                }
                            }  
                        }
                    }
                }
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
            if (script && !script.isCharacterActuallyDead() && script.isAbleToClickAttack())
            {
                if (playerFormation.ContainsKey(number) && playerFormation[number].HasValue && !characterInventory[playerFormation[number].Value].Item1)
                {
                    if (!playerIdInInventory.HasValue) return;
                    var character = characterInventory[number];
                    var characterType = characterInfos[character.Item2];
                    if (characterType == null)
                    {
                        characterInventory.Remove(characterInventory[playerIdInInventory.Value]);
                        playerFormation[number] = null;
                        return;
                    }
                    if (characterType == typeof(Knight))
                    {
                        if (GameInfoManager.knight.CurrentHealth <= 0)
                        {
                            return;
                        }
                    }
                    else if (characterType == typeof(Archer))
                    {
                        if (GameInfoManager.archer.CurrentHealth <= 0)
                        {
                            return;
                        }
                    }
                    else if (characterType == typeof(Wizard))
                    {
                        if (GameInfoManager.wizard.CurrentHealth <= 0)
                        {
                            return;
                        }
                    }
                    else
                    {
                        characterInventory.Remove(characterInventory[playerIdInInventory.Value]);
                        playerFormation[number] = null;
                        return;
                    }
                }

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

    public List<Tuple<GameObject, Type>> GetCurrentCharacterScripts()
    {
        List<Tuple<GameObject, Type>> result = new List<Tuple<GameObject, Type>>();

        foreach(var e in characterInventory)
        {
            GameObject gameObject = e.Item1;
            Type type = characterInfos[e.Item2];

            result.Add(new Tuple<GameObject, Type>(gameObject, type));
        }

        return result;
        
    }
}
