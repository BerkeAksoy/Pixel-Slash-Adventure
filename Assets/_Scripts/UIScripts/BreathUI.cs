using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BerkeAksoyCode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BreathUI : MonoBehaviour
{
    private int numberOfSlots = 5;
    public Image[] breathBubbles;
    public Sprite fullBreath, emptyBreath;

    public void UIOnOff(bool value)
    {
        gameObject.SetActive(value);
    }

    public void UpdateBreathUI(int maxBreath, int curBreath)
    {
        if (!isActiveAndEnabled)
        {
            UIOnOff(true);
        }
        
        int slotValue = maxBreath / numberOfSlots;

        int activeBreathSlots = numberOfSlots - (maxBreath - curBreath) / slotValue;

        for(int i = 0; i < numberOfSlots; i++)
        {
            if (i < activeBreathSlots)
            {
                breathBubbles[i].sprite = fullBreath;
            }
            else
            {
                breathBubbles[i].sprite = emptyBreath;
            }
        }
    }
    
    /*public void flipBreathSprite()
    {
        if (myBC.isActiveAndEnabled)
        {
            transform.localScale = new Vector2(1f, 1f);
        }
    }*/




}
