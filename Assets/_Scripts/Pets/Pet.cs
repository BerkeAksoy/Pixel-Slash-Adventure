using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public abstract class Pet : MonoBehaviour
    {

        Player player = null;
        Animator animator = null;
        Rigidbody2D myRigidbody2D = null;
        private float timer = 0;
        protected bool grounded, canAttack;
        protected float moveSpeed, attackDistance;
        private bool aR = false, aA = false; // alreadyRunning, alreadyAttacking
        private GameManager gm;

        public float baseMoveSpeed;
        public bool aggresive;

        protected virtual void Start()
        {
            gm = GameManager.Instance;
            player = GameObject.Find("Player").GetComponent<Player>();
            animator = GetComponent<Animator>();
            myRigidbody2D = GetComponent<Rigidbody2D>();

            moveSpeed = baseMoveSpeed;
            attackDistance = 5f;
        }

        protected virtual void FixedUpdate()
        {
            calculatePetMovement();
            checkPetDistance();
        }

        protected virtual void calculatePetMovement()
        {
            if (isFacingRight())
            {
                myRigidbody2D.velocity = new Vector2(moveSpeed, myRigidbody2D.velocity.y);
            }
            else
            {
                myRigidbody2D.velocity = new Vector2(-moveSpeed, myRigidbody2D.velocity.y);
            }
        }

        protected virtual void checkPetDistance()
        {
            float playerDistX = getXRelativeDistance(player.gameObject);
            //float playerDistY = getYRelativeDistance(player.gameObject);

            if (IsAggresive())
            {
                if (EnemyInVision() == null)
                {
                    if (!aA)
                    {
                        if (playerDistX < 0 != isFacingRight()) // Set position to player
                        {
                            transform.localScale = new Vector2(-transform.localScale.x, 1f);
                        }

                        if (PlayerInVision() && Mathf.Abs(playerDistX) <= 8f && Mathf.Abs(playerDistX) > 1.2f)
                        {
                            if (!aR)
                            {
                                animator.SetBool("isMoving", true);
                                moveSpeed = baseMoveSpeed;
                                aR = true;
                            }
                        }
                        else if ((!PlayerInVision() || Mathf.Abs(playerDistX) > 8f))// && player.isGrounded())
                        {
                            DelayedBlink();
                        }
                        else
                        {
                            if (aR)
                            {
                                animator.SetBool("isMoving", false);
                                moveSpeed = 0;
                                aR = false;
                            }
                        }
                    }
                    else
                    {
                        animator.SetBool("isAttacking", false);
                        animator.SetBool("isMoving", false);
                        aA = false;
                        aR = false;
                    }
                }
                else
                {
                    GameObject enemyToAttack = EnemyInVision();
                    Debug.Log(enemyToAttack);
                    float enemyDistX = getXRelativeDistance(enemyToAttack);
                    float enemyDistY = getYRelativeDistance(enemyToAttack);

                    if (enemyDistX < 0 != isFacingRight()) // Set position to player
                    {
                        transform.localScale = new Vector2(-transform.localScale.x, 1f);
                    }

                    if ((!PlayerInVision() || Mathf.Abs(playerDistX) > 8f))//&& player.isGrounded())
                    {
                        DelayedBlink();
                        animator.SetBool("isAttacking", false);
                        animator.SetBool("isMoving", false);
                        aA = false;
                        aR = false;
                    }
                    else if (Mathf.Abs(enemyDistX) > 1f && Mathf.Abs(enemyDistY) <= 2f)
                    {
                        if (!aR)
                        {
                            animator.SetBool("isMoving", true);
                            animator.SetBool("isAttacking", false);
                            moveSpeed = baseMoveSpeed;
                            aR = true;
                            aA = false;
                        }
                    }
                    else if (Mathf.Abs(enemyDistX) <= 1f && Mathf.Abs(enemyDistY) <= 2f)
                    {
                        if (!aA)
                        {
                            animator.SetBool("isMoving", false);
                            animator.SetBool("isAttacking", true);
                            moveSpeed = 0;
                            aR = false;
                            aA = true;
                        }
                    }
                    else
                    {
                        animator.SetBool("isAttacking", false);
                        aA = false;
                    }
                }
            }
        }

        protected virtual bool PlayerInVision()
        {
            RaycastHit2D[] hits = new RaycastHit2D[1];
            Debug.DrawLine(this.transform.position, player.transform.position, Color.red);

            hits[0] = Physics2D.Linecast(transform.position, player.transform.position, 1 << LayerMask.NameToLayer("Ground"));

            if (hits[0].collider != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        protected virtual GameObject EnemyInVision()
        {
            int enemyCount = gm.EnemyList.Count;
            List<Enemy> enemyList = gm.EnemyList;
            List<GameObject> enemiesInSight = new List<GameObject>();
            GameObject enemyToAttack = null;
            RaycastHit2D[] hits = new RaycastHit2D[enemyCount];

            for (int i = 0; i < enemyCount; i++)
            {
                Debug.DrawLine(this.transform.position, enemyList[i].transform.position, Color.red);
                hits[i] = Physics2D.Linecast(transform.position, enemyList[i].transform.position, 1 << LayerMask.NameToLayer("Ground"));

                if (hits[i].collider == null)
                {
                    enemiesInSight.Add(enemyList[i].gameObject);
                }
                else
                {
                    enemiesInSight.Remove(enemyList[i].gameObject);
                }
            }

            enemyToAttack = NearestEnemy(enemiesInSight);

            return enemyToAttack;
        }

        private GameObject NearestEnemy(List<GameObject> list)
        {
            float min = Mathf.Infinity;
            GameObject enemy = null;

            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    float distance = Vector2.Distance(transform.position, list[i].transform.position);

                    if (distance < min && distance < attackDistance)
                    {
                        min = distance;
                        enemy = list[i];
                    }
                }
            }

            return enemy;
        }

        protected virtual bool IsAggresive()
        {
            if (aggresive)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected virtual void DelayedBlink()
        {
            // Delayed Blink
            timer += Time.deltaTime / 0.8f; // Divided by 0.8 to make it 0.8 seconds.

            if (timer > 0.8f)
            {
                timer = 0;

                Vector2 playerPosition = player.transform.position;
                if (true) //player.isFacingRight())
                {
                    transform.position = new Vector2(playerPosition.x - 1, playerPosition.y);
                }
                else
                {
                    transform.position = new Vector2(playerPosition.x + 1, playerPosition.y);
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log("grounded true");

            if (collision.gameObject.tag.Equals("Ground"))
            {
                grounded = true;
            }
        }

        protected virtual float getXRelativeDistance(GameObject GO)
        {
            return transform.position.x - GO.transform.position.x;
        }

        protected virtual float getYRelativeDistance(GameObject GO)
        {
            return transform.position.y - GO.transform.position.y;
        }

        protected virtual bool isFacingRight()
        {
            return transform.localScale.x > 0;
        }

    }
}