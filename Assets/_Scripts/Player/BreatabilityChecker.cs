using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class BreatabilityChecker : MonoBehaviour
    {

        private LayerMask waterLayer;
        private bool canBreath;
        private float submergeLength = 0.1f;
        private Vector3 mouthOffset;

        private void Awake()
        {
            canBreath = true;
        }

        private void Update()
        {
            isBreathable();
        }

        private void isBreathable() // If the characters' mouth under water they cannot breath even if their body is not in water.
        {
            canBreath = !Physics2D.Raycast(transform.position + mouthOffset, Vector2.down, submergeLength, waterLayer);
        }

        public bool GetCanBreath()
        {
            return canBreath;
        }

        // Move the code to appropriate UI script
        /*IEnumerator breathCheck()
        {
            stats.updateBreathUI();

            while (!canBreath)
            {
                yield return new WaitForSeconds(2f);

                if (stats.currentBreath > 0)
                {
                    stats.currentBreath--;
                    stats.updateBreathUI();
                    Debug.Log("My Breath: " + stats.currentBreath);
                }
                else
                {
                    Debug.Log("Taking Damage");
                    //takeDamage(10);
                }

                stats.fullBreath = false;
            }
        }*/

       /* private void checker()
        {
            if (!stats.fullBreath)
            {
                //StartCoroutine(refillBreath());
                StopCoroutine(breathCheck());

                stats.refillBreath();
            }
        }*/
    }
}
