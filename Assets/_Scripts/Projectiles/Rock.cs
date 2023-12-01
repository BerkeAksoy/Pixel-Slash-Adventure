using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class Rock : Spell
    {
        // Start is called before the first frame update
        void Start()
        {
            getComponents();
            spellSpeedY = 2;
            throwMovement();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            checkDamagable(collision);
        }

        IEnumerator Resistance()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.2f);
                spellSpeedY -= 1;
                if (spellSpeedX > 0)
                {
                    spellSpeedX -= 0.1f;
                }
            }
        }

        protected override void checkDamagable(Collider2D collision)
        {
            IDamagable hit = collision.GetComponent<IDamagable>();

            if (hit != null)
            {
                hit.takeDamage(damage);
                animator.SetTrigger("isHit");
                Destroy(gameObject, 0.17f);
            }
            else if (collision.tag == "Ground")
            {
                animator.SetTrigger("isHit");
                Destroy(gameObject, 0.17f);
            }
        }
    }
}
