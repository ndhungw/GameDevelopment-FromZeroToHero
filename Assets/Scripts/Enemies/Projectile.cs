using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rigidbody2d;
    // Start is called before the first frame update
    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.magnitude > 1000.0f)
        {
            Destroy(gameObject);
        }
    }

    public void Launch(Vector2 lookDirection, int v)
    {
        Debug.Log("Shot fired");
        rigidbody2d.AddForce(lookDirection * v);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject);
        CharacterScript character = collision.gameObject.GetComponent<CharacterScript>();
        if (character != null)
        {
            character.ChangeHealth(-10);
        }
        Destroy(gameObject);

    }
}
