using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BerkeAksoyCode
{
    public class ManaBar : MonoBehaviour
    {
        private RawImage shadow;
        private RectTransform maskRectTransform;
        private float maskWidth;

        private void Awake()
        {
            maskRectTransform = transform.Find("Mask").GetComponent<RectTransform>();

            shadow = transform.Find("Mask/Shadow").GetComponent<RawImage>();

            maskWidth = maskRectTransform.sizeDelta.x;
        }

        private void Update()
        {
            Rect uvRect = shadow.uvRect;
            uvRect.x -= .045f * Time.deltaTime;
            uvRect.y -= .25f * Time.deltaTime;
            shadow.uvRect = uvRect;
        }

        public void updateMana(CharStats playerStats)
        {
            float normalizedMana = normalizeMana(playerStats);

            Vector2 maskSizeDelta = maskRectTransform.sizeDelta;
            maskSizeDelta.x = normalizedMana * maskWidth;
            maskRectTransform.sizeDelta = maskSizeDelta;

            //transform.Find("Mask").localScale = new Vector2(normalizedHealth, transform.Find("Mask").localScale.y);
        }

        private float normalizeMana(CharStats playerStats)
        {
            return (float)playerStats.currentMana / (float)playerStats.maxMana;
        }
    }
}