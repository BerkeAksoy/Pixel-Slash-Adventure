using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class CharJumpController : MonoBehaviour
    {
        private Rigidbody2D myRB2D;
        private LayerInteractionStateDefiner layerIntStatusDefiner;
        private LayerInteractionStateDefiner.CharLayerInteractionStatus charLayerIntStatus;
        private MovementJuice movementJuice;
        private Vector2 curVelocity;

        [Header("Jumping Stats")]
        [SerializeField, Range(1f, 10f)]
        [Tooltip("Maximum jump height")]
        private float jumpHeight = 2f;

        [SerializeField, Range(0.2f, 1.25f)]
        [Tooltip("How long it takes to reach that height before coming back down")]
        private float timeToJumpApex = 0.5f;

        [SerializeField, Range(0f, 5f)]
        [Tooltip("Gravity multiplier to apply when going up")]
        private float upwardMoveMult = 1f;

        [SerializeField, Range(1f, 10f)]
        [Tooltip("Gravity multiplier to apply when coming down")]
        private float downwardMoveMult = 2f;

        [SerializeField, Range(0, 1)]
        [Tooltip("How many times can you jump in the air?")]
        private int maxAirJumps = 0;

        [Header("Options")]
        [SerializeField]
        [Tooltip("If activated the player needs to hold jump key to achieve higher jump")]
        private bool variableJumpHeight;
        [SerializeField]
        [Tooltip("Adds as much as lost jump velocity, player needs to be experienced to achieve highest double jump. Switching off is not recommended and it is experimental only.")]
        private bool variableJumpVelocity = true;

        [SerializeField, Range(1f, 10f)]
        [Tooltip("Gravity multiplier when you let go of jump")]
        private float jumpCutOff;

        [SerializeField, Range(1f, 40f)]
        [Tooltip("The fastest speed the character can fall")]
        private float fallSpeedLimit =  20f;

        [SerializeField, Range(0f, 0.2f)]
        [Tooltip("How long should coyote time last?")]
        private float coyoteTime = 0.1f;

        [SerializeField, Range(0f, 0.2f)]
        [Tooltip("How far from ground should we cache your jump?")]
        private float jumpBuffer = 0.1f;

        private float jumpVelocity, gravMultiplier, defaultGravityScale = 1f;

        [Header("Current State")]
        private float coyoteTimeCounter = 0, jumpBufferCounter = 0;
        private bool pressingJump, canJumpAgain = false, currentlyJumping, desiredJump;

        public LayerInteractionStateDefiner.CharLayerInteractionStatus CharLayerIntStatus { get => charLayerIntStatus;}

        private void Awake()
        {
            myRB2D = GetComponent<Rigidbody2D>();
            layerIntStatusDefiner = GetComponent<LayerInteractionStateDefiner>();
            movementJuice = GetComponent<MovementJuice>();
        }

        private void Update()
        {
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
        }

        private void FixedUpdate() // Known crucial bug, rarely jump does not work, maybe it is due to input getting or jumpBuffer
        {
            charLayerIntStatus = layerIntStatusDefiner.GetCharPhyStatus();
            curVelocity = myRB2D.velocity;

            if (desiredJump)
            {
                DoAJump();
                curVelocity.y = Mathf.Clamp(curVelocity.y, -fallSpeedLimit, 200);
                myRB2D.velocity = new Vector2(myRB2D.velocity.x, curVelocity.y);
                return; // Skip gravity calculations this frame, so currentlyJumping doesn't turn off. This makes sure you can't do the coyote time double jump bug.
            }

            DefineGravityMultiplier();
            CalculateGravityScale();
        }

        private void CalculateGravityScale()
        {
            Vector2 newGravity = new Vector2(0, (-2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2)); // (1/2)*g*(t^2) = h
            myRB2D.gravityScale = (newGravity.y / Physics2D.gravity.y) * gravMultiplier;
            curVelocity.y = Mathf.Clamp(curVelocity.y, -fallSpeedLimit, 200);
            myRB2D.velocity = new Vector2(myRB2D.velocity.x, curVelocity.y); // We are setting the y velocity to implement jump and to limit falling speed. Gravity handled by unity physics2D.
        }

        private void DefineGravityMultiplier() // We change the character's gravity based on her Y direction
        {
            if (myRB2D.velocity.y > 0.01f) // If Kit is going up somehow, maybe he is on a moving platform
            {
                if (charLayerIntStatus == LayerInteractionStateDefiner.CharLayerInteractionStatus.OnDryLand || charLayerIntStatus == LayerInteractionStateDefiner.CharLayerInteractionStatus.Swimming)
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
                else
                {
                    gravMultiplier = downwardMoveMult;
                }
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
                canJumpAgain = (maxAirJumps == 1 && canJumpAgain == false);
                coyoteTimeCounter = 0;

                AdjustJumpSpeed();
                jumpBufferCounter = 0;

                curVelocity.y += jumpVelocity;
                currentlyJumping = true;

                if (movementJuice != null)
                {
                    movementJuice.JumpEffects();
                }
            }

            if (jumpBuffer == 0) // If we don't have a jump buffer, then turn off desiredJump immediately after hitting jumping
            {
                desiredJump = false;
            }
        }

        private void AdjustJumpSpeed() // If the char is moving up or down when the char jumps (such as when doing a double jump), adjust the jumpSpeed. This method will ensure the jump is the exact same strength, no matter the char's velocity.
        {
            if (curVelocity.y >= 0f)
            {
                if (charLayerIntStatus != LayerInteractionStateDefiner.CharLayerInteractionStatus.OnDryLand) // If the char is not onGround then it means the char is either in cayote-time or canJumpAgain is true. Therefore, we need to calculate jump velocity according to gravityscale which will change after jump. Gravityscale changes because we alter it according to char's vertical movement status.
                {
                    if (variableJumpHeight)
                    {
                        jumpVelocity = Mathf.Sqrt(-2f * Physics2D.gravity.y * (myRB2D.gravityScale / jumpCutOff) * jumpHeight);
                    }
                    else
                    {
                        jumpVelocity = Mathf.Sqrt(-2f * Physics2D.gravity.y * (myRB2D.gravityScale / upwardMoveMult) * jumpHeight);
                    }
                }
                else
                {
                    if (jumpBufferCounter >= 0.03f) // (Also read below comment that starts with okay.) if we are using jump buffer we need to check whether we do jump due to jump buffer or not because if we jump immediately after touching the ground layer, one frame delay will cause wrong gravityScale calculation.
                    {
                        jumpVelocity = Mathf.Sqrt(-2f * Physics2D.gravity.y * (myRB2D.gravityScale / jumpCutOff) * jumpHeight);
                        //Debug.Log("corrected version up");
                    }
                    else
                    {
                        jumpVelocity = Mathf.Sqrt(-2f * Physics2D.gravity.y * myRB2D.gravityScale * jumpHeight);
                    }
                }

                if (variableJumpVelocity)
                {
                    jumpVelocity = Mathf.Max(jumpVelocity - curVelocity.y, 0f); // Adds as much as lost jump velocity, player needs to be experienced to achieve highest double jump
                    //Debug.Log("Jump with a velocity " + jumpVelocity);
                }
                else
                {
                    jumpVelocity = Mathf.Max(jumpVelocity, 0f); // Always adds full jump velocity, eliminates player experience thus, reduces fun.
                }
            }
            else if (curVelocity.y < 0f)
            {
                if (charLayerIntStatus != LayerInteractionStateDefiner.CharLayerInteractionStatus.OnDryLand) // If the char is not onGround then it means the char is either in cayote-time or canJumpAgain is true. Therefore, we need to calculate jump velocity according to gravityscale which will change after jump. Gravityscale changes because we alter it according to char's vertical movement status.
                {
                    jumpVelocity = Mathf.Sqrt(-2f * Physics2D.gravity.y * (myRB2D.gravityScale / downwardMoveMult) * jumpHeight);
                }
                else // okaaay, if an eagle captures our char and rises constantly and our char presses space when the eagle let us go, the char will have positive y velocity. At that specific time if our char hits a ground he will jump with wrong gravityScale thus, we correct it. I know this is a long shot but it is a must to do.
                {
                    if (jumpBufferCounter >= 0.03f)
                    {
                        jumpVelocity = Mathf.Sqrt(-2f * Physics2D.gravity.y * (myRB2D.gravityScale / downwardMoveMult) * jumpHeight);
                        //Debug.Log("corrected version down");
                    }
                    else
                    {
                        jumpVelocity = Mathf.Sqrt(-2f * Physics2D.gravity.y * myRB2D.gravityScale * jumpHeight);
                    }
                }

                float velY = myRB2D.velocity.y;
                jumpVelocity += Mathf.Abs(velY); // Character immediately stops falling and starts rising
            }
        }

        private void JumpBuffer() // Jump buffer allows us to queue up a jump, which will play when we next hit the ground
        {
            if (jumpBuffer > 0)
            {
                if (desiredJump)
                {
                    jumpBufferCounter += Time.deltaTime;

                    if (jumpBufferCounter > jumpBuffer) // If time exceeds the jump buffer, cancel desired jump
                    {
                        desiredJump = false;
                        jumpBufferCounter = 0;
                    }
                }
            }
        }

        private void CayoteTime() // If we're not on the ground and we're not currently jumping, that means we've stepped off the edge of a platform. So, start the coyote time counter...
        {
            if (!currentlyJumping && !charLayerIntStatus.Equals(LayerInteractionStateDefiner.CharLayerInteractionStatus.OnDryLand))
            {
                coyoteTimeCounter += Time.deltaTime;
            }
            else // Reset it when we touch the ground, or jump
            {
                coyoteTimeCounter = 0;
            }
        }


    }
}
