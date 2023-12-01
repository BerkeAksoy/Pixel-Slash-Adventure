using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class Dropper : MonoBehaviour
    {
        [SerializeField]
        private int goldDropChance = 0, gemDropChance = 0;

        public Item[] dropableItemList;
        private Item[] itemsToDrop;

        public int maxGoldValue, maxDropCount, minDropCount;
        public Gold[] coinPrefabs;

        public void dropCoins()
        {
            Player player = GameObject.Find("Player").GetComponent<Player>();

            if (Random.Range(1, 101) <= goldDropChance)
            {
                Vector2 dropPos = new Vector2(transform.position.x + Mathf.Sign(transform.position.x - player.transform.position.x) * Random.Range(0.5f, 1f), transform.position.y + Random.Range(0.4f, 1f));
                Instantiate(coinPrefabs[0], dropPos, Quaternion.identity);
            }
        }

        public void dropGem()
        {
            if (Random.Range(1, 10001) <= gemDropChance)
            {

            }
        }

        public void dropItem()
        {
            Player player = GameObject.Find("Player").GetComponent<Player>();

            if (maxDropCount > dropableItemList.Length)
            {
                maxDropCount = dropableItemList.Length;
            }
            itemsToDrop = new Item[Random.Range(minDropCount, maxDropCount + 1)];

            Debug.Log("Item drop");

            for (int i = 0; i < itemsToDrop.Length; i++)
            {
                if (Random.Range(1, 101) <= dropableItemList[i].itemData.dropChance) // Itemin chancini gir aq
                {
                    itemsToDrop[i] = dropableItemList[i];
                }

                if (itemsToDrop[i] != null)
                {
                    Vector2 dropPos = new Vector2(transform.position.x + Mathf.Sign(transform.position.x - player.transform.position.x) * Random.Range(0.5f, 1f), transform.position.y + Random.Range(0.4f, 1f));
                    Instantiate(itemsToDrop[i], dropPos, Quaternion.identity);
                }
            }
        }



    }
}