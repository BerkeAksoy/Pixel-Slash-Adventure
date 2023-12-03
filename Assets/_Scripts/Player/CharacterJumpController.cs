using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class CharacterJumpController : MonoBehaviour
    {
        [Header("Components")]
        private Rigidbody2D myRigidbody2D;
        private LayerInteractionStateDefiner layerInteractionStateDefiner;
        private LayerInteractionStateDefiner.CharLayerInteractionStatus charLayerIntStatus;
        private Vector2 velocity;
        //private characterJuice juice;

        [Header("Jumping Stats")]
        [SerializeField, Range(2f, 8f)][Tooltip("Maximum jump height")]
        private float jumpHeight = 7.3f;

        private bool jumpPressed, jumpReleased;

        [SerializeField, Range(0.2f, 1.25f)][Tooltip("How long it takes to reach that height before coming back down")]
        public float timeToJumpApex;

        [SerializeField, Range(0f, 5f)][Tooltip("Gravity multiplier to apply when going up")]
        public float upwardMovementMultiplier = 1f;

        [SerializeField, Range(1f, 10f)]
        [Tooltip("Gravity multiplier to apply when coming down")]
        public float downwardMovementMultiplier = 6.17f;

        [SerializeField, Range(0, 1)][Tooltip("How many times can you jump in the air?")]
        public int maxAirJumps = 0;

        [Header("Options")]
        [SerializeField][Tooltip("Should the character drop when you let go of jump?")]
        public bool variablejumpHeight;

        [SerializeField, Range(1f, 10f)][Tooltip("Gravity multiplier when you let go of jump")]
        public float jumpCutOff;

        [SerializeField][Tooltip("The fastest speed the character can fall")] // fallSpeedLimit
        public float fallSpeedLimit;

        [SerializeField, Range(0f, 0.2f)][Tooltip("How long should coyote time last?")]
        public float coyoteTime = 0.1f;

        [SerializeField, Range(0f, 0.3f)][Tooltip("How far from ground should we cache your jump?")]
        public float jumpBuffer = 0.1f;

        [Header("Calculations")]
        public float jumpSpeed;
        private float defaultGravityScale;
        public float gravMultiplier;

        [Header("Current State")]
        public bool canJumpAgain = false;
        private bool desiredJump;
        private float jumpBufferCounter;
        private float coyoteTimeCounter = 0;
        private bool pressingJump;
        public bool onGround;
        private bool currentlyJumping;
        private bool alwaysFullThrust;

        private void Awake()
        {
            
        }

        private void Update()
        {
            charLayerIntStatus = layerInteractionStateDefiner.GetCharPhyStatus();
            if (Input.GetKeyDown(KeyCode.Space)){
                jumpPressed = true;
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                jumpReleased = true;
                jumpPressed = false;
            }
        }

        private void FixedUpdate()
        {
            calculateGravity();
        }

        private void calculateGravity() // We change the character's gravity based on her Y direction
        {
            if (charLayerIntStatus.Equals(LayerInteractionStateDefiner.CharLayerInteractionStatus.OnAir))
            {
                if (myRigidbody2D.velocity.y > 0.01f) // If Kit is going up...
                {
                    if (variablejumpHeight)
                    {
                        if (pressingJump && currentlyJumping) // Apply upward multiplier if player is rising and holding jump
                        {
                            gravMultiplier = upwardMovementMultiplier;
                        }
                        else // But apply a special downward multiplier if the player lets go of jump
                        {
                            gravMultiplier = jumpCutOff;
                        }
                    }
                    else
                    {
                        gravMultiplier = upwardMovementMultiplier;
                    }
                }
                else if (myRigidbody2D.velocity.y < -0.01f) // If Kit is going down...
                {
                    gravMultiplier = downwardMovementMultiplier;
                }
            }

            //Set the character's Rigidbody's velocity
            //But clamp the Y variable within the bounds of the speed limit, for the terminal velocity assist option
            myRigidbody2D.velocity = new Vector3(velocity.x, Mathf.Clamp(velocity.y, -fallSpeedLimit, 100));
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

                //Determine the power of the jump, based on our gravity and stats
                jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * myRigidbody2D.gravityScale * jumpHeight);

                //If Kit is moving up or down when she jumps (such as when doing a double jump), change the jumpSpeed // This will ensure the jump is the exact same strength, no matter your velocity.
                if (velocity.y > 0f)
                {
                    if (alwaysFullThrust)
                    {
                        jumpSpeed = Mathf.Max(jumpSpeed, 0f); // Tam ziplama gucu ekliyor
                    }
                    else
                    {
                        jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0f); // Havada kaybettigi gucu ekliyor
                    }
                }
                else if (velocity.y < 0f)
                {
                    jumpSpeed += Mathf.Abs(myRigidbody2D.velocity.y);
                }

                //Apply the new jumpSpeed to the velocity. It will be sent to the Rigidbody in FixedUpdate;
                velocity.y += jumpSpeed;
                //currentlyJumping = true;

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

    }
}
