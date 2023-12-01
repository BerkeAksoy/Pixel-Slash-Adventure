using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class EyeDemon : Enemy
    {
        void Start()
        {
            componentGetter();
            timeToDestroy = 0.5f;
            StartCoroutine(startAttack());
        }

        void Update()
        {
            calculateMovement();
        }

        protected override void OnTriggerExit2D(Collider2D collision)
        {
            changeDirection(collision);
        }

        IEnumerator startAttack()
        {
            while (isAlive)
            {
                yield return new WaitForSeconds(2f);
                attack();
            }
        }

        protected override void changeDirection(Collider2D collision)
        {
            if (collision.CompareTag("Projectile") || collision.CompareTag("Player"))
            {

            }
            else if (collision != null)
            {
                flipSprite();
            }
        }

        public override void takeDamage(int value)
        {
            if (value == -1)
            {
                health -= health;
                isAlive = false;
                StopAllCoroutines();
                animator.SetTrigger("Die");
                movementSpeed = 0;
                Destroy(gameObject, timeToDestroy);
            }

            if (!isHit && health > 0)
            {
                health -= value;
                if (!myHB.activeInHierarchy && myHB != null)
                {
                    myHB.SetActive(true);
                }

                myHB.GetComponent<HealthBar>().updateHealth(this);
                StartCoroutine(bloodParticle());

                if (health > 0)
                {
                    isHit = true;
                    StartCoroutine(refreshIsHit());
                }
            }

            if (health <= 0 && isAlive)
            {
                if (GetComponent<Dropper>())
                {
                    GetComponent<Dropper>().dropItem();
                    GetComponent<Dropper>().dropCoins();
                }
                isAlive = false;
                if (myHB != null)
                {
                    myHB.SetActive(false);
                }
                player.stats.addXP(XPValue);
                StopAllCoroutines();
                animator.SetTrigger("Die");
                movementSpeed = 0;
                GetComponent<CapsuleCollider2D>().enabled = false;
                GetComponent<BoxCollider2D>().enabled = false;
                myRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
                Destroy(gameObject, timeToDestroy);
            }
        }
    }
}