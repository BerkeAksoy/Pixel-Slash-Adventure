using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode {
    public class CharacterController : MonoBehaviour
    {
        private Rigidbody2D myRigidbody2D;
        private LayerInteractionStateDefiner layerIntStatusDefiner;
        private LayerInteractionStateDefiner.CharLayerInteractionStatus charLayerIntStatus;
        private Vector2 velocity, limitVelocity;
        [SerializeField, Range(0f, 20f)] private float maxSpeed = 10f;
        private float horizontalInput = 0;
        private float turnSpeed,
            maxGroundTurnSpeed = 80f, maxWaterTurnSpeed = 30f, maxAirTurnSpeed = 80f,
            acceleration, deceleration,
            maxGroundAcceleration = 52f, maxGroundDeceleration = 52f,
            maxWaterAcceleration = 30f, maxWaterDeceleration = 60f,
            maxAirAcceleration = 20f, maxAirDeceleration = 52f,
            speedDelta, friction = 0;

        private Animator animator;

        [SerializeField] private bool useAcceleration, useAirAssist;
        private bool lookingRight, pressingMoveKey;

        private void Awake()
        {
            myRigidbody2D = GetComponent<Rigidbody2D>();
            layerIntStatusDefiner = GetComponent<LayerInteractionStateDefiner>();
        }

        void Start()
        {

        }

        void Update()
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            pressingMoveKey = horizontalInput != 0 ? true : false;

            if (pressingMoveKey)
            {
                FlipSprite();
            }

            CalculateDeltaSpeed();

            limitVelocity = new Vector2(horizontalInput, 0f) * Mathf.Max(maxSpeed - friction, 0f);
        }

        private void FixedUpdate()
        {
            //if (pressingMoveKey)
            {
                if (useAcceleration)
                {
                    runWithAcceleration();
                }
                else
                {
                    runWithoutAcceleration();
                }
            }
        }

        private void CalculateDeltaSpeed()
        {
            if (pressingMoveKey)
            {
                // If the sign of our horizontalInput doesn't match our movement, it means we are tring to turn.
                if (Mathf.Sign(horizontalInput) != Mathf.Sign(velocity.x))
                {
                    speedDelta = turnSpeed * Time.deltaTime;
                }
                else // If they match, it means we're simply running along and so should use the acceleration stat
                {
                    speedDelta = acceleration * Time.deltaTime;
                }
            }
            else // And if we're not pressing a direction at all, use the deceleration stat
            {
                speedDelta = deceleration * Time.deltaTime;
            }
        }

        private void runWithAcceleration()
        {
            charLayerIntStatus = layerIntStatusDefiner.GetCharPhyStatus();

            if (charLayerIntStatus.Equals(LayerInteractionStateDefiner.CharLayerInteractionStatus.OnDryLand))
            {
                acceleration = maxGroundAcceleration;
                deceleration = maxGroundDeceleration;
                turnSpeed = maxGroundTurnSpeed;
            }
            else if (charLayerIntStatus.Equals(LayerInteractionStateDefiner.CharLayerInteractionStatus.Swimming) || charLayerIntStatus.Equals(LayerInteractionStateDefiner.CharLayerInteractionStatus.WaterWalking))
            {
                acceleration = maxWaterAcceleration;
                deceleration = maxWaterDeceleration;
                turnSpeed = maxWaterTurnSpeed;
            }
            else if(charLayerIntStatus.Equals(LayerInteractionStateDefiner.CharLayerInteractionStatus.OnAir))
            {
                acceleration = maxAirAcceleration;
                deceleration = maxAirDeceleration;
                turnSpeed = maxAirTurnSpeed;
            }
            else
            {
                acceleration = maxGroundAcceleration;
                deceleration = maxGroundDeceleration;
                turnSpeed = maxGroundTurnSpeed;
            }

            //Move our velocity towards the desired velocity, at the rate of the number calculated above
            velocity.x = Mathf.MoveTowards(velocity.x, limitVelocity.x, speedDelta);

            //Update the Rigidbody with this new velocity
            myRigidbody2D.velocity = velocity;

        }

        private void runWithoutAcceleration()
        {
            velocity.x = limitVelocity.x;
            myRigidbody2D.velocity = velocity;
        }

        private void FlipSprite()
        {
            // Flips the sprite according to player's input.
            if (!lookingRight && horizontalInput > 0)
            {
                transform.localScale = new Vector2(1f, 1f);
                lookingRight = true;
            }
            else if (lookingRight && horizontalInput < 0)
            {
                transform.localScale = new Vector2(-1f, 1f);
                lookingRight = false;
            }
        }
    }

    /*private void Swim()
    {
        if (inWater)
        {
            float verticalInput = Input.GetAxis("Vertical");
            Vector2 swimVelocity = new Vector2(myRigidbody2D.velocity.x, verticalInput * climbingSpeed);

            if (!feared)
            {
                swimVelocity = new Vector2(myRigidbody2D.velocity.x, verticalInput * climbingSpeed);
            }
            else
            {
                swimVelocity = new Vector2(myRigidbody2D.velocity.x, -verticalInput * climbingSpeed);
            }
        }
    }*/

    /*private void climbing()
    {
        if (onLadder)
        {
            float verticalInput = Input.GetAxis("Vertical");
            Vector2 climbVelocity = new Vector2(myRigidbody2D.velocity.x, verticalInput * climbingSpeed);

            if (!feared)
            {
                climbVelocity = new Vector2(myRigidbody2D.velocity.x, verticalInput * climbingSpeed);
            }
            else
            {
                climbVelocity = new Vector2(myRigidbody2D.velocity.x, -verticalInput * climbingSpeed);
            }

            myRigidbody2D.velocity = climbVelocity;

            if (Input.GetButton("Vertical") && !alreadyClimbing)
            {
                animator.SetBool("isClimbing", true);
                alreadyClimbing = true;
            }
            else if (!Input.GetButton("Vertical"))
            {
                animator.SetBool("isClimbing", false);
                alreadyClimbing = false;
            }
        }
    }*/
}
