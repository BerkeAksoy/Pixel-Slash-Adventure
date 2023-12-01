using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class Item : MonoBehaviour
    {

        private bool isCollected = false, canPick = false;
        public ItemDataSO itemData;

        private void Start()
        {
            StartCoroutine(WaitToBePickable());
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && !isCollected && canPick)
            {
                isCollected = true;
                if (UIManager.Instance.addItemToInventory(this))
                {
                    Destroy(gameObject);
                }
            }
        }

        IEnumerator WaitToBePickable()
        {
            yield return new WaitForSeconds(0.4f);
            canPick = true;
        }



    }
}