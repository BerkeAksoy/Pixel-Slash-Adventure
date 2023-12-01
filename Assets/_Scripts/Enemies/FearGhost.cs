using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class FearGhost : Enemy
    {

        private bool canFear = false, appeared = false;

        private void Start()
        {
            componentGetter();
            canSeeX = 6f;
            canSeeY = 6f;
            canAttackX = 4f;
            canAttackY = 4f;
        }

        private void Update()
        {
            if (Mathf.Abs(getXDistanceToPlayer()) < canAttackX && Mathf.Abs(getYDistanceToPlayer()) < canAttackY && !appeared)
            {
                appeared = true;
                animator.SetTrigger("Appear");
            }

            if (canFear)
            {
                checkFearArea();
            }
        }

        private void checkFearArea()
        {
            if (Mathf.Abs(getXDistanceToPlayer()) < canAttackX && Mathf.Abs(getYDistanceToPlayer()) < canAttackY)
            {
                canFear = false;
                player.fear();
            }
        }

        private void death()
        {
            Destroy(gameObject);
        }

        private void isCanFearTrue()
        {
            canFear = true;
        }

        private void isCanFearFalse()
        {
            canFear = false;
        }

        protected override void OnTriggerExit2D(Collider2D collision)
        {

        }
    }
}