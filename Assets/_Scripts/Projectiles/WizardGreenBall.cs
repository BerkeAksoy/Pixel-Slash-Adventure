using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class WizardGreenBall : Spell, ITracer
    {

        private float rotateSpeed = 180f;

        private void Start()
        {
            getComponents();
            spellSpeedX = 6f;
            damage = 2;
        }

        private void FixedUpdate()
        {
            trace();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            checkDamagable(collision);
        }

        protected override void checkDamagable(Collider2D collision)
        {
            IDamagable hit = collision.GetComponent<IDamagable>();

            if (hit != null)
            {
                hit.takeDamage(damage);
                Destroy(gameObject);
            }
            else if (collision.tag == "Projectile")
            {
                animator.SetTrigger("Explode");
                Destroy(gameObject, 0.3f);
                Destroy(collision.gameObject);
            }
            else if (collision.tag != "Sword")
            {
                Destroy(gameObject);
            }
        }

        public void trace()
        {
            Vector2 direction = (Vector2)player.transform.position - (Vector2)transform.position;

            direction.Normalize();

            float rotateAmount = Vector3.Cross(direction, transform.right).z;

            myRigidBody2D.angularVelocity = -rotateAmount * rotateSpeed;

            myRigidBody2D.velocity = spellSpeedX * transform.right;
        }


    }
}