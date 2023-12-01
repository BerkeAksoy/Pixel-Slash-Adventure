using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class HellHound : Enemy
    {

        void Start()
        {
            componentGetter();
            canAttackX = 1.2f;
            canAttackY = 0.5f;
            canSeeY = 1.2f;
            newCombatTime = 0.63f;
            idleWaitTime = 0f;
            timeToDestroy = 2f;
            health = 10;
            maxHealth = 10;
        }

        void Update()
        {
            if (isAlive)
            {
                checkDistance();
            }
            calculateMovement();
        }

        protected override void OnTriggerExit2D(Collider2D collision)
        {
            changeDirection(collision);
        }

        protected override void changeDirection(Collider2D collision)
        {
            if (collision.CompareTag("Projectile") || collision.CompareTag("Player") || collision.CompareTag("Bridge"))
            {

            }
            else if (collision != null)
            {
                flipSprite();
            }
        }


    }
}