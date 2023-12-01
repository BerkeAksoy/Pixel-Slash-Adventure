using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class Destructible : MonoBehaviour, IDamagable
    {

        private int health = 1;
        private float timeToDestroy = 0.33f;
        private bool isHit = false, isAlive = true;
        private Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }


        public void takeDamage(int value)
        {
            if (!isHit && health > 0)
            {
                health -= value;

                if (health > 0)
                {
                    isHit = true;
                    animator.SetTrigger("isHit");
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

                animator.SetTrigger("Die");
                GetComponent<CapsuleCollider2D>().enabled = false;
                GetComponent<BoxCollider2D>().enabled = false;
                Destroy(gameObject, timeToDestroy);
            }
        }

        protected virtual IEnumerator refreshIsHit()
        {
            yield return new WaitForSeconds(0.2f);
            isHit = false;
        }
    }
}