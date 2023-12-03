using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode {
    public class CharacterController : MonoBehaviour
    {
        private Rigidbody2D myRigidbody2D;
        private LayerInteractionStateDefiner layerIntStatusDefiner;
        private LayerInteractionStateDefiner.CharLayerInteractionStatus charLayerIntStatus;
        private Vector2 curVelocity, desiredVelocity;
        private bool lookingRight, pressingMoveKey;
        private float horizontalInput = 0, acceleration, deceleration, turnSpeed, speedDelta, friction = 0f;

        [SerializeField, Range(0f, 50f)]
        [Tooltip("See the code to understand how to calculate")]
        private float maxGroundTurnSpeed = 50f, maxWaterTurnSpeed = 30f, maxAirTurnSpeed = 50f,
            maxGroundAcceleration = 50f, maxGroundDeceleration = 50f,
            maxWaterAcceleration = 50f, maxWaterDeceleration = 50f,
            maxAirAcceleration = 50f, maxAirDeceleration = 50f;

        private Animator animator;

        [SerializeField, Range(0f, 20f)] private float maxSpeed = 10f;
        [SerializeField] private bool useAcceleration, useAirAssist;

        private void Awake()
        {
            myRigidbody2D = GetComponent<Rigidbody2D>();
            layerIntStatusDefiner = GetComponent<LayerInteractionStateDefiner>();
        }

        void Update()
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            pressingMoveKey = horizontalInput != 0 ? true : false;

            desiredVelocity = new Vector2(horizontalInput, 0f) * Mathf.Max(maxSpeed - friction, 0f);

            if (pressingMoveKey)
            {
                FlipSprite();
            }
        }

        private void FixedUpdate()
        {
            charLayerIntStatus = layerIntStatusDefiner.GetCharPhyStatus();

            if (useAcceleration)
            {
                SetAccProperties();
                CalculateDeltaSpeed();

                curVelocity.x = Mathf.MoveTowards(curVelocity.x, desiredVelocity.x, speedDelta); // Given enough time, curVelocity.x will be equal to desiredVelocity.x even if the speedDelta is greater than the difference between them.
                myRigidbody2D.velocity = curVelocity;
            }
            else
            {
                runWithoutAcceleration();
            }
        }

        private void CalculateDeltaSpeed()
        {
            if (pressingMoveKey)
            {
                // If the sign of our horizontalInput doesn't match our movement, it means we are tring to turn.
                if (Mathf.Sign(horizontalInput) != Mathf.Sign(curVelocity.x))
                {
                    // Buradaki secim tamamen tercihe baglidir. Her bir framede belli bir hiz artisi mi istiyorsun yoksa max hiza ne kadar surede ulasacagina gore mi hesaplamak istiyorsun.
                    speedDelta = turnSpeed * Time.fixedDeltaTime; // Her bir fixed framede artacak hiz birimini verir. turnSpeed degiskeninin alabilecegi en yuksek deger, sadece max hiza ulasma suresini kisaltacaktir. Max hiza ulasma suresi, max hizin ne olduguna baglidir.
                    //speedDelta = maxSpeed * GetAccerelationPercent(turnSpeed); // Her bir fixed framede artacak hiz birimi, max hizin yuzde kaci kacar artacagidir, yani burada max hiza tek karede ulasmak icin turn speed degiskeninin en fazla alabilecegi degere esitlenmesi yeterlidir.
                }
                else // If they match, it means we're simply running along and so should use the acceleration stat
                {
                    speedDelta = acceleration * Time.fixedDeltaTime;
                    //speedDelta = maxSpeed * GetAccerelationPercent(acceleration);
                }
            }
            else // And if we're not pressing a direction at all, use the deceleration stat
            {
                speedDelta = deceleration * Time.fixedDeltaTime;
                //speedDelta = maxSpeed * GetAccerelationPercent(deceleration);
            }
        }

        private float GetAccerelationPercent(float accType) // Divides given acceleration type to max acceleration value
        {
            return (accType / 50f);
        }

        private void SetAccProperties()
        {
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
            else if (charLayerIntStatus.Equals(LayerInteractionStateDefiner.CharLayerInteractionStatus.OnAir))
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
        }

        private void runWithoutAcceleration()
        {
            curVelocity.x = desiredVelocity.x;
            myRigidbody2D.velocity = curVelocity;
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
