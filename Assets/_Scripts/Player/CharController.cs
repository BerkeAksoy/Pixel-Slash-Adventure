using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode {
    public class CharController : MonoBehaviour
    {
        private Rigidbody2D myRB2D;
        private LayerInteractionStateDefiner layerIntStatusDefiner;
        private LayerInteractionStateDefiner.CharLayerInteractionStatus charLayerIntStatus;
        private Vector2 curVelocity, desiredVelocity;
        private bool lookingRight, pressingMoveKey;
        private float horizontalInput = 0, acceleration, deceleration, turnSpeed, speedDelta; // friction = 0f;

        [SerializeField, Range(0f, 50f)]
        [Tooltip("See the code to understand how to calculate")]
        private float maxGroundTurnSpeed = 50f, maxWaterTurnSpeed = 30f, maxAirTurnSpeed = 50f,
            maxGroundAcceleration = 50f, maxGroundDeceleration = 50f,
            maxWaterAcceleration = 50f, maxWaterDeceleration = 50f,
            maxAirAcceleration = 50f, maxAirDeceleration = 50f;

        [SerializeField, Range(0f, 20f)] private float maxSpeed = 10f;
        [SerializeField] private bool useAcceleration, useAirAssist;
        private bool feared;
        private float climbingSpeed;

        //-----------------------------------------------------------------------------------------------------------------------------

        [Header("Jumping Stats")]
        [SerializeField, Range(1f, 6f)]
        [Tooltip("Maximum jump height")]
        private float jumpHeight = 1f;

        [SerializeField, Range(0.2f, 1.25f)]
        [Tooltip("How long it takes to reach that height before coming back down")]
        public float timeToJumpApex = 0.5f;

        [SerializeField, Range(0f, 5f)]
        [Tooltip("Gravity multiplier to apply when going up")]
        public float upwardMoveMult = 1f;

        [SerializeField, Range(1f, 10f)]
        [Tooltip("Gravity multiplier to apply when coming down")]
        public float downwardMoveMult = 2f;

        [SerializeField, Range(0, 1)]
        [Tooltip("How many times can you jump in the air?")]
        public int maxAirJumps = 0;

        [Header("Options")]
        [SerializeField] public bool variableJumpHeight;

        [SerializeField, Range(1f, 10f)]
        [Tooltip("Gravity multiplier when you let go of jump")]
        public float jumpCutOff;

        [SerializeField]
        [Tooltip("The fastest speed the character can fall")]
        public float fallSpeedLimit = 100;

        [SerializeField, Range(0f, 0.2f)]
        [Tooltip("How long should coyote time last?")]
        public float coyoteTime = 0.1f;

        [SerializeField, Range(0f, 0.3f)]
        [Tooltip("How far from ground should we cache your jump?")]
        public float jumpBuffer = 0.1f;

        private float jumpVelocity, gravMultiplier, calculatedGravityScale, defaultGravityScale = 1f;

        [Header("Current State")]
        private float coyoteTimeCounter = 0, jumpBufferCounter;
        private bool pressingJump, canJumpAgain = false, currentlyJumping, desiredJump;
        [SerializeField] private bool alwaysFullThrust;

        private void Awake()
        {
            myRB2D = GetComponent<Rigidbody2D>();
            layerIntStatusDefiner = GetComponent<LayerInteractionStateDefiner>();
        }

        void Update()
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            pressingMoveKey = horizontalInput != 0 ? true : false;

            desiredVelocity = new Vector2(horizontalInput, 0f) * Mathf.Max(maxSpeed, 0f); // If friction is wanted, just subtract it from maxSpeed.

            if (Input.GetKeyDown(KeyCode.Space))
            {
                desiredJump = true;
                pressingJump = true;
            }
            if (Input.GetKeyUp(KeyCode.Space)) 
            {
                pressingJump = false;
            }

            CayoteTime();
            JumpBuffer();

            if (pressingMoveKey)
            {
                FlipSprite();
            }
        }

        private void FixedUpdate()
        {
            charLayerIntStatus = layerIntStatusDefiner.GetCharPhyStatus();
            curVelocity = myRB2D.velocity;
            DefineGravityMultiplier();
            CalculateGravityScale();

            if (useAcceleration)
            {
                RunWithAcc();
            }
            else
            {
                RunWithoutAcc();
            }

            if (desiredJump)
            {
                Debug.Log("We should see a jump");
                DoAJump();
                myRB2D.velocity = new Vector2(myRB2D.velocity.x, Mathf.Clamp(curVelocity.y, -fallSpeedLimit, 100));
                return; // Skip gravity calculations this frame, so currentlyJumping doesn't turn off. This makes sure you can't do the coyote time double jump bug.
            }


            Debug.Log("Current velocity: " + curVelocity + " \nrigidbody velocity: " + myRB2D.velocity);
            myRB2D.velocity = new Vector2(curVelocity.x, myRB2D.velocity.y);
        }

        private void CalculateGravityScale()
        {
            Vector2 newGravity = new Vector2(0, (-2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2)); // (1/2)*g*(t^2) = h
            myRB2D.gravityScale = (newGravity.y / Physics2D.gravity.y) * gravMultiplier;

            //curVelocity.y += Physics2D.gravity.y * gravMultiplier * Time.fixedDeltaTime;
            //curVelocity.y = Mathf.Clamp(curVelocity.y, -fallSpeedLimit, 100);
        }

        private void DefineGravityMultiplier() // We change the character's gravity based on her Y direction
        {
            if (myRB2D.velocity.y > 0.01f) // If Kit is going up somehow, maybe he is on a moving platform
            {
                if(charLayerIntStatus == LayerInteractionStateDefiner.CharLayerInteractionStatus.OnDryLand || charLayerIntStatus == LayerInteractionStateDefiner.CharLayerInteractionStatus.Swimming)
                {
                    gravMultiplier = defaultGravityScale;
                }

                if (variableJumpHeight)
                {
                    if (pressingJump && currentlyJumping) // Apply upward multiplier if player is rising and holding jump
                    {
                        gravMultiplier = upwardMoveMult;
                    }
                    else // But apply a special downward multiplier if the player lets go of jump
                    {
                        gravMultiplier = jumpCutOff;
                    }
                }
                else
                {
                    gravMultiplier = upwardMoveMult;
                }
            }
            else if (myRB2D.velocity.y < -0.01f) // If Kit is going down somehow, maybe he is on a moving platform
            {
                if (charLayerIntStatus == LayerInteractionStateDefiner.CharLayerInteractionStatus.OnDryLand || charLayerIntStatus == LayerInteractionStateDefiner.CharLayerInteractionStatus.Swimming)
                {
                    gravMultiplier = defaultGravityScale;
                }

                gravMultiplier = downwardMoveMult;
            }
            else // If the char is not moving vertically
            {
                if (charLayerIntStatus == LayerInteractionStateDefiner.CharLayerInteractionStatus.OnDryLand) // and if he is on the ground
                {
                    currentlyJumping = false;
                }

                gravMultiplier = defaultGravityScale;
            }
        }

        private void DoAJump() // No jumping on the bottom of the water
        {
            if (charLayerIntStatus.Equals(LayerInteractionStateDefiner.CharLayerInteractionStatus.OnDryLand) || (coyoteTimeCounter > 0.03f && coyoteTimeCounter < coyoteTime) || canJumpAgain)
            {
                desiredJump = false;
                jumpBufferCounter = 0;
                coyoteTimeCounter = 0;

                //If we have double jump on, allow us to jump again (but only once)
                canJumpAgain = (maxAirJumps == 1 && canJumpAgain == false);

                jumpVelocity = Mathf.Sqrt(-2f * Physics2D.gravity.y * myRB2D.gravityScale * jumpHeight); // v^2 = 2gh
                Debug.Log("Needed jump velocity to reach desired jump height is: " + jumpVelocity);

                if (curVelocity.y > 0f) // If Kit is moving up or down when she jumps (such as when doing a double jump), change the jumpSpeed. This will ensure the jump is the exact same strength, no matter your velocity.
                {
                    if (alwaysFullThrust)
                    {
                        jumpVelocity = Mathf.Max(jumpVelocity, 0f); // Tam ziplama gucu ekliyor
                    }
                    else
                    {
                        jumpVelocity = Mathf.Max(jumpVelocity - curVelocity.y, 0f); // Havada kaybettigi gucu ekliyor
                    }
                }
                else if (curVelocity.y < 0f)
                {
                    jumpVelocity += Mathf.Abs(myRB2D.velocity.y);
                }

                curVelocity.y += jumpVelocity;

                currentlyJumping = true;

                /*if (juice != null)
                {
                    //Apply the jumping effects on the juice script
                    juice.jumpEffects();
                }*/
            }

            if (jumpBuffer == 0) // If we don't have a jump buffer, then turn off desiredJump immediately after hitting jumping
            {
                desiredJump = false;
            }
        }

        private void JumpBuffer() // Jump buffer allows us to queue up a jump, which will play when we next hit the ground
        {
            if (jumpBuffer > 0)
            {
                Debug.Log("I am in jump buffer");
                // Instead of immediately turning off "desireJump", start counting up...
                // All the while, the DoAJump function will repeatedly be fired off
                if (desiredJump)
                {
                    Debug.Log("I am in jump buffer and also in desired jump");
                    jumpBufferCounter += Time.deltaTime;

                    if (jumpBufferCounter > jumpBuffer) //If time exceeds the jump buffer, turn off "desireJump"
                    {
                        desiredJump = false;
                        jumpBufferCounter = 0;
                    }
                }
            }
        }

        private void CayoteTime()
        {
            // If we're not on the ground and we're not currently jumping, that means we've stepped off the edge of a platform. So, start the coyote time counter...
            if (!currentlyJumping && !charLayerIntStatus.Equals(LayerInteractionStateDefiner.CharLayerInteractionStatus.OnDryLand))
            {
                coyoteTimeCounter += Time.deltaTime;
            }
            else // Reset it when we touch the ground, or jump
            {
                coyoteTimeCounter = 0;
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

        private void RunWithAcc()
        {
            SetAccProperties();
            CalculateDeltaSpeed();

            curVelocity.x = Mathf.MoveTowards(curVelocity.x, desiredVelocity.x, speedDelta); // Given enough time, curVelocity.x will be equal to desiredVelocity.x even if the speedDelta is greater than the difference between them.
            //myRB2D.velocity = curVelocity;
        }

        private void RunWithoutAcc()
        {
            curVelocity.x = desiredVelocity.x;
            //myRB2D.velocity = curVelocity;
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