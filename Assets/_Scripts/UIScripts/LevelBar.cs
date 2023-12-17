using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BerkeAksoyCode
{
    public class LevelBar : MonoBehaviour
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
            uvRect.x -= .04f * Time.deltaTime;
            uvRect.y -= .2f * Time.deltaTime;
            shadow.uvRect = uvRect;
        }

        public void updateXP(CharStats playerStats)
        {
            float normalizedXP = normalizeXP(playerStats);

            Vector2 maskSizeDelta = maskRectTransform.sizeDelta;
            maskSizeDelta.x = normalizedXP * maskWidth;
            maskRectTransform.sizeDelta = maskSizeDelta;

            //transform.Find("Mask").localScale = new Vector2(normalizedHealth, transform.Find("Mask").localScale.y);
        }

        private float normalizeXP(CharStats playerStats)
        {
            return (float)playerStats.currentXP / (float)playerStats.ReqExpForNextLevel[playerStats.currentLevel - 1];
        }
    }
}