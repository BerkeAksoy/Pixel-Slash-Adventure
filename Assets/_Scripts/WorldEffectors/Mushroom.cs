using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class Mushroom : MonoBehaviour
    {

        private Animator animator;
        private bool isTouched;
        private Player player;

        private void Start()
        {
            animator = GetComponent<Animator>();
            isTouched = false;
            player = GameObject.Find("Player").GetComponent<Player>();
        }

        IEnumerator reIsTouched()
        {
            yield return new WaitForSeconds(0.1f);
            isTouched = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && !isTouched)
            {
                isTouched = true;
                StartCoroutine(reIsTouched());
                Vector2 bouncerVelocity = new Vector2(0, 800);

                // TODO //Vector2 velocity = new Vector2(player.getRigidbody2D().velocity.x, 20);
                //player.getRigidbody2D().AddForce(bouncerVelocity);
                //if(player.getRigidbody2D().velocity.y)
                // TODO player.getRigidbody2D().velocity = velocity;
                animator.SetTrigger("Bounce");
            }
        }
    }
}