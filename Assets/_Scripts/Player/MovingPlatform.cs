using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class MovingPlatform : MonoBehaviour
    {
        public Transform startP, endP;
        public float speed = 4f;

        private Rigidbody2D rb;
        private Vector3 startPFixed, endPFixed, moveDirection;
        private bool moveUpwards;

        private void Awake()
        {
            startPFixed = startP.position;
            endPFixed = endP.position;

            moveDirection = (endPFixed - startPFixed).normalized;

            rb = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            DefineMoveDirection();
            Moving();
        }

        private void Moving()
        {
            if (moveUpwards)
            {
                rb.MovePosition(transform.position + moveDirection * Time.fixedDeltaTime);
            }
            else
            {
                rb.MovePosition(transform.position + -moveDirection * Time.fixedDeltaTime);
            }
        }

        private void DefineMoveDirection()
        {
            if (transform.position.y <= startPFixed.y && !moveUpwards)
            {
                moveUpwards = true;
            }

            if (transform.position.y >= endPFixed.y && moveUpwards)
            {
                moveUpwards = false;
            }
        }
        
        
    }
}
