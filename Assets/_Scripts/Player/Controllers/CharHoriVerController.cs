using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode {
    public class CharHoriVerController : MonoBehaviour
    {
        private Rigidbody2D myRB2D;
        private LayerInteractionStateDefiner layerIntStatusDefiner;
        private LayerInteractionStateDefiner.CharLayerInteractionStatus charLayerIntStatus;
        private Vector2 curVelocity, desiredVelocity;
        private bool facingRight = true, pressingMoveHoriKey, pressingMoveVerKey;
        private float horizontalInput = 0f, verticalInput = 0f, acceleration, deceleration, turnSpeed, speedDeltaX, speedDeltaY,  friction = 0f;

        [SerializeField, Tooltip("See the code to understand how to calculate"), Range(0f, 50f)]
        private float maxGroundTurnSpeed = 32f, maxWaterTurnSpeed = 21f, maxAirTurnSpeed = 12f,
            maxGroundAcceleration = 40f, maxGroundDeceleration = 40f,
            maxWaterAcceleration = 25f, maxWaterDeceleration = 35f,
            maxAirAcceleration = 12f, maxAirDeceleration = 8f;

        [SerializeField, Range(0f, 20f)]
        private float maxSpeed = 7f;

        [SerializeField, Tooltip("Reduces max speed"), Range(0f, 10f)]
        private float groundFriction = 0f, waterFriction = 4.5f, airFriction = 0f; 
        [SerializeField]
        private bool useAcceleration, useAirAssist;
        
        private bool feared;
        private float climbingSpeed;

        public Vector2 CurVelocity { get => curVelocity;}
        public Rigidbody2D MyRB2D { get => myRB2D; set => myRB2D = value; }

        private void Awake()
        {
            myRB2D = GetComponent<Rigidbody2D>();
            layerIntStatusDefiner = GetComponent<LayerInteractionStateDefiner>();
        }

        private void Update()
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            pressingMoveHoriKey = horizontalInput != 0 ? true : false;

            desiredVelocity = new Vector2(horizontalInput, 0f) * Mathf.Max(maxSpeed - friction, 0f);
            
            if (charLayerIntStatus == LayerInteractionStateDefiner.CharLayerInteractionStatus.Swimming ||
                charLayerIntStatus == LayerInteractionStateDefiner.CharLayerInteractionStatus.WaterWalking)
            {
                verticalInput = Input.GetAxis("Vertical");
                pressingMoveVerKey = verticalInput != 0 ? true : false;
                
                float verticalSpeed = 0f;
                if (pressingMoveVerKey)
                {
                    verticalSpeed = verticalInput * Mathf.Max(maxSpeed - friction, 0f);
                }

                if (verticalInput < 0)
                {
                    CharJumpController.MinimumFallSpeed = Mathf.Max(maxSpeed - friction, 1f);
                }
                else
                {
                    CharJumpController.MinimumFallSpeed = 1f;
                }
                
                desiredVelocity = new Vector2(desiredVelocity.x, verticalSpeed);

                if (pressingMoveHoriKey && pressingMoveVerKey)
                {
                    desiredVelocity = new Vector2(desiredVelocity.x, verticalSpeed) / Mathf.Sqrt(2);
                }
            }

            if (pressingMoveHoriKey)
            {
                FlipSprite();
            }
        }

        private void FixedUpdate()
        {
            charLayerIntStatus = layerIntStatusDefiner.GetCharPhyStatus();

            if (useAcceleration)
            {
                RunWithAcc();
            }
            else
            {
                RunWithoutAcc();
            }
        }

        private void CalculateDeltaSpeed()
        {
            if (pressingMoveHoriKey)
            {
                if (Mathf.Sign(horizontalInput) != Mathf.Sign(curVelocity.x)) // If the sign of our horizontalInput doesn't match our movement, it means we are trying to turn.
                {
                    // Buradaki secim tamamen tercihe baglidir. Her bir framede belli bir hiz artisi mi istiyorsun yoksa max hiza ne kadar surede ulasacagina gore mi hesaplamak istiyorsun.
                    speedDeltaX = turnSpeed * Time.fixedDeltaTime; // Her bir fixed framede artacak hiz birimini verir. turnSpeed degiskeninin alabilecegi en yuksek deger, sadece max hiza ulasma suresini kisaltacaktir. Max hiza ulasma suresi, max hizin ne olduguna baglidir.
                    //speedDelta = maxSpeed * GetAccerelationPercent(turnSpeed); // Her bir fixed framede artacak hiz birimi, max hizin yuzde kaci kacar artacagidir, yani burada max hiza tek karede ulasmak icin turn speed degiskeninin en fazla alabilecegi degere esitlenmesi yeterlidir.
                }
                else // If they match, it means we're simply running along and so should use the acceleration stat
                {
                    speedDeltaX = acceleration * Time.fixedDeltaTime;
                    //speedDelta = maxSpeed * GetAccerelationPercent(acceleration);
                }
            }
            else // And if we're not pressing a direction at all, use the deceleration stat
            {
                speedDeltaX = deceleration * Time.fixedDeltaTime;
                //speedDelta = maxSpeed * GetAccerelationPercent(deceleration);
            }

            if (pressingMoveVerKey)
            {
                if (Mathf.Sign(verticalInput) != Mathf.Sign(curVelocity.y))
                {
                    speedDeltaY = turnSpeed * Time.fixedDeltaTime;
                }
                else
                {
                    speedDeltaY = acceleration * Time.fixedDeltaTime;
                }
            }
            else
            {
                speedDeltaY = deceleration * Time.fixedDeltaTime;
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
                friction = groundFriction;
            }
            else if (charLayerIntStatus.Equals(LayerInteractionStateDefiner.CharLayerInteractionStatus.Swimming) || charLayerIntStatus.Equals(LayerInteractionStateDefiner.CharLayerInteractionStatus.WaterWalking))
            {
                acceleration = maxWaterAcceleration;
                deceleration = maxWaterDeceleration;
                turnSpeed = maxWaterTurnSpeed;
                friction = waterFriction;
            }
            else if (charLayerIntStatus.Equals(LayerInteractionStateDefiner.CharLayerInteractionStatus.OnAir))
            {
                if (useAirAssist)
                {
                    acceleration = maxAirAcceleration;
                    deceleration = maxAirDeceleration;
                    turnSpeed = maxAirTurnSpeed;
                }
                else
                {
                    acceleration = 0f;
                    deceleration = 0f;
                    turnSpeed = 0f;
                }

                friction = airFriction;
            }
            else // Default case
            {
                acceleration = maxGroundAcceleration;
                deceleration = maxGroundDeceleration;
                turnSpeed = maxGroundTurnSpeed;
                friction = groundFriction;
            }
        }

        private void RunWithAcc()
        {
            SetAccProperties();
            CalculateDeltaSpeed();

            curVelocity.x = Mathf.MoveTowards(curVelocity.x, desiredVelocity.x, speedDeltaX); // Given enough time, curVelocity.x will be equal to desiredVelocity.x even if the speedDelta is greater than the difference between them.
            //curVelocity.y = Mathf.MoveTowards(myRB2D.velocity.y, desiredVelocity.y, speedDelta); // burayi bi dinlendikten sonra dusun
            
            if (desiredVelocity.y != 0)
            {
                curVelocity.y = Mathf.MoveTowards(myRB2D.velocity.y, desiredVelocity.y, speedDeltaY);
                myRB2D.velocity = new Vector2(curVelocity.x, curVelocity.y);
            }
            else
            {
                myRB2D.velocity = new Vector2(curVelocity.x, myRB2D.velocity.y);
            }
        }

        private void RunWithoutAcc()
        {
            curVelocity.x = desiredVelocity.x;
            
            if (desiredVelocity.y != 0)
            {
                curVelocity.y = desiredVelocity.y;
                myRB2D.velocity = new Vector2(curVelocity.x, curVelocity.y);
            }
            else
            {
                myRB2D.velocity = new Vector2(curVelocity.x, myRB2D.velocity.y);
            }
            //myRB2D.velocity = curVelocity;
        }

        private void FlipSprite()
        {
            // Flips the sprite according to player's input.
            if (!facingRight && horizontalInput > 0)
            {
                transform.localScale = new Vector2(1f, 1f);
                facingRight = true;
            }
            else if (facingRight && horizontalInput < 0)
            {
                transform.localScale = new Vector2(-1f, 1f);
                facingRight = false;
            }
        }

        public bool isFacingRight()
        {
            return facingRight;
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

        // probably I will control fear through state machine
        // public void fear()
        //     {
        //         feared = true;
        //         StartCoroutine(reFear());
        //     }

        //     IEnumerator reFear()
        //     {
        //         yield return new WaitForSeconds(3f);
        //         feared = false;
        //     }
    }
}