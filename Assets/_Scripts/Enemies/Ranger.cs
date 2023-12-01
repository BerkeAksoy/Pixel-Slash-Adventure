using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class Ranger : Enemy
    {
        // Start is called before the first frame update
        void Start()
        {
            componentGetter();
            canSeeX = 12f;
            canAttackX = 6f;
        }

        // Update is called once per frame
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
            //changeDirection(collision);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //jump(collision);
        }

        private void wait()
        {
            //if (!inCombat && Mathf.Abs(distanceX) < canSeeX && Mathf.Abs(distanceY) < canSeeY || isHit == true)
            {
                if (isHit)
                {
                    // if (distanceX > 0 != isFacingLeft())
                    {
                        transform.localScale = new Vector2(-transform.localScale.x, 1f);
                        Caster caster = GetComponentInChildren<Caster>();
                        if (caster != null)
                        {
                            caster.transform.localScale = transform.localScale;
                        }
                    }
                }

                // Kovala
            }
        }
    }
}