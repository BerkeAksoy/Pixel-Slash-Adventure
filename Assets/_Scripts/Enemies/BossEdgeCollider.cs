using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class BossEdgeCollider : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            GameObject.Find("Player").GetComponent<Player>().takeDamage(1);
        }
    }
}