using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class LayerInteractionStateDefiner : MonoBehaviour
    {

        [SerializeField] private LayerMask groundLayer, waterLayer;
        [SerializeField] private Vector3 footOffset = new Vector3(0.2f, 0, 0);
        private bool onGround, inWater;
        private float groundRayLength = 0.7f, waterRayLength = 0.2f;
        private CharLayerInteractionStatus charLayerIntStatus;

        public enum CharLayerInteractionStatus
        {
            OnDryLand,
            Swimming,
            WaterWalking,
            OnAir,
            OnLadder
        }

        private void FixedUpdate()
        {
            CheckLayerInteractionState();
        }

        private void CheckLayerInteractionState()
        {
            onGround = Physics2D.Raycast(transform.position - footOffset, Vector2.down, groundRayLength, groundLayer) || Physics2D.Raycast(transform.position + footOffset, Vector2.down, groundRayLength, groundLayer);
            inWater = Physics2D.Raycast(transform.position, Vector2.down, waterRayLength, waterLayer);

            if (onGround && !inWater) // On dry land
            {
                charLayerIntStatus = CharLayerInteractionStatus.OnDryLand;
                Debug.DrawRay(transform.position - footOffset, Vector2.down, Color.green, groundRayLength);
            }
            else if (inWater && !onGround) // Swimming
            {
                charLayerIntStatus = CharLayerInteractionStatus.Swimming;
                Debug.DrawRay(transform.position - footOffset, Vector2.down, Color.yellow, groundRayLength);
            }
            else if (inWater && onGround) // Walking in water
            {
                charLayerIntStatus = CharLayerInteractionStatus.WaterWalking;
                Debug.DrawRay(transform.position - footOffset, Vector2.down, Color.blue, groundRayLength);
            }
            else if (!onGround && !inWater) // On Air
            {
                charLayerIntStatus = CharLayerInteractionStatus.OnAir;
                Debug.DrawRay(transform.position - footOffset, Vector2.down, Color.red, groundRayLength);
            }
        }

        public CharLayerInteractionStatus GetCharPhyStatus()
        {
            return charLayerIntStatus;
        }
    }
}
