using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class PlayerSpell : MonoBehaviour, ITracer
    {
        [SerializeField]
        private float spellSpeed = 6f;
        private bool isFacingRight = true;
        [SerializeField]
        private int id = 0, damage = 0, spellCost = 0;
        private Enemy nearestEnemy = null;
        private float rotateSpeed = 180f;
        private Rigidbody2D myRigidbody2D;
        public bool traceEnemy;
        private Player player;
        public GameObject effect;

        private GameManager gm;
        void Start()
        {
            player = GameObject.Find("Player").GetComponent<Player>();
            gm = GameManager.Instance;
            myRigidbody2D = GetComponent<Rigidbody2D>();
            isFacingRight = true; //player.isFacingRight();

            calculateDamage(player.stats.cMgcDmg);

            if (!isFacingRight)
            {
                transform.localScale = new Vector2(-1, 1f);
                if (GetComponentInChildren<ParticleSystem>())
                {
                    GetComponentInChildren<ParticleSystem>().transform.localScale = new Vector2(-1, 1f);
                }
            }

            if (traceEnemy)
            {
                nearestEnemy = findNearestEnemy();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (traceEnemy)
            {
                if (nearestEnemy != null)
                {
                    trace();
                }
                else
                {
                    normalMovement();
                }
            }
            else
            {
                normalMovement();
            }
        }

        private void normalMovement()
        {
            if (isFacingRight)
            {
                transform.Translate(new Vector3(spellSpeed * Time.deltaTime, 0, 0));
            }
            else
            {
                transform.Translate(new Vector3(-spellSpeed * Time.deltaTime, 0, 0));
            }
        }

        public void trace()
        {
            Vector2 direction = (Vector2)nearestEnemy.transform.position - (Vector2)transform.position;
            float rotateAmount;

            direction.Normalize();

            if (isFacingRight)
            {
                rotateAmount = Vector3.Cross(direction, transform.right).z;
                myRigidbody2D.velocity = spellSpeed * transform.right;
            }
            else
            {
                rotateAmount = Vector3.Cross(direction, -transform.right).z;
                myRigidbody2D.velocity = spellSpeed * -transform.right;
            }

            myRigidbody2D.angularVelocity = -rotateAmount * rotateSpeed;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            IDamagable hit = collision.GetComponent<IDamagable>();

            if (hit != null)
            {
                hit.takeDamage(damage);
                SpellExplosion();
            }
            else if (collision.tag != "EnemyNormalProjectile")
            {
                SpellExplosion();
            }
        }

        protected virtual void SpellExplosion()
        {
            if (effect != null)
            {
                Instantiate(effect, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }

        public Enemy findNearestEnemy()
        {
            float min = Mathf.Infinity, distance = 0;
            int enemyIndex = -1;

            for (int i = 0; i < gm.EnemyList.Count; i++)
            {
                distance = Vector3.Distance(gm.EnemyList.ToArray()[i].transform.position, transform.position);

                if (distance <= min)
                {
                    min = distance;
                    enemyIndex = i;
                }
            }

            return gm.EnemyList.ToArray()[enemyIndex];
        }

        public int getSpellCost()
        {
            return this.spellCost;
        }

        public void calculateDamage(int mgcDmgIn)
        {
            damage = mgcDmgIn;

            if (Random.Range(0, 100) < player.stats.cCritChance)
            {
                damage = Mathf.FloorToInt(damage * player.stats.getDamageRandomizer() * player.stats.cCritMultiplier);
                Debug.Log("Crit Timeeeeeeeeeeeeeeeeee !!!!!!!!!!!!!!!!!!!!!");
            }
            else
            {
                damage = Mathf.FloorToInt((float)damage * player.stats.getDamageRandomizer());
            }
        }


    }
}