using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public abstract class Spell : MonoBehaviour
    {
        [SerializeField]
        protected float spellSpeedX = 3f, spellSpeedY = 0;
        [SerializeField]
        protected int damage = 1;

        protected Animator animator;
        protected Rigidbody2D myRigidBody2D;
        protected Player player;
        protected ParticleSystem effect;

        protected virtual void getComponents()
        {
            animator = GetComponent<Animator>();
            myRigidBody2D = GetComponent<Rigidbody2D>();
            player = GameObject.Find("Player").GetComponent<Player>();
        }

        protected virtual void calculateMovement()
        {
            transform.Translate(new Vector3(spellSpeedX * Time.deltaTime, 0, 0));
        }

        protected virtual void throwMovement()
        {
            // V0 = gt
            // H = 1/2gt^2

            Vector2 playerPosition = player.transform.position;
            float maxHeight;
            float distanceX = transform.position.x - playerPosition.x;
            float distanceY;
            float t; // tepeden inecegi zaman
            float v; // Toplam havada gecirdigi zaman dilimi 0.31 yukselirken

            if (Mathf.Abs(transform.position.y - playerPosition.y) <= 2 || playerPosition.y - transform.position.y < 0)
            {
                maxHeight = 2;
                distanceY = Mathf.Abs(transform.position.y + maxHeight - playerPosition.y - 0.7f);
                t = Mathf.Sqrt(2 * distanceY / 35); // tepeden inecegi zaman
                v = -distanceX / (0.31f + t); // Toplam havada gecirdigi zaman dilimi 0.31 yukselirken
                if (!float.IsNaN(v))
                {
                    myRigidBody2D.velocity = new Vector2(v, 10.60f);
                }
            }
            else
            {
                maxHeight = 4f;
                distanceY = transform.position.y + maxHeight - playerPosition.y - 0.7f;
                t = Mathf.Sqrt(2 * distanceY / 35);
                v = -distanceX / (0.48f + t);
                if (!float.IsNaN(v))
                {
                    myRigidBody2D.velocity = new Vector2(v, 16.8f);
                }
            }
        }

        protected virtual void checkDamagable(Collider2D collision)
        {
            IDamagable hit = collision.GetComponent<IDamagable>();

            if (hit != null)
            {
                if (collision.CompareTag("Player"))
                {
                    hit.takeDamage(damage);
                }

                SpellExplosion();
            }
            else if (collision.tag != "Projectile")
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

        public virtual void setSpeed(float speed)
        {
            spellSpeedX = speed;
        }
    }
}
