using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class MovementJuice : MonoBehaviour
    {
        private CharHorizontalController moveScript;
        private CharJumpController jumpScript;
        private Animator animator;
        private GameObject characterSprite;

        [Header("Components - Particles")]
        [SerializeField] private ParticleSystem moveParticles;
        [SerializeField] private ParticleSystem jumpParticles;
        [SerializeField] private ParticleSystem landParticles;

        [Header("Components - Audio")]
        [SerializeField] AudioSource jumpSFX;
        [SerializeField] AudioSource landSFX;

        [Header("Settings - Squash and Stretch")]
        [SerializeField]
        private bool squashAndStretch = true;
        [SerializeField, Tooltip("Width Squeeze, Height Squeeze, Duration")]
        private Vector3 jumpSquashSettings;
        [SerializeField, Tooltip("Width Squeeze, Height Squeeze, Duration")]
        private Vector3 landSquashSettings;
        [SerializeField, Tooltip("How powerful should the effect be?"), Range(1f, 2f)]
        private float landSqueezeMultiplier;
        [SerializeField, Tooltip("How powerful should the effect be?"), Range(1f, 2f)]
        private float jumpSqueezeMultiplier;
        [SerializeField]
        private float landDrop = 1;

        [Header("Tilting")]
        [SerializeField] private bool leanBackward;
        [SerializeField] private bool tiltChar = false;
        [SerializeField, Tooltip("How far should the character tilt?"), Range(0f, 15f)]
        private float maxTilt = 6f;
        [SerializeField, Tooltip("How fast should the character tilt?"), Range(0f, 30f)]
        private float tiltSpeed;

        [Header("Calculations")]
        private float runningSpeed;
        private float maxSpeed = 1f;

        // Current States
        private bool jumpSqueezing, landSqueezing, playerGrounded;

        private void Awake()
        {
            //if (leanBackward) { maxTilt = -maxTilt; }
        }

        void Start()
        {
            moveScript = GetComponent<CharHorizontalController>();
            jumpScript = GetComponent<CharJumpController>();
            animator = GetComponentInChildren<Animator>();
            characterSprite = GetComponentInChildren<SpriteRenderer>().gameObject;
        }

        void Update()
        {
            if (tiltChar)
            {
                TiltCharacter();
            }

            SetAnimationSpeed();

            CheckForLanding();
        }

        private void TiltCharacter()
        {
            float directionToTilt = 0;
            if (moveScript.CurVelocity.x != 0) // See which direction the character is currently running towards, and tilt in that direction
            {
                directionToTilt = Mathf.Sign(moveScript.CurVelocity.x);
                if (leanBackward) { directionToTilt = -directionToTilt; }
            }

            //Create a vector that the character will tilt towards
            Vector3 targetRotVector = new Vector3(0, 0, Mathf.Lerp(-maxTilt, maxTilt, Mathf.InverseLerp(-1, 1, directionToTilt)));

            //And then rotate the character in that direction
            characterSprite.transform.rotation = Quaternion.RotateTowards(animator.transform.rotation, Quaternion.Euler(-targetRotVector), tiltSpeed * Time.deltaTime);
        }

        private void SetAnimationSpeed()
        {
            //We need to change the character's running animation to suit their current speed
            runningSpeed = Mathf.Clamp(Mathf.Abs(moveScript.CurVelocity.x), 0, maxSpeed);
            if (animator != null)
            {
                animator.SetFloat("runSpeed", runningSpeed);
            }
        }

        private void CheckForLanding()
        {
            if (!playerGrounded && jumpScript.CharLayerIntStatus == LayerInteractionStateDefiner.CharLayerInteractionStatus.OnDryLand) // TODO Check swimming conditions
            {
                //By checking for this, and then immediately setting playerGrounded to true, we only run this code once when the player hits the ground 
                playerGrounded = true;

                //Play an animation, some particles, and a sound effect when the player lands
                if (animator != null)
                {
                   //animator.SetTrigger("Landed");
                }

                if (landParticles != null)
                {
                    landParticles.Play();
                }

                if (landSFX != null && !landSFX.isPlaying && landSFX.enabled)
                {
                    landSFX.Play();
                }

                if (moveParticles != null)
                {
                    moveParticles.Play();
                }

                if (squashAndStretch && !landSqueezing && landSqueezeMultiplier >= 1) //Start the landing squash and stretch coroutine.
                {
                    //float velocityMultiplier = Mathf.Abs(jumpScript.CurVelocity.y) / jumpScript.FallSpeedLimit; // Between 0f and 1f. TODO CurVelocity.y value must return the value just before touching the ground.
                    StartCoroutine(JumpSqueeze(landSquashSettings.x * landSqueezeMultiplier, landSquashSettings.y / landSqueezeMultiplier, landSquashSettings.z, landDrop, false));
                }
            }
            else if (playerGrounded && jumpScript.CharLayerIntStatus != LayerInteractionStateDefiner.CharLayerInteractionStatus.OnDryLand) // Player has left the ground, so stop playing the running particles.
            {
                playerGrounded = false;
                if(moveParticles == null) { return; }
                moveParticles.Stop();
            }
        }

        public void JumpEffects()
        {
            if (animator != null)
            {
                //animator.ResetTrigger("Landed");
                animator.SetTrigger("Jump");
            }

            if (jumpSFX != null && jumpSFX.enabled)
            {
                jumpSFX.Play();
            }

            if (squashAndStretch && !jumpSqueezing && jumpSqueezeMultiplier >= 1)
            {
                StartCoroutine(JumpSqueeze(jumpSquashSettings.x / jumpSqueezeMultiplier, jumpSquashSettings.y * jumpSqueezeMultiplier, jumpSquashSettings.z, 0, true));
            }

            if (jumpParticles != null)
            {
                jumpParticles.Play();
            }
        }

        IEnumerator JumpSqueeze(float xSqueeze, float ySqueeze, float seconds, float dropAmount, bool jumpSqueeze)
        {
            //We log that the player is squashing/stretching, so we don't do these calculations more than once
            if (jumpSqueeze) { jumpSqueezing = true; }
            else { landSqueezing = true; }
            //squeezing = true;

            Vector3 originalSize = Vector3.one;
            Vector3 newSize = new Vector3(xSqueeze, ySqueeze, originalSize.z);

            Vector3 originalPosition = Vector3.zero;
            Vector3 newPosition = new Vector3(0, -dropAmount, 0);

            //We very quickly lerp the character's scale and position to their squashed and stretched pose...
            float t = 0f;
            while (t <= 1.0f)
            {
                t += Time.deltaTime / 0.01f;
                characterSprite.transform.localScale = Vector3.Lerp(originalSize, newSize, t);
                characterSprite.transform.localPosition = Vector3.Lerp(originalPosition, newPosition, t);
                yield return null;
            }

            //And then we lerp back to the original scale and position at a speed dicated by the developer
            //It's important to do this to the character's game object that holds sprite, not the gameobject with a Rigidbody an/or collision detection
            t = 0f;
            while (t <= 1.0f)
            {
                t += Time.deltaTime / seconds;
                characterSprite.transform.localScale = Vector3.Lerp(newSize, originalSize, t);
                characterSprite.transform.localPosition = Vector3.Lerp(newPosition, originalPosition, t);
                yield return null;
            }

            if (jumpSqueeze) { jumpSqueezing = false; }
            else { landSqueezing = false; }
        }
    }
}
