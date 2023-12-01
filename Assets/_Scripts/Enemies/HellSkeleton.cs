using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class HellSkeleton : Enemy
    {

        private bool borned = true;

        private void Start()
        {
            componentGetter();
            canAttackX = 1.2f;
            canAttackY = 1f;
            newCombatTime = 0.75f;
            idleWaitTime = 1.5f;
            timeToDestroy = 1f;
            health = 10;
            maxHealth = 10;
            //StartCoroutine(waitToBorn());
        }

        void Update()
        {
            if (borned)
            {
                if (isAlive)
                {
                    checkDistance();
                }
                calculateMovement();
            }
        }

        IEnumerator waitToBorn()
        {
            yield return new WaitForSeconds(4f);
            borned = true;
        }

        protected override void OnTriggerExit2D(Collider2D collision)
        {

        }


    }
}