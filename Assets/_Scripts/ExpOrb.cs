using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BerkeAksoyCode
{
    public class ExpOrb : MonoBehaviour
    {

        private Player player;

        private int expValue;
        private float moveSpeed = 4f;

        public int ExpValue { get => expValue;
            set => expValue = value;}

        private void Awake()
        {
            moveSpeed = Random.Range(4f, 6f);
        }

        void Start()
        {
            player = GameObject.Find("Player").GetComponent<Player>();
        }

        private void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                player.stats.addXP(expValue);
                Destroy(gameObject);
            }
        }
    }
}