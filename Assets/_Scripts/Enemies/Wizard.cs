using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class Wizard : Enemy
    {

        void Start()
        {
            componentGetter();
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