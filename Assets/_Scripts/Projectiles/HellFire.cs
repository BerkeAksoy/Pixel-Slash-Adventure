﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class HellFire : Spell
    {

        private void Start()
        {
            getComponents();
        }

        private void FixedUpdate()
        {
            calculateMovement();
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


    }
}