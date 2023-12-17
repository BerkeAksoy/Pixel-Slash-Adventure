using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class Skeleton : Enemy
    {

        private bool inCorpse = false;
        public GameObject prefab;

        private void Start()
        {
            componentGetter();
            animator.SetBool("isMoving", true);
            canAttackX = 1.2f;
            canAttackY = 1f;
            newCombatTime = 0.75f;
            idleWaitTime = 1.5f;
            timeToDestroy = 0f;
            health = 5;
            maxHealth = 5;
        }

        void Update()
        {
            if (!inCorpse)
            {
                calculateMovement();
                if (isAlive)
                {
                    checkDistance();
                }
            }
        }

        protected override void OnTriggerExit2D(Collider2D collision)
        {
            changeDirection(collision);
        }

        public override void takeDamage(int value)
        {
            if (!isHit && health > 0 && !inCorpse)
            {
                health -= value;
                if (!healthBar.activeInHierarchy && healthBar != null)
                {
                    healthBar.SetActive(true);
                }

                healthBar.GetComponent<HealthBar>().updateHealth(this);

                if (health > 0)
                {
                    isHit = true;
                    animator.SetTrigger("isHit");
                    StartCoroutine(refreshIsHit());
                }
            }

            if (!isHit && inCorpse && health > 0)
            {
                health--;
                healthBar.GetComponent<HealthBar>().updateHealth(this);

                if (health > 0)
                {
                    isHit = true;
                    StartCoroutine(refreshIsHit());
                }
            }

            if (health <= 0 && isAlive && !inCorpse)
            {
                startInCorpse();
                health = 4;
                maxHealth = 3;
                healthBar.GetComponent<HealthBar>().updateHealth(this);

                animator.SetTrigger("Die");
                knockback();
            }

            if (health <= 0 && inCorpse)
            {
                isAlive = false;

                if (GetComponent<Dropper>())
                {
                    GetComponent<Dropper>().dropItem();
                    GetComponent<Dropper>().dropCoins();
                }

                if (healthBar != null)
                {
                    healthBar.SetActive(false);
                }
                
                StopAllCoroutines();

                if (GetComponent<CapsuleCollider2D>())
                {
                    GetComponent<CapsuleCollider2D>().enabled = false;
                }
                if (GetComponent<BoxCollider2D>())
                {
                    GetComponent<BoxCollider2D>().enabled = false;
                }

                myRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
                Destroy(gameObject, timeToDestroy);
            }
        }

        private void startInCorpse()
        {
            inCorpse = true;
            StartCoroutine(countDown());
        }

        private void knockback()
        {
            myRigidbody2D.AddForce(new Vector2(0, 300));

        }

        IEnumerator countDown()
        {
            yield return new WaitForSeconds(6f);
            Instantiate(prefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}