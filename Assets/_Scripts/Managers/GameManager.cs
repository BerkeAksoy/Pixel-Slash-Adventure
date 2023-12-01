using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace BerkeAksoyCode
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;

        private List<Enemy> enemyList;
        public Item[] itemHeld;
        public Item[] itemEquipped;
        public Item[] referenceItems;
        public int heldCoins, heldGems, actTag;

        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    Debug.LogError("Game Manager is null.");
                }
                return instance;
            }
        }

        public List<Enemy> EnemyList { get => enemyList; set => enemyList = value; }

        private void Awake()
        {
            if (instance == null)
            {
                enemyList = new List<Enemy>();
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                GameObject.Find("Player").GetComponent<Player>().transform.position = new Vector3(-1, 12, 0);
                TextTyper.Instance.closeDialog();
                LevelManager.Instance.RestartScene();
            }
        }

        public void addCoin(int valueIn)
        {
            heldCoins += valueIn;

            UIManager.Instance.updateCoins();
        }


    }
}