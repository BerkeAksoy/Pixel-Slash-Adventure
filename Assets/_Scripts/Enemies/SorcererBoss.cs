using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class SorcererBoss : Enemy
    {

        private bool inDrain, inAttack, inSpawn, isMoving, isHurt;

        private bool fase1Cmpltd, fase2Cmpltd, fase3Cmpltd, timerS, delayOK, faseS;
        private int protectorCount = 0;
        private Enemy[] spawnedEnemies;
        private Protector[] spawnedProtectors;

        public Protector protectorPrefab;
        public Enemy[] enemyPrefabs;

        private void Start()
        {
            componentGetter();
            spawnedEnemies = new Enemy[4];
            spawnedProtectors = new Protector[4];
            canSeeX = 100f;
            canAttackX = 3f;
            health = 20;
            maxHealth = 20;
        }

        private void Update()
        {
            test();

            calculateMovement();
        }

        protected override void OnTriggerExit2D(Collider2D collision)
        {

        }

        private void test()
        {
            float distanceX = getXDistanceToPlayer();

            if (!inSpawn && !isHurt && !inDrain)
            {
                if (Mathf.Abs(distanceX) > canAttackX && !isMoving)
                {
                    animator.SetBool("inCombat", false);
                    animator.SetBool("isMoving", true);
                }

                if (distanceX > 0 != isFacingLeft()) // Set position to player
                {
                    flipSprite();
                    GetComponentInChildren<EdgeCollider2D>().transform.localScale = new Vector2(-transform.localScale.x, 1f);
                }

                if (Mathf.Abs(distanceX) <= canAttackX) // Enemy can attack // 0.4f beklet
                {
                    if (!timerS)
                    {
                        timerS = true;
                        StartCoroutine(delay());
                    }
                    if (delayOK && !faseS)
                    {
                        if (health <= 3 * maxHealth / 4 && !fase1Cmpltd) // not in combat eklenmeli mi
                        {
                            animator.SetBool("isMoving", false);
                            animator.SetBool("inCombat", false);
                            animator.SetBool("isSpawning", true);
                            inSpawn = true;
                            faseS = true;

                            Debug.Log("fase 1 " + health);
                        }
                        else if (health <= maxHealth / 2 && !fase2Cmpltd && fase1Cmpltd)
                        {
                            animator.SetBool("isMoving", false);
                            animator.SetBool("inCombat", false);
                            animator.SetBool("isSpawning", true);
                            inSpawn = true;
                            faseS = true;

                            Debug.Log("fase 2 " + health);
                        }
                        else if (health <= maxHealth / 4 && !fase3Cmpltd && fase1Cmpltd && fase2Cmpltd)
                        {
                            animator.SetBool("isMoving", false);
                            animator.SetBool("inCombat", false);
                            animator.SetBool("isSpawning", true);
                            inSpawn = true;
                            faseS = true;

                            Debug.Log("fase 3 " + health);
                        }
                        else
                        {
                            animator.SetBool("isMoving", false);
                            animator.SetBool("inCombat", true);
                        }
                    }
                }
            }
        }

        IEnumerator faseChecker()
        {
            int startHealth = health;
            yield return new WaitForSeconds(0.6f);
            Debug.Log(protectorCount);
            while ((inSpawn || inDrain))
            {
                yield return new WaitForSeconds(0.01f);
                if (protectorCount <= 0)
                {
                    animator.SetBool("isHurt", true);

                    if (!fase1Cmpltd && health != maxHealth)
                    {
                        Debug.Log("Oldu1");
                        fase1Cmpltd = true;
                    }
                    else if (fase1Cmpltd && !fase2Cmpltd && health < 3 * maxHealth / 4)
                    {
                        Debug.Log("Oldu2");
                        fase2Cmpltd = true;
                    }
                    else if (fase1Cmpltd && fase2Cmpltd && !fase3Cmpltd && health < maxHealth / 2)
                    {
                        Debug.Log("Oldu3");
                        fase3Cmpltd = true;
                    }
                }
            }

            faseS = false;
        }

        IEnumerator delay()
        {
            yield return new WaitForSeconds(0.2f);
            if (Mathf.Abs(getXDistanceToPlayer()) < canAttackX)
            {
                delayOK = true;
            }
            else
            {
                delayOK = false;
            }

            timerS = false;
        }

        private void isDrainTrue()
        {
            inDrain = true;
            StartCoroutine(gainHP());
            StartCoroutine(dealDamage());
            gameObject.layer = LayerMask.NameToLayer("EnemyInvisible");
        }

        IEnumerator dealDamage()
        {
            while (inDrain)
            {
                yield return new WaitForSeconds(1f);

                if (Mathf.Abs(getXDistanceToPlayer()) < 7.36f)
                {
                    player.takeDamage(10);
                }
            }
        }

        private void isDrainFalse()
        {
            inDrain = false;
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        }

        private void isHurtTrue()
        {
            isHurt = true;
        }

        private void isHurtFalse()
        {
            isHurt = false;
            animator.SetBool("isHurt", false);
        }

        private void isMovingTrue()
        {
            isMoving = true;
            gameObject.layer = LayerMask.NameToLayer("EnemyInvisible");
        }

        private void isMovingFalse()
        {
            isMoving = false;
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        }

        private void isSpawningTrue()
        {
            inSpawn = true;
            StartCoroutine(faseChecker());
        }

        private void isSpawningFalse()
        {
            inSpawn = false;
            animator.SetBool("isSpawning", false);
        }

        private void spawnProtectors()
        {
            protectorCount = Random.Range(1, 5);
            int enemyCount = Random.Range(1, 3);
            Debug.Log(protectorCount);
            Vector2[] Pp = { new Vector2(25, -4), new Vector2(26, -4), new Vector2(27, -4), new Vector2(28, -4) };
            Vector2[] Ep = { new Vector2(25, -2), new Vector2(30, -2) };

            for (int i = 0; i < protectorCount; i++)
            {
                Protector p = Instantiate(protectorPrefab, Pp[i], Quaternion.identity);
                spawnedProtectors[i] = p;
            }

            for (int j = 0; j < enemyCount; j++)
            {
                Enemy e = Instantiate(enemyPrefabs[0], Ep[j], Quaternion.identity);
                spawnedEnemies[j] = e;
            }
        }

        private void destroyProtectors()
        {
            for (int i = 0; i < 4; i++)
            {
                if (spawnedProtectors[i] != null)
                {
                    spawnedProtectors[i].takeDamage(1);
                }

                if (spawnedEnemies[i] != null)
                {
                    spawnedEnemies[i].takeDamage(-1);
                }
            }

            for (int i = 0; i < 4; i++)
            {
                spawnedProtectors[i] = null;
                spawnedEnemies[i] = null;
            }
        }

        public override void takeDamage(int value)
        {
            if (!isHit)
            {
                isHit = true;
                health -= 5;

                StartCoroutine(refreshIsHit());
            }
        }

        IEnumerator gainHP()
        {
            while (inDrain)
            {
                yield return new WaitForSeconds(0.8f);
                if (health < maxHealth)
                {
                    health += 1;
                }
                Debug.Log(health);
            }
        }

        public void redProtectorCount()
        {
            protectorCount--;
            Debug.Log("kalan protector " + protectorCount);
        }


    }
}