using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    public LayerMask PlayerLayer;
    public float PickUpRadius = 5.0f;
    public int TreasureValue = 100;

    //public GameObject WinPanel;

    public Sprite ClosedChest;

    public Sprite ChestOpen;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider2D collider = Physics2D.OverlapCircle(transform.position, PickUpRadius, PlayerLayer);

            if (collider != null)
            {
                OpenChest();
            }

            
        }
        
    }

    private void OpenChest()
    {
        spriteRenderer.sprite = ChestOpen;
        LevelManager.LM.TreasureFound = 100;
        LevelManager.LM.ShowVictoryPanel();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, PickUpRadius);
    }
}
