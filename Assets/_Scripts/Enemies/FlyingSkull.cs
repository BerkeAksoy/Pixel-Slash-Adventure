using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class FlyingSkull : Enemy, ITracer
    {

        private float rotateSpeed = 180f, mainSpeed = 2f;
        private bool canTrace = false;
        public int damage;

        void Start()
        {
            componentGetter();
            canSeeX = 20f;
            canSeeY = 20f;
            timeToDestroy = 0.5f;
        }


        void Update()
        {
            if (isAlive)
            {
                checkDistance();
                if (canTrace)
                {
                    trace();
                }
            }
        }

        protected override void OnTriggerExit2D(Collider2D collision)
        {

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            IDamagable hit = collision.GetComponent<IDamagable>();

            if (hit != null)
            {
                hit.takeDamage(damage);
                takeDamage(health);
            }
        }

        protected override void checkDistance()
        {
            float distanceY = getYDistanceToPlayer();
            float distanceX = getXDistanceToPlayer();

            if ((Mathf.Abs(distanceX) < canSeeX && Mathf.Abs(distanceY) < canSeeY) || isHit) // Enemy can see player
            {
                animator.SetBool("isMoving", true);
                canTrace = true;
            }
            else
            {
                animator.SetBool("isMoving", false);
                canTrace = false;
            }
        }

        public void trace()
        {
            Vector2 direction = (Vector2)player.transform.position - (Vector2)transform.position;

            direction.Normalize();

            float rotateAmount = Vector3.Cross(direction, transform.right).z;

            myRigidbody2D.velocity = -movementSpeed * transform.right;
            myRigidbody2D.angularVelocity = rotateAmount * rotateSpeed;

            if (getXDistanceToPlayer() > 0 == isFacingLeft() && transform.localScale.y > 0) // Player soldaysa ve enemy sola bakiyorsa ve enemynin localscale y'si 1 ise
            {
                // Don't change
            }
            else if (getXDistanceToPlayer() > 0 == isFacingLeft() && transform.localScale.y < 0) // Player soldaysa ve enemy sola bakiyorsa ve enemynin localscale y'si -1 ise
            {
                transform.localScale = new Vector2(1, 1); ;
            }
            else if (getXDistanceToPlayer() < 0 == isFacingLeft() && transform.localScale.y > 0) // Player sagdaysa ve enemy sola bakiyorsa ve enemynin localscale y'si 1 ise
            {
                transform.localScale = new Vector2(1, -1); ;
            }
            else if (getXDistanceToPlayer() < 0 == isFacingLeft() && transform.localScale.y < 0) // Player sagdaysa ve enemy sola bakiyorsa ve enemynin localscale y'si -1 ise
            {
                //Don't change
            }

        }

        public override void takeDamage(int value)
        {
            if (!isHit && health > 0)
            {
                health -= value;
                knockback();
                if (!healthBar.activeInHierarchy && healthBar != null)
                {
                    healthBar.SetActive(true);
                }

                healthBar.GetComponent<HealthBar>().updateHealth(this);

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
                if (healthBar != null)
                {
                    healthBar.SetActive(false);
                }
                
                StopAllCoroutines();
                animator.SetTrigger("Die");
                movementSpeed = 0;
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

        private void knockback()
        {
            setMoveSpeed(-movementSpeed * 2);
            StartCoroutine(setMainSpeed());
        }

        IEnumerator setMainSpeed()
        {
            yield return new WaitForSeconds(0.2f);
            setMoveSpeed(mainSpeed);
        }
    }
}