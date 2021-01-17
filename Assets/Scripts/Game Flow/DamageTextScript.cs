using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextScript : MonoBehaviour
{
    public void DestroyDamageTextOnAnimationEnd()
    {
        var parent = gameObject.transform.parent.gameObject;
        Destroy(parent);
    }
}
