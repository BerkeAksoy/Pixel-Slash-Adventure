using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class FireBomb : Spell
    {

        private void Start()
        {
            getComponents();

            myRigidBody2D.velocity = new Vector2(spellSpeedX, spellSpeedY);
            myRigidBody2D.velocity = spellSpeedX * transform.right;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            animator.SetTrigger("Explode");
            Destroy(gameObject, 0.66f);
            myRigidBody2D.velocity = new Vector2(0, 0);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            IDamagable hit = collision.GetComponent<IDamagable>();

            if (hit != null)
            {
                if (damage != 0)
                {
                    hit.takeDamage(damage);
                }
            }
        }
    }
}