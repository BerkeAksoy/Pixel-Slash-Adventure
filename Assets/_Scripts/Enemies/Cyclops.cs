using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class Cyclops : Enemy
    {

        private bool isHornActivated = false, isInBinoculars = false, isInHorn = false;
        private float canSeeBinocularsX = 10f, canSeeBinocularsY = 2f;
        [SerializeField]
        private GameObject[] enemyPrefabs = null;

        void Start()
        {
            componentGetter();
            animator.SetBool("isMoving", true);
            idleWaitTime = 1.9f;
            newCombatTime = 2.2f;
            canAttackX = 10f;
            canSeeY = 1f;
            health = 100;
            maxHealth = 100;
        }

        // Update is called once per frame
        void Update()
        {
            if (isAlive)
            {
                checkDistance();
                if (isHornActivated == false)
                {
                    checkWithBinoculars();
                }
            }
            calculateMovement();
        }

        protected void instantiateEnemyFriends()
        {
            if (GameObject.Find("PointA"))
            {
                Instantiate(enemyPrefabs[0], GameObject.Find("PointA").transform.position, Quaternion.identity);
            }

            if (GameObject.Find("PointB"))
            {
                Instantiate(enemyPrefabs[1], GameObject.Find("PointB").transform.position, Quaternion.identity);
            }

            if (GameObject.Find("PointC"))
            {
                Instantiate(enemyPrefabs[2], GameObject.Find("PointC").transform.position, Quaternion.identity);
            }

            if (GameObject.Find("PointD"))
            {
                Instantiate(enemyPrefabs[3], GameObject.Find("PointD").transform.position, Quaternion.identity);
            }
        }

        private void checkWithBinoculars()
        {
            float distanceY = getYDistanceToPlayer();
            float distanceX = getXDistanceToPlayer();

            if (isInBinoculars)
            {
                if (Mathf.Abs(distanceX) < canSeeBinocularsX && Mathf.Abs(distanceY) < canSeeBinocularsY) // Enemy can see player Dürbünlü
                {
                    isHornActivated = true;
                    isInHorn = true;
                    animator.SetTrigger("EnemySeen");
                    StartCoroutine(refreshIsInHorn());
                }
            }
        }

        protected override void checkDistance()
        {
            float distanceY = getYDistanceToPlayer();
            float distanceX = getXDistanceToPlayer();

            bool notBlocked = checkRayCast();


            if (isHornActivated == false && ((Mathf.Abs(distanceX) < canSeeX && Mathf.Abs(distanceY) < canSeeY && notBlocked) || isHit))
            {
                isHornActivated = true;
                isInHorn = true;
                animator.SetTrigger("EnemySeen");
                StartCoroutine(refreshIsInHorn());
            }
            else if (isHornActivated && ((Mathf.Abs(distanceX) < canAttackX && Mathf.Abs(distanceY) < canAttackY && notBlocked) || isHit))
            {
                if (distanceX > 0 != isFacingLeft()) // Set position to player
                {
                    flipSprite();

                    Caster caster = GetComponentInChildren<Caster>();
                    if (caster != null)
                    {
                        caster.transform.localScale = transform.localScale;
                    }
                }

                if (!inCombat && !isInHorn) // Enemy can attack
                {
                    inCombat = true;
                    animator.SetBool("inCombat", true);
                    animator.SetBool("isMoving", false);

                    StartCoroutine(refreshInCombat());
                }

                if (inCombat)
                {
                    if (Mathf.Abs(distanceX) < 1.3f)
                    {
                        animator.SetBool("isNear", true);
                    }
                    else
                    {
                        animator.SetBool("isNear", false);
                    }
                }
            }

        }

        protected override void OnTriggerExit2D(Collider2D collision)
        {
            changeDirection(collision);
        }

        protected override void changeDirection(Collider2D collision)
        {
            if (collision.tag != "Projectile" && collision.tag != "Player" && !isHit)
            {
                animator.SetBool("isMoving", false);
                //isInBinoculars = true;
                StartCoroutine(waitForIdle());
            }
        }

        protected override IEnumerator refreshInCombat()
        {
            yield return new WaitForSeconds(newCombatTime);
            animator.SetBool("inCombat", false);
            animator.SetBool("isMoving", true);
            animator.SetBool("isNear", false);
            inCombat = false;
        }

        private IEnumerator refreshIsInHorn() // Bu zaman diliminde combata giremez
        {
            yield return new WaitForSeconds(1.5f);
            isInHorn = false;
        }

        protected override IEnumerator waitForIdle()
        {
            yield return new WaitForSeconds(idleWaitTime);
            flipSprite();
            animator.SetBool("isMoving", true);
            isInBinoculars = false;
        }

        public void setIsInBinoculars(int value)
        {
            if (value == 1)
            {
                isInBinoculars = true;
            }
            else
            {
                isInBinoculars = false;
            }
        }
    }
}