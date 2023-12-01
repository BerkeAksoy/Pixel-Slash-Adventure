using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class AxeDemon : Enemy
    {

        void Start()
        {
            componentGetter();
            newCombatTime = 1f;
            timeToDestroy = 1.5f;
            canSeeX = 10f;
            canSeeY = 2f;
            canAttackX = 1.5f;
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

        }
    }
}