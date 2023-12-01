using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BerkeAksoyCode
{
    public abstract class Enemy : MonoBehaviour, IDamagable
    {

        protected Rigidbody2D myRigidbody2D;
        protected CapsuleCollider2D myBody2D;
        protected BoxCollider2D myFeet2D;
        protected GameObject myEyes, myHB;
        protected float movementSpeed = 2f, newCombatTime = 2f, idleWaitTime = 2f, timeToDestroy = 1f, jumpForce = 6f;
        protected float canSeeX = 6f, canSeeY = 0.5f, canAttackX = 6f, canAttackY = 2f;
        protected int health = 15, spellId = 0, maxHealth = 15, XPValue = 50;
        protected bool inCombat = false, isHit = false, isAlive = true;
        protected Animator animator;
        protected GameManager gm;
        protected Player player;
        public GameObject expOrb;
        [SerializeField]
        private GameObject popUpText;

        protected void OnDestroy()
        {
            gm.EnemyList.Remove(this);
        }


        protected virtual void componentGetter()
        {
            myRigidbody2D = GetComponent<Rigidbody2D>();
            myBody2D = GetComponent<CapsuleCollider2D>();
            myFeet2D = GetComponent<BoxCollider2D>();
            animator = GetComponent<Animator>();
            myEyes = GameObject.Find("/" + name + "/Eyes");
            myHB = GameObject.Find("/" + name + "/Canvas/Health Bar");
            gm = GameManager.Instance;
            player = GameObject.Find("Player").GetComponent<Player>();

            if (myHB != null)
            {
                myHB.SetActive(false);
            }

            gm.EnemyList.Add(this);
        }

        protected virtual void calculateMovement()
        {
            if (isFacingLeft())
            {
                myRigidbody2D.velocity = new Vector2(-movementSpeed, myRigidbody2D.velocity.y);
            }
            else
            {
                myRigidbody2D.velocity = new Vector2(movementSpeed, myRigidbody2D.velocity.y);
            }
        }

        protected virtual bool checkRayCast()
        {

            RaycastHit2D[] hits = new RaycastHit2D[2];

            hits[0] = Physics2D.Linecast(myEyes.transform.position, player.transform.position, 1 << LayerMask.NameToLayer("Player"));
            hits[1] = Physics2D.Linecast(myEyes.transform.position, player.transform.position, 1 << LayerMask.NameToLayer("Ground"));

            //Debug.DrawLine(myEyes.transform.position, player.transform.position, Color.red);

            // Player Seen
            if (hits[0].collider != null && hits[1].collider == null)
            {
                //Debug.Log(this.tag + " PlayerSeen");
                return true;
            }

            return false;
        }

        protected virtual void checkDistance()
        {
            float distanceY = getYDistanceToPlayer();
            float distanceX = getXDistanceToPlayer();

            if ((Mathf.Abs(distanceX) < canSeeX && Mathf.Abs(distanceY) < canSeeY && checkRayCast()) || isHit) // Enemy can see player && raycast not hits any ground item
            {
                if (!inCombat) // Walk enough to attack
                {
                    animator.SetBool("isMoving", true);
                }

                if (distanceX > 0 != isFacingLeft()) // Set position to player
                {
                    flipSprite();

                    Caster caster = GetComponentInChildren<Caster>();
                    if (caster != null)
                    {
                        caster.transform.localScale = transform.localScale;
                    }
                }
                if (!inCombat && Mathf.Abs(distanceX) < canAttackX && Mathf.Abs(distanceY) < canAttackY) // Enemy can attack
                {
                    inCombat = true;
                    animator.SetBool("inCombat", true);
                    animator.SetBool("isMoving", false);
                    StartCoroutine(refreshInCombat());
                }
            }
            else
            {
                switch (this.tag)
                {
                    case "Patroller":
                        break;
                    default:
                        animator.SetBool("isMoving", false);
                        break;
                }
            }
        }

        protected virtual float getXDistanceToPlayer()
        {
            return transform.position.x - player.transform.position.x;
        }

        protected virtual float getYDistanceToPlayer()
        {
            return transform.position.y - player.transform.position.y;
        }

        protected virtual bool isFacingLeft()
        {
            return transform.localScale.x > 0;
        }

        protected abstract void OnTriggerExit2D(Collider2D collision);

        protected virtual void changeDirection(Collider2D collision)
        {
            if (collision.CompareTag("Projectile") || collision.CompareTag("Player") || collision.CompareTag("Bridge"))
            {

            }
            else if (collision != null)
            {
                flipSprite();
                animator.SetBool("isMoving", false);
                StartCoroutine(waitForIdle());
            }
        }

        protected virtual void flipSprite()
        {
            transform.localScale = new Vector2(-transform.localScale.x, 1f);

            if (myHB != null)
            {
                myHB.transform.rotation = myHB.transform.rotation * Quaternion.Euler(0f, 180f, 0f);
            }

        }

        protected virtual void jump(Collider2D collision)
        {
            if (!collision.tag.Equals("Projectile") && collision.tag.Equals("Player") && !isHit)
            {
                if (myFeet2D.IsTouchingLayers(LayerMask.GetMask("Ground")))
                {
                    myRigidbody2D.velocity = new Vector2(myRigidbody2D.velocity.x, jumpForce);
                }
                //animator.SetBool("isJumping", true);
            }
        }

        public virtual void takeDamage(int value)
        {
            if (!isHit && health > 0)
            {
                health -= value;
                if (value > 0)
                {
                    GameObject a = Instantiate(popUpText, transform.position, Quaternion.identity);
                    a.GetComponent<TextMeshPro>().text = value.ToString();
                }
                else
                {
                    GameObject a = Instantiate(popUpText, transform.position, Quaternion.identity);
                    a.GetComponent<TextMeshPro>().text = "Missed!";
                }

                if (!myHB.activeInHierarchy && myHB != null)
                {
                    myHB.SetActive(true);
                }

                myHB.GetComponent<HealthBar>().updateHealth(this);
                if (GameObject.Find("/" + name + "/Blood Particles"))
                {
                    StartCoroutine(bloodParticle());
                }

                if (health > 0)
                {
                    isHit = true;
                    if (value > maxHealth / 3) // isHit animation will play only the damage is greater than a certain limit
                    {
                        animator.SetTrigger("isHit");
                    }
                    StartCoroutine(refreshIsHit());
                }
            }

            if (health <= 0 && isAlive)
            {
                createExpOrb();

                isAlive = false;
                StopAllCoroutines();
                animator.SetTrigger("Die");
                movementSpeed = 0;

                if (myHB != null)
                {
                    myHB.SetActive(false);
                }

                if (GetComponent<Dropper>())
                {
                    GetComponent<Dropper>().dropItem();
                    GetComponent<Dropper>().dropCoins();
                }
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

        private void createExpOrb()
        {
            int orbCount = XPValue / 10;
            int lastOrbValue = XPValue % 10;

            if (lastOrbValue > 0)
            {
                orbCount++;
            }

            for (int i = 0; i < orbCount; i++)
            {
                GameObject eO = Instantiate(expOrb, transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-0.4f, 1.6f), 0), Quaternion.identity);

                if (lastOrbValue > 0 && i == orbCount - 1)
                {
                    eO.GetComponent<ExpOrb>().ExpValue = lastOrbValue;
                }
                else
                {
                    eO.GetComponent<ExpOrb>().ExpValue = 10;
                }
            }

            //player.stats.addXP(XPValue);
        }

        protected virtual void attack()
        {
            Caster caster = GetComponentInChildren<Caster>();
            if (caster != null)
            {
                caster.instantiatePrefab(spellId, isFacingLeft());
            }
        }

        public void setMoveSpeed(float moveSpeed)
        {
            movementSpeed = moveSpeed;
        }

        protected virtual IEnumerator waitForIdle()
        {
            yield return new WaitForSeconds(idleWaitTime);
            animator.SetBool("isMoving", true);
        }

        protected virtual IEnumerator refreshIsHit()
        {
            yield return new WaitForSeconds(0.4f);
            isHit = false;
        }

        protected virtual IEnumerator refreshInCombat()
        {
            yield return new WaitForSeconds(newCombatTime);
            animator.SetBool("inCombat", false);
            animator.SetBool("isMoving", true);
            inCombat = false;
        }

        protected virtual IEnumerator bloodParticle()
        {
            if (GetComponentInChildren<ParticleSystem>())
            {
                GetComponentInChildren<ParticleSystem>().Play();
                yield return new WaitForSeconds(0.3f);
                GetComponentInChildren<ParticleSystem>().Stop();
                Debug.Log("Destroyed Blood Particle");
            }

        }

        public int getHealth()
        {
            return health;
        }

        public int getMaxHealth()
        {
            return maxHealth;
        }

        public float getTimeToDestroy()
        {
            return timeToDestroy;
        }
    }
}