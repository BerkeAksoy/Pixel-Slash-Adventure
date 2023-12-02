using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class CharacterJumpController : MonoBehaviour
    {

        private bool jumpPressed, jumpReleased;

        private void Awake()
        {
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)){
                jumpPressed = true;
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                jumpReleased = true;
            }
        }

    }
}
