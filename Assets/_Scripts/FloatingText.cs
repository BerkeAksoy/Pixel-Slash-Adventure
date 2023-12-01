using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{

    void Start()
    {
        transform.localPosition += new Vector3(Random.Range(-0.5f, 0.5f), 1.2f, 0);
        Destroy(gameObject, 0.5f);
    }
}
