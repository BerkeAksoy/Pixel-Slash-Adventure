using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class Protector : MonoBehaviour, IDamagable
    {

        private SorcererBoss Boss;

        private void Start()
        {
            Boss = GameObject.Find("Sorcerer Boss").GetComponent<SorcererBoss>();

            if (Boss == null)
            {
                Debug.LogError("");
            }
        }

        public void takeDamage(int value)
        {
            GetComponent<Animator>().SetTrigger("Die");
            Destroy(gameObject, 0.5f);
        }

        private void OnDestroy()
        {
            Boss.redProtectorCount();
        }
    }
}