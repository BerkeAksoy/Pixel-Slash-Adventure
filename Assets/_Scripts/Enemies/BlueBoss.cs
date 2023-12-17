using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class BlueBoss : Enemy
    {

        private bool inFirstF = true, inIdle = true, inJumpAttack = false;

        public GameObject[] effectPrefabs;

        void Start()
        {
            componentGetter();
            animator.SetBool("inIdle", true);
            canSeeX = 6f;
            canSeeY = 1.2f;
            canAttackX = 1.4f;
            canAttackY = 1.2f;
            newCombatTime = 0.76f;
            idleWaitTime = 1f;
            timeToDestroy = 0.875f;
            health = 10;
            maxHealth = 10;
        }

        void Update()
        {
            if (isAlive)
            {
                if (inFirstF)
                {
                    //calculateMovement();
                    checkDistance();
                }
                else
                {
                    if (getXDistanceToPlayer() > 0 != isFacingLeft() && !inJumpAttack) // Set position to player
                    {
                        flipSprite();
                    }
                }

                calculateMovement();
            }
        }

        protected override void OnTriggerExit2D(Collider2D collision)
        {

        }

        protected override void checkDistance()
        {
            float distanceY = getYDistanceToPlayer();
            float distanceX = getXDistanceToPlayer();

            if ((Mathf.Abs(distanceX) < canSeeX && Mathf.Abs(distanceY) < canSeeY && checkRayCast()) || isHit) // Enemy can see player && raycast not hits any ground item
            {
                if (inIdle)
                {
                    inIdle = false;
                    animator.SetBool("inIdle", false);
                }

                if (!inCombat && inFirstF) // Walk enough to attack
                {
                    animator.SetBool("isMoving", true);
                }

                if (distanceX > 0 != isFacingLeft() && !inCombat) // Set position to player
                {
                    flipSprite();

                    Caster caster = GetComponentInChildren<Caster>();
                    if (caster != null)
                    {
                        caster.transform.localScale = transform.localScale;
                    }
                }
                if (!inCombat && Mathf.Abs(distanceX) < canAttackX && Mathf.Abs(distanceY) < canAttackY && inFirstF) // Enemy can attack
                {
                    inCombat = true;
                    animator.SetBool("inCombat", true);

                    animator.SetBool("isMoving", false);
                    StartCoroutine(refreshInCombat());
                }
                //else if(!inCombat && Mathf.Abs(distanceX) < canAttackX && Mathf.Abs(distanceY) < canAttackY && !inFirstF)
                //{
                //animator.SetBool("isMoving", false);
                //animator.SetBool("inJumpAttack", true);
                //}
            }
            else
            {
                if (!inIdle)
                {

                    movementSpeed = 0;
                    animator.SetBool("isMoving", false);
                    animator.SetTrigger("isBlinking");

                    //if (!inFirstF)
                    //{
                    //animator.SetBool("inJumpAttack", true);
                    //}
                }
            }
        }

        private void jumpMovement()
        {
            // V0 = gt
            // H = 1/2gt^2

            float distanceX = getXDistanceToPlayer();
            float distanceY = 4.53f;
            float t; // tepeden inecegi zaman
            float v;

            StartCoroutine(activateJumpAttack());

            t = Mathf.Sqrt(2 * distanceY / 35); // tepeden inecegi zaman

            v = (Mathf.Abs(distanceX) / (2 * t)); // Yatay hizi

            if (!float.IsNaN(v))
            {
                myRigidbody2D.velocity = new Vector2(myRigidbody2D.velocity.x, 17.815f);
                setMoveSpeed(v);
                StartCoroutine(slam());
            }
        }

        IEnumerator slam()
        {
            yield return new WaitForSeconds(0.8f);
            animator.SetTrigger("Slam");
        }

        IEnumerator activateJumpAttack()
        {
            yield return new WaitForSeconds(0.7f);
            inJumpAttack = true;
            StartCoroutine(refreshJumpAttack());
        }

        IEnumerator refreshJumpAttack()
        {
            yield return new WaitForSeconds(0.7f);
            inJumpAttack = false;
        }

        private void instantiateEffect1()
        {
            GameObject effect;

            if (isFacingLeft())
            {
                effect = Instantiate(effectPrefabs[0], new Vector2(transform.position.x - 2.13f, effectPrefabs[0].transform.position.y), Quaternion.identity);
            }
            else
            {
                effect = Instantiate(effectPrefabs[0], new Vector2(transform.position.x + 2.13f, effectPrefabs[0].transform.position.y), Quaternion.identity);
            }

            Destroy(effect, 0.5f);
        }

        private void instantiateEffect2()
        {
            GameObject effect;

            if (isFacingLeft())
            {
                effect = Instantiate(effectPrefabs[1], new Vector2(transform.position.x - 3.83f, effectPrefabs[1].transform.position.y), Quaternion.identity);
            }
            else
            {
                effect = Instantiate(effectPrefabs[1], new Vector2(transform.position.x + 3.83f, effectPrefabs[1].transform.position.y), Quaternion.identity);
            }

            Destroy(effect, 0.375f);
        }

        private void instantiateEffect3()
        {
            GameObject effect;

            if (isFacingLeft())
            {
                effect = Instantiate(effectPrefabs[2], new Vector2(transform.position.x - 6.1f, effectPrefabs[2].transform.position.y), Quaternion.identity);
            }
            else
            {
                effect = Instantiate(effectPrefabs[2], new Vector2(transform.position.x + 6.1f, effectPrefabs[2].transform.position.y), Quaternion.identity);
            }

            Destroy(effect, 0.125f);
        }

        private void blink()
        {
            Vector3 blinkVector;

            if (getXDistanceToPlayer() > 0)
            {
                blinkVector = new Vector3(player.transform.position.x - 2, transform.position.y, 0);
            }
            else
            {
                blinkVector = new Vector3(player.transform.position.x + 2, transform.position.y, 0);
            }

            transform.SetPositionAndRotation(blinkVector, Quaternion.identity);
        }

        public override void takeDamage(int value)
        {
            if (!isHit && health > 0)
            {
                health -= value;

                //UIManager.Instance.updateEnemyHealth(this);
                StartCoroutine(bloodParticle());

                if (health > 0)
                {
                    isHit = true;
                    StartCoroutine(refreshIsHit());

                    if (health <= maxHealth / 2)
                    {
                        inFirstF = false;
                        Debug.Log("Second fase e geçti " + health);
                        animator.SetBool("isMoving", false);
                        animator.SetBool("inJumpAttack", true);
                    }
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
                
                StopAllCoroutines();
                animator.SetTrigger("Die");
                movementSpeed = 0;
                GetComponent<CapsuleCollider2D>().enabled = false;
                myRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
                myRigidbody2D.velocity = new Vector2(0, 0);
                Destroy(gameObject, timeToDestroy);
            }
        }

        protected override IEnumerator refreshInCombat()
        {
            yield return new WaitForSeconds(newCombatTime);
            if (inFirstF)
            {
                animator.SetBool("inCombat", false);
            }
            else
            {

            }
            animator.SetBool("isMoving", true);
            inCombat = false;
        }

        protected override IEnumerator refreshIsHit()
        {
            yield return new WaitForSeconds(0.4f);
            isHit = false;
        }
    }
}