using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BerkeAksoyCode
{
    public class equipSlot : MonoBehaviour
    {

        public Image itemImage;
        public Item item;
        public bool hasItem;
        // 0: Helmet
        // 1: Amulet
        // 2: Weapon
        // 3: Armor
        // 4: Book1
        // 5: Book2
        // 6: Book3
        // 7: Belt
        // 8: Gloves
        // 9: Ring
        // 10: Boots
        public string slotType;
        public int id;

        public void remEquipSlot()
        {
            if (hasItem)
            {
                UIManager.Instance.deEquip(this);
            }
        }


    }
}