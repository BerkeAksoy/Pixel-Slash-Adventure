using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BerkeAksoyCode
{
    public class BreathController : MonoBehaviour
    {
        [SerializeField, Tooltip("Sets at what each time period the char does breath"), Range(0.1f, 1.5f)]
        private float inhaleTime = 0.25f;
        [SerializeField, Tooltip("Sets at what each time period the char does choke"), Range(0.1f, 1.5f)]
        private float chokeTime = 0.5f;
        
        [SerializeField, Tooltip("Sets at what each time period the char does choke"), Range(0.1f, 1.5f)]
        private int chokeDamage = 10;
        
        private bool canBreath;
        private float inhaleTimeCounter = 0f, chokeTimeCounter = 0f;

        private CharStats charStats;
        private BreathUI breathUI;
        private BreathStateDefiner stateDefiner;
        private Player player;

        private void Awake()
        {
            charStats = GetComponent<CharStats>();
            breathUI = GetComponentInChildren<BreathUI>(true);
            stateDefiner = GetComponent<BreathStateDefiner>();
            player = GetComponent<Player>();
        }

        private void Update()
        {
            if (canBreath)
            {
                chokeTimeCounter = 0;
                RefillBreath();
            }
            else
            {
                inhaleTimeCounter = 0;
                Choke();
            }
        }

        private void FixedUpdate()
        {
            canBreath = stateDefiner.isBreathable();
        }
        
        private void RefillBreath()
        {
            if (!charStats.FullBreath)
            {
                inhaleTimeCounter += Time.deltaTime;

                if (inhaleTimeCounter > inhaleTime)
                {
                    inhaleTimeCounter = 0;
                    charStats.CurBreathLevel ++; // Inhale amount
                    
                    if (charStats.CurBreathLevel >= charStats.BreathCapacity)
                    {
                        charStats.FullBreath = true;
                        breathUI.UIOnOff(false);
                    }
                    else
                    {
                        breathUI.UpdateBreathUI(charStats.BreathCapacity, charStats.CurBreathLevel);
                    }
                }
            }
        }
        
        private void Choke()
        {
            chokeTimeCounter += Time.deltaTime;

            if (chokeTimeCounter > chokeTime)
            {
                chokeTimeCounter = 0;
                
                if (charStats.CurBreathLevel > 0)
                {
                    charStats.CurBreathLevel--; // Choke amount
                    charStats.FullBreath = false;

                    breathUI.UpdateBreathUI(charStats.BreathCapacity, charStats.CurBreathLevel);
                }
                else
                {
                    player.takeDamage(chokeDamage);
                }
            }
        }
    }
}
