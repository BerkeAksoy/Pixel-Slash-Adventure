using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class Sellers : MonoBehaviour
    {

        GameObject shopCanvas;

        private void Start()
        {
            shopCanvas = GameObject.Find("Shop Canvas");
        }

        public void openCanvas(string seller)
        {
            shopCanvas.GetComponent<TownUI>().loadSellerArrays(seller);
        }
    }
}