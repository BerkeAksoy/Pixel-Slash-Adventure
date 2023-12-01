using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class Player : MonoBehaviour, IDamagable
    {
        private float climbingSpeed = 3, nextMagic = 0, nextSword = 0;
        private bool feared;

        private CapsuleCollider2D myBody2D;

        private GameObject myMagicBook2D;
        private GameObject mySword2D;

        private Animator animator;
        private PhysicsMaterial2D zeroFriction;
        public CharStats stats;
        private PlayerSpell spellToCast;
        [SerializeField]
        private GameObject popUpText;

        private bool canDoubleJump = true, isAwake = true, onLadder;

        void Start()
        {
            stats = GetComponent<CharStats>();
            myBody2D = GetComponent<CapsuleCollider2D>();
            myMagicBook2D = GameObject.Find("/" + name + "/Magic Book");
            mySword2D = GameObject.Find("/" + name + "/Hit_Box");
            animator = GetComponent<Animator>();
            zeroFriction = myBody2D.sharedMaterial;
        }

        void Update()
        {
            if (isAwake)
            {
                castSpell();
                attack();

                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    stats.chosenSpellSlot = 0;
                    changeSpellTo(0);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    stats.chosenSpellSlot = 1;
                    changeSpellTo(1);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    stats.chosenSpellSlot = 2;
                    changeSpellTo(2);
                }
            }
        }

        private void attack()
        {
            if (Input.GetKeyDown(KeyCode.X) && Time.time > nextSword)
            {
                int hitChance = Random.Range(0, 100);

                if (hitChance >= stats.cMissChance)
                {
                    int damage = stats.cPhyDmg;

                    Debug.Log(stats.cCritChance);

                    if (Random.Range(0, 100) < stats.cCritChance)
                    {
                        damage = Mathf.FloorToInt(stats.cPhyDmg * stats.getDamageRandomizer() * stats.cCritMultiplier);
                        Debug.Log("Crit Timeeeeeeeeeeeeeeeeee !!!!!!!!!!!!!!!!!!!!!");
                    }
                    else
                    {
                        damage = Mathf.FloorToInt(stats.cPhyDmg * stats.getDamageRandomizer());
                    }

                    mySword2D.GetComponent<FriendlyHitBox>().setDamage(damage);
                }
                else
                {
                    mySword2D.GetComponent<FriendlyHitBox>().setDamage(0);
                    Debug.Log("Missed!");
                }
                animator.SetTrigger("isAttacking");
                nextSword = Time.time + stats.cAttackSpeed;
            }
        }

        public void changeSpellTo(int slotNo)
        {
            if (stats.eqpMagicBooks[slotNo] != null)
            {
                spellToCast = stats.eqpMagicBooks[slotNo].playerSpell;
                stats.cCastSpeed = stats.eqpMagicBooks[slotNo].castRate;
                stats.calculateStats();
                //stats.cMgcDmg += stats.eqpMagicBooks[slotNo].spellDamage;
            }
            else
            {
                spellToCast = null;
            }
        }

        private void castSpell()
        {
            if (Input.GetButtonDown("Fire1") && Time.time > nextMagic && spellToCast != null) //Değişim
            {
                int spellCost = Mathf.FloorToInt((float)spellToCast.getSpellCost() * (float)(100 - stats.cManaCostReduce) / 100f);

                if (stats.currentMana >= spellCost)
                {
                    animator.SetTrigger("CastSpell");
                    nextMagic = Time.time + stats.cCastSpeed;

                    stats.currentMana -= spellCost;
                    stats.updateMana();
                    Instantiate(spellToCast, myMagicBook2D.transform.position, Quaternion.identity);
                }
                else
                {
                    Debug.Log("Not enough mana");
                }
            }
        }

        public void takeDamage(int value)
        {
            //Vector2 deathKick = new Vector2(-Mathf.Sign(myRigidbody2D.velocity.x) * 8.0f, 18.0f);
            Vector2 deathKick;

            /*if (isFacingRight())
            {
                deathKick = new Vector2(-4.0f, 2.0f);
            }
            else*/
            {
                deathKick = new Vector2(4.0f, 2.0f);
            }

            //myRigidbody2D.velocity = deathKick;


            if (isAwake)
            {
                stats.currentHP -= value;

                myBody2D.sharedMaterial = null;
            }

            if (value > 0)
            {
                GameObject a = Instantiate(popUpText, transform.position, Quaternion.identity);
                a.GetComponent<TextMeshPro>().text = value.ToString();
                a.GetComponent<TextMeshPro>().color = Color.red;
            }
            else
            {
                GameObject a = Instantiate(popUpText, transform.position, Quaternion.identity);
                a.GetComponent<TextMeshPro>().text = "Missed!";
                a.GetComponent<TextMeshPro>().color = Color.red;
            }

            stats.updateHealth();

            if (stats.currentHP > 0 && !isAwake)
            {
                StartCoroutine(reBorn());
            }
            else if (stats.currentHP <= 0)
            {
                gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
                Debug.Log("You died");
            }
        }

        public void fear()
        {
            feared = true;
            StartCoroutine(reFear());
        }

        IEnumerator reFear()
        {
            yield return new WaitForSeconds(3f);
            feared = false;
        }

        private IEnumerator reBorn()
        {
            yield return new WaitForSeconds(0.6f);
            animator.SetTrigger("Reborn");
            //gameObject.layer = LayerMask.NameToLayer("Player");
            myBody2D.sharedMaterial = zeroFriction;
            //myRigidbody2D.freezeRotation = true;
            isAwake = true;
        }
    }
}
