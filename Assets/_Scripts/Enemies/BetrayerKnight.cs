using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class BetrayerKnight : Enemy
    {

        void Start()
        {
            componentGetter();
            animator.SetBool("isMoving", true);
            canAttackX = 1f;
            canAttackY = 0.5f;
            newCombatTime = 1.6f;
            idleWaitTime = 1.5f;
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
            changeDirection(collision);
        }

        public override void takeDamage(int value)
        {
            int dodgeChange = Random.Range(0, 101);

            if (dodgeChange <= 20 && !isHit)
            {
                animator.SetTrigger("Dodge");
                isHit = true;
                StartCoroutine(refreshIsHit());
            }
            else if (!isHit)
            {
                base.takeDamage(value);
            }
        }
    }
}