using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class BreathStateDefiner : MonoBehaviour
    {
        [SerializeField] private LayerMask waterLayer;
        [SerializeField, Tooltip("Sets mouth position relative to character's position")]
        private Vector3 mouthOffset = new Vector3(0.05f,0.268f,0);
        private const float _SubmergeLength = 0.1f;

        public bool isBreathable() // If the characters' mouth under water they cannot breath even if their body is not in water and vice-versa.
        {
            return !Physics2D.Raycast(transform.position + mouthOffset, Vector2.down, _SubmergeLength, waterLayer);
        }
    }
}
