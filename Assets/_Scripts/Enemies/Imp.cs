using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class Imp : Enemy
    {

        void Start()
        {
            componentGetter();
            animator.SetBool("isMoving", true);
            canAttackX = 1f;
            canAttackY = 1f;
            newCombatTime = 0.66f;
            idleWaitTime = 1f;
            timeToDestroy = 3f;
            health = 10;
            maxHealth = 10;
        }

        void Update()
        {
            calculateMovement();
            if (isAlive)
            {
                checkDistance();
            }
        }

        protected override void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.CompareTag("Sword"))
            {
                changeDirection(collision);
            }
        }
    }
}