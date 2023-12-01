using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class HellBeast : Enemy
    {

        void Start()
        {
            componentGetter();
            idleWaitTime = 1.5f;
            newCombatTime = 1f;
            timeToDestroy = 1.38f;
            animator.SetBool("isMoving", true);
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
    }
}