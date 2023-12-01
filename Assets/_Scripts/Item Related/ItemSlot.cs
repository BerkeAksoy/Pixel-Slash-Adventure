using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BerkeAksoyCode
{
    public class ItemSlot : MonoBehaviour
    {
        public Image itemImage;
        public Text rarityText;
        public Item item;
        public bool hasItem;
        public int id;
        public static int count;

        private void Awake()
        {
            rarityText = GetComponentInChildren<Text>();
        }

        public void selectItem()
        {
            if (GameObject.Find("Shop Canvas") != null)
            {
                GameObject.Find("Shop Canvas").GetComponent<TownUI>().showSlctdInvItem(item, id);
            }

            if (UIManager.Instance != null)
            {
                UIManager.Instance.showSelectedItem(item, id);
            }
        }

        public void selectShopItem()
        {
            if (GameObject.Find("Shop Canvas") != null)
            {
                GameObject.Find("Shop Canvas").GetComponent<TownUI>().showSlctdShopItem(item, id);
            }
        }
    }
}