using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace BerkeAksoyCode
{
    public class FollowPlayer : MonoBehaviour
    {
        private CinemachineVirtualCamera vcam;

        void Start()
        {
            Player player = GameObject.Find("Player").GetComponent<Player>();
            vcam = GetComponent<CinemachineVirtualCamera>();
            vcam.LookAt = player.transform;
            vcam.Follow = player.transform;
        }
    }
}