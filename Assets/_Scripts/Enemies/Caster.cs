using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caster : MonoBehaviour
{
    [SerializeField]
    private GameObject[] prefabs = null;

    public void instantiatePrefab(int id, bool isFacingleft)
    {
        if (isFacingleft)
        {
            Instantiate(prefabs[id], transform.position, transform.rotation * Quaternion.Euler(0f, 180f, 0f));
        }
        else
        {
            Instantiate(prefabs[id], transform.position, Quaternion.identity);
        }
    }
}
