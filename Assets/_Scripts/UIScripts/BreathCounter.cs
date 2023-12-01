using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BreathCounter : MonoBehaviour
{

    private int numberOfSlots = 5;
    private float slotValue;
    public Image[] breathBubbles;
    public Sprite fullBreath, emptyBreath;

    public void activateIndicator()
    {
        gameObject.SetActive(true);
    }

    public void deActivateIndicator()
    {
        gameObject.SetActive(false);
    }

    public void updateBreathIndicator(int maxBreath, int curBreath)
    {
        slotValue = (float)maxBreath / (float)numberOfSlots;

        int activeBreathSlots = numberOfSlots - (int)((maxBreath - curBreath) / slotValue);

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




}
