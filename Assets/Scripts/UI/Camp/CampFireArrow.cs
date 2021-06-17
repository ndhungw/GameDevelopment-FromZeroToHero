using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFireArrow : MonoBehaviour
{
    public GameObject Camp;

    
    private void Update()
    {
        transform.position = new Vector3(Camp.transform.position.x, Camp.GetComponent<BoxCollider2D>().bounds.max.y + GetComponent<BoxCollider2D>().bounds.extents.y, 0);   
    }
}
