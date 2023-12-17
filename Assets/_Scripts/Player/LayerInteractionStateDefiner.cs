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
        [SerializeField] private float groundRayLength = 0.2f, waterRayLength = 0.2f;
        private CharLayerInteractionStatus charLayerIntStatus;
        private Vector3 curPos;

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
            curPos = transform.position;
            CheckLayerInteractionState();
        }

        private void CheckLayerInteractionState()
        {
            onGround = Physics2D.Raycast(curPos - footOffset, Vector2.down, groundRayLength, groundLayer) || Physics2D.Raycast(transform.position + footOffset, Vector2.down, groundRayLength, groundLayer);
            inWater = Physics2D.Raycast(curPos, Vector2.down, waterRayLength, waterLayer);

            if (onGround && !inWater && charLayerIntStatus != CharLayerInteractionStatus.OnDryLand) // On dry land
            {
                charLayerIntStatus = CharLayerInteractionStatus.OnDryLand;
                Debug.DrawRay(transform.position - footOffset, new Vector2(0, -groundRayLength), Color.green, 0.1f);
            }
            else if (inWater && !onGround && charLayerIntStatus != CharLayerInteractionStatus.Swimming) // Swimming
            {
                charLayerIntStatus = CharLayerInteractionStatus.Swimming;
                Debug.DrawRay(transform.position - footOffset, new Vector2(0, -groundRayLength), Color.yellow, 0.1f);
            }
            else if (inWater && onGround && charLayerIntStatus != CharLayerInteractionStatus.WaterWalking) // Walking in water
            {
                charLayerIntStatus = CharLayerInteractionStatus.WaterWalking;
                Debug.DrawRay(transform.position - footOffset, new Vector2(0, -groundRayLength), Color.blue, 0.1f);
            }
            else if (!onGround && !inWater && charLayerIntStatus != CharLayerInteractionStatus.OnAir) // On Air
            {
                charLayerIntStatus = CharLayerInteractionStatus.OnAir;
                Debug.DrawRay(transform.position - footOffset, new Vector2(0, -groundRayLength), Color.red, 0.1f);
            }
        }

        public CharLayerInteractionStatus GetCharPhyStatus()
        {
            return charLayerIntStatus;
        }
    }
}
