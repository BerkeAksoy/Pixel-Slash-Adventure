using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretWall : MonoBehaviour
{

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if((collision.gameObject.tag == "Player" || collision.gameObject.tag == "Projectile") && GetComponent<Renderer>().enabled)
        {
            GetComponent<CompositeCollider2D>().enabled = false;
            GetComponent<Renderer>().enabled = false;
        }
    }


}
