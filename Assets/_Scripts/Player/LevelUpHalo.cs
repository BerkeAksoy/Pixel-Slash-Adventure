using UnityEngine;

namespace BerkeAksoyCode
{
    public class LevelUpHalo : MonoBehaviour
    {
        private GameObject toFollow;

        void Start()
        {
            Destroy(gameObject, 0.8f);
        }

        private void Update()
        {
            if(toFollow == null){ return;}
            transform.position = toFollow.transform.position;
        }

        public void SetupLevelHalo(GameObject levelGainer)
        {
            toFollow = levelGainer;
        }
    }
}