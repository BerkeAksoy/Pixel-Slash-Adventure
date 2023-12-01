using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class Arrow : Spell
    {

        void Start()
        {
            getComponents();
            damage = 1;
            spellSpeedX = 8f;
        }

        void FixedUpdate()
        {
            calculateMovement();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            checkDamagable(collision);
        }
    }
}