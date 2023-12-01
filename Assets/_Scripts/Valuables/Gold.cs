using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class Gold : MonoBehaviour
    {

        private int value;
        public string type;
        private bool isCollected;

        private void Start()
        {
            startType();
        }

        public void startType()
        {
            if (type.Equals("Mega"))
            {
                value = Random.Range(800, 1001);
            }
            else if (type.Equals("Golden"))
            {
                value = Random.Range(200, 351);
            }
            else if (type.Equals("Silver"))
            {
                value = Random.Range(75, 200);
            }
            else if (type.Equals("Bronz"))
            {
                value = Random.Range(1, 51);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && !isCollected)
            {
                isCollected = true;
                GameManager.Instance.addCoin(value);
                Destroy(gameObject);
            }
        }


    }
}