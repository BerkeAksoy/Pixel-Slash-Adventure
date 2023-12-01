using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BerkeAksoyCode
{
    public class HealthBar : MonoBehaviour
    {
        private RawImage fillImage, shadow;
        private RectTransform maskRectTransform;
        private float maskWidth;
        public Gradient gradient;

        private void Awake()
        {
            maskRectTransform = transform.Find("Mask").GetComponent<RectTransform>();
            fillImage = transform.Find("Mask/Fill").GetComponent<RawImage>();

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

        public void updateHealth(Enemy enemy)
        {
            float normalizedHealth = normalizeHealth(enemy);

            Vector2 maskSizeDelta = maskRectTransform.sizeDelta;
            maskSizeDelta.x = normalizedHealth * maskWidth;
            maskRectTransform.sizeDelta = maskSizeDelta;

            fillImage.color = gradient.Evaluate(normalizedHealth);
        }

        public void updateHealth(CharStats playerStats)
        {
            float normalizedHealth = normalizeHealth(playerStats);

            Vector2 maskSizeDelta = maskRectTransform.sizeDelta;
            maskSizeDelta.x = normalizedHealth * maskWidth;
            maskRectTransform.sizeDelta = maskSizeDelta;

            //transform.Find("Mask").localScale = new Vector2(normalizedHealth, transform.Find("Mask").localScale.y);



            fillImage.color = gradient.Evaluate(normalizedHealth);
        }

        private float normalizeHealth(Enemy enemy)
        {
            return (float)enemy.getHealth() / (float)enemy.getMaxHealth();
        }

        private float normalizeHealth(CharStats playerStats)
        {
            return (float)playerStats.currentHP / (float)playerStats.maxHP;
        }



    }
}