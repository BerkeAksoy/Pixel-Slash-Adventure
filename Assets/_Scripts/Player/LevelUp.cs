using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class LevelUp : MonoBehaviour
    {

        private float originalScale;
        private Player player;

        void Start()
        {
            player = GameObject.Find("Player").GetComponent<Player>();
            Destroy(gameObject, 0.8f);
            transform.parent = player.transform;
            originalScale = player.transform.localScale.x;
        }

        private void Update()
        {
            if (player.transform.localScale.x != originalScale) // Even if the player localScale changes levelUp sprite stays the same
            {
                originalScale = player.transform.localScale.x;
                transform.localScale = new Vector2(-transform.localScale.x, 1f);
            }
        }
    }
}