using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{

    private GameObject portalB, portalY, player;
    private Vector3 portalBPos, portalYPos;
    public Animator animator;

    private static bool teleported = false;


    void Start()
    {
        player = GameObject.Find("Player");


        animator = GetComponent<Animator>();
        portalB = GameObject.Find("Blue Portal");
        portalY = GameObject.Find("Yellow Portal");

        portalBPos = portalB.transform.position;
        portalYPos = portalY.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("hey");
        if (collision.tag.Equals("Player") && !teleported)
        {
            if (this.name.Equals("Blue Portal"))
            {
                Invoke("TeleportToY", 0.3f);
                animator.SetTrigger("teleport");
                teleported = true;
                Destroy(gameObject, 2f);
            }
            else
            {
                Invoke("TeleportToB", 0.3f);
                animator.SetTrigger("teleport");
                teleported = true;
                Destroy(gameObject, 1.8f);
            }
        }
    }

    private void TeleportToY()
    {
        player.transform.position = portalYPos;
    }

    private void TeleportToB()
    {
        player.transform.position = portalBPos;
    }

    IEnumerator CanTeleport()
    {
        yield return new WaitForSeconds(0.5f);
        teleported = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        teleported = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        StartCoroutine(CanTeleport());
    }
}
