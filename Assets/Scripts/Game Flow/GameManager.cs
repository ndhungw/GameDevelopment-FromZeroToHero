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
    }

    private void Start()
    {
        if (spawnPoint)
        {
            if (currentPlayer.HasValue)
            {
                bool spawnResult = spawnPlayerNumber(currentPlayer.Value);
            }
        }
    }

    private bool spawnPlayerNumber(int number)
    {
        GameObject player; 
        if (!playerFormation.ContainsKey(number))
        {
            GameObject newPlayer = Instantiate(playerPrefabs[currentPlayer.Value], spawnPoint.position, new Quaternion());
            Debug.Log(newPlayer.transform.position);
            playerFormation[playerFormation.Count] = newPlayer;
            player = newPlayer;
        }
        else
        {
            player = playerFormation[number];
            CharacterScript script = player.GetComponent<CharacterScript>();
            if (!script || script.GetHealth() <= 0)
            {
                return false;
            }
            player.transform.position = spawnPoint.position;
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
        
    }
}
