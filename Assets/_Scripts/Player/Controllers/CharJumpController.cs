using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
        private float friction = 0f;

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
        private int maxAirJumps = 1;

        [Header("Options")]
        [SerializeField]
        [Tooltip("If activated the player needs to hold jump key to achieve higher jump")]
        private bool variableJumpHeight;
        [Tooltip("Adds as much as lost jump velocity, player needs to be experienced to achieve highest double jump. Switching off is not recommended and it is experimental only.")]
        private bool variableJumpVelocity = true;

        [Header("Variables")]
        [SerializeField, Range(1f, 10f)]
        [Tooltip("Gravity multiplier when you let go of jump")]
        private float jumpCutOff;

        [SerializeField, Range(1f, 40f)]
        [Tooltip("The fastest speed the character can fall")]
        private float fallSpeedLimit = 20f;
        
        [SerializeField, Range(0f, 20f)]
        [Tooltip("Reduces the fastest speed that the character can fall")]
        private float waterVerFric = 4f, airVerFric = 0f;
        
        [SerializeField, Range(0f, 10f)]
        [Tooltip("At which velocity should hang time start?")]
        private float hangTimeVelThreshold = 0f;

        [SerializeField, Range(0f, 0.2f)]
        [Tooltip("How long should coyote time last?")]
        private float coyoteTime = 0.1f;

        [SerializeField, Range(0f, 0.6f)]
        [Tooltip("How far from ground should we cache your jump?")]
        private float jumpBuffer = 0.1f;

        private float jumpVelocity, gravMultiplier, defaultGravityScale = 1f;
        private int setOffFrames = 1, setOffFrameDecounter = 0;
        private int curAirJumps;

        [Header("Current State")]
        private float coyoteTimeCounter = 0, jumpBufferCounter = 0;
        private bool pressingJump, currentlyJumping, desiredJump;

        public LayerInteractionStateDefiner.CharLayerInteractionStatus CharLayerIntStatus { get => charLayerIntStatus;}

        private void Awake()
        {
            setOffFrameDecounter = setOffFrames;
            curAirJumps = maxAirJumps;
            myRB2D = GetComponent<Rigidbody2D>();
            layerIntStatusDefiner = GetComponent<LayerInteractionStateDefiner>();
            movementJuice = GetComponent<MovementJuice>();
        }

        private void Update()
        {
            JumpInput();
            CayoteTime();
            JumpBuffer();
        }

        private void JumpInput()
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
        }

        private void FixedUpdate()
        {
            charLayerIntStatus = layerIntStatusDefiner.GetCharPhyStatus();
            curVelocity = myRB2D.velocity;

            checkJumpState();

            DefineGravityMultAndFric();
            CalculateGravityScale();
            if (desiredJump)
            {
                DoAJump();
            }

            ApplyVelocity();
        }

        private void checkJumpState()
        {
            if(charLayerIntStatus == LayerInteractionStateDefiner.CharLayerInteractionStatus.OnDryLand)
            {
                currentlyJumping = false;
                curAirJumps = maxAirJumps;
            }
        }

        private void CalculateGravityScale()
        {
            Vector2 newGravity = new Vector2(0, (-2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2)); // (1/2)*g*(t^2) = h
            myRB2D.gravityScale = (newGravity.y / Physics2D.gravity.y) * gravMultiplier;
        }

        private void ApplyVelocity()
        {
            curVelocity.y = Mathf.Clamp(curVelocity.y, -Mathf.Max(fallSpeedLimit - friction, 1f), 200);
            myRB2D.velocity = new Vector2(myRB2D.velocity.x, curVelocity.y); // We are setting the y velocity to implement jump and to limit falling speed. Gravity handled by unity physics2D.
        }

        private void DefineGravityMultAndFric()
        {
            friction = airVerFric;
            
            if (charLayerIntStatus == LayerInteractionStateDefiner.CharLayerInteractionStatus.OnDryLand)
            {
                gravMultiplier = defaultGravityScale;
                return;
            }
                
            if (charLayerIntStatus == LayerInteractionStateDefiner.CharLayerInteractionStatus.Swimming || charLayerIntStatus == LayerInteractionStateDefiner.CharLayerInteractionStatus.WaterWalking)
            {
                gravMultiplier = defaultGravityScale;
                friction = waterVerFric;
                return;
            }
            
            if (myRB2D.velocity.y > 0.01f) // If the char is going up somehow, maybe he is on a moving platform
            {
                if (variableJumpHeight)
                {
                    if (pressingJump)
                    {
                        gravMultiplier = upwardMoveMult;
                    }
                    else
                    {
                        gravMultiplier = jumpCutOff;
                    }
                }
                else
                {
                    gravMultiplier = upwardMoveMult;
                }

                if (myRB2D.velocity.y < hangTimeVelThreshold)
                {
                    gravMultiplier = gravMultiplier/2f;
                }
            }
            else if (myRB2D.velocity.y < -0.01f) // If the char is going down somehow, maybe he is on a moving platform
            {
                gravMultiplier = downwardMoveMult;
                
                if (myRB2D.velocity.y > -hangTimeVelThreshold)
                {
                    gravMultiplier = gravMultiplier/2f;
                }
            }
            else
            {
                gravMultiplier = gravMultiplier/2f;
            }
        }

        private void DoAJump() // Define "WHY" we jumped. // No jumping on the bottom of the water
        {
            if (charLayerIntStatus == LayerInteractionStateDefiner.CharLayerInteractionStatus.OnDryLand || (coyoteTimeCounter > 0.03f && coyoteTimeCounter < coyoteTime))
            {
                ActivateJump();
            }
            else if(charLayerIntStatus == LayerInteractionStateDefiner.CharLayerInteractionStatus.OnAir && curAirJumps > 0)
            {
                ActivateJump();
                curAirJumps--;
            }

            if (jumpBuffer == 0) // If we don't have a jump buffer, then turn off desiredJump immediately after hitting jumping
            {
                desiredJump = false;
            }
        }

        private void ActivateJump()
        {
            desiredJump = false;
            coyoteTimeCounter = 0;
            jumpBufferCounter = 0;

            AdjustJumpSpeed();
            curVelocity.y += jumpVelocity;
            currentlyJumping = true;

            if (movementJuice != null)
            {
                movementJuice.JumpEffects();
            }
        }

        private void AdjustJumpSpeed() // Jump version 1.4 - hang time added // If the char is moving up or down when the char jumps (such as when doing a double jump), adjust the jumpSpeed. This method will ensure the jump is the exact same strength, no matter the char's velocity.
        {
            int reasonNo = 0;

            if (myRB2D.velocity.y > 0.01f)
            {
                jumpVelocity = Mathf.Sqrt(-2f * Physics2D.gravity.y * myRB2D.gravityScale * jumpHeight);

                if (variableJumpVelocity) // Must be true if not experimenting
                {
                    jumpVelocity = Mathf.Max(jumpVelocity - curVelocity.y, 0f); // Adds as much as lost jump velocity, player needs to be experienced to achieve highest double jump
                }
                else
                {
                    jumpVelocity = Mathf.Max(jumpVelocity, 0f); // Always adds full jump velocity, eliminates player experience thus, reduces fun.
                }
            }
            else if (myRB2D.velocity.y < -0.01f)
            {
                if (charLayerIntStatus != LayerInteractionStateDefiner.CharLayerInteractionStatus.OnDryLand) // If the char is not onGround then it means the char is either in cayote-time or canJumpAgain is true. Therefore, we need to calculate jump velocity according to gravityscale which will change after jump. Gravityscale changes because we alter it according to char's vertical movement status.
                {
                    jumpVelocity = Mathf.Sqrt(-2f * Physics2D.gravity.y * (myRB2D.gravityScale / downwardMoveMult) * jumpHeight);
                    reasonNo = 4;
                }
                else // okaaay, if an eagle captures our char and rises constantly and our char presses space when the eagle let us go, the char will have positive y velocity. At that specific time if our char hits a ground he will jump with wrong gravityScale thus, we correct it. I know this is a long shot but it is a must to do.
                {
                    jumpVelocity = Mathf.Sqrt(-2f * Physics2D.gravity.y * myRB2D.gravityScale * jumpHeight);
                    reasonNo = 6;
                }

                float velY = myRB2D.velocity.y;
                jumpVelocity += Mathf.Abs(velY); // Character immediately stops falling and starts rising
            }
            else
            {
                jumpVelocity = Mathf.Sqrt(-2f * Physics2D.gravity.y * myRB2D.gravityScale * jumpHeight);
                //Debug.Log("GravMult: " + gravMultiplier + " downMult: " + downwardMoveMult + " jump Height: " + jumpHeight);
                reasonNo = 8;
            }

            //Debug.Log(jumpVelocity + "Reason: " + reasonNo);
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