using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class LayerInteractionStateDefiner : MonoBehaviour
    {

        [SerializeField] private LayerMask groundLayer, waterLayer;
        [SerializeField] private Vector3 footOffset = new Vector3(0.18f, 0, 0);
        private bool onGround, inWater;
        [SerializeField] private float groundRayLength = 0.6f, waterRayLength = 0.3f;
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
            Vector3 curPos = transform.position;
            onGround = Physics2D.Raycast(curPos - footOffset, Vector2.down, groundRayLength, groundLayer) || Physics2D.Raycast(curPos + footOffset, Vector2.down, groundRayLength, groundLayer); // Calculated through feet.
            inWater = Physics2D.Raycast(curPos, Vector2.down, waterRayLength, waterLayer); // Calculated through body.

            if (onGround && !inWater && charLayerIntStatus != CharLayerInteractionStatus.OnDryLand) // On dry land
            {
                charLayerIntStatus = CharLayerInteractionStatus.OnDryLand;
            }
            else if (inWater && !onGround && charLayerIntStatus != CharLayerInteractionStatus.Swimming) // Swimming
            {
                charLayerIntStatus = CharLayerInteractionStatus.Swimming;
            }
            else if (inWater && onGround && charLayerIntStatus != CharLayerInteractionStatus.WaterWalking) // Walking in water
            {
                charLayerIntStatus = CharLayerInteractionStatus.WaterWalking;
            }
            else if (!onGround && !inWater && charLayerIntStatus != CharLayerInteractionStatus.OnAir) // On Air
            {
                charLayerIntStatus = CharLayerInteractionStatus.OnAir;
            }
            
            //Debug.DrawRay(curPos - footOffset, new Vector2(0, -groundRayLength), Color.green, 0.1f);
            //Debug.DrawRay(curPos, new Vector2(0, -waterRayLength), Color.blue, 0.1f);
        }

        public CharLayerInteractionStatus GetCharPhyStatus()
        {
            return charLayerIntStatus;
        }
    }
}
