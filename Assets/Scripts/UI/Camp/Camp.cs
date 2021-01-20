using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camp : MonoBehaviour
{
    public LayerMask PlayerLayer;
    public float PickUpRadius = 5.0f;
    public GameObject CampUI;

    public bool isUsed = false;

    // Update is called once per frame
    void Update()
    {
        if (isUsed == false)
        {
            Collider2D collider = Physics2D.OverlapCircle(transform.position, PickUpRadius, PlayerLayer);

            if (collider != null)
            {
                openCampUI();
                isUsed = true;
            }
        }
        
    }

    private void openCampUI()
    {
        if (!CampUI.activeInHierarchy)
        {
            CampUI.SetActive(true);
        }
    }
}
