using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueBossEffect : MonoBehaviour
{

    public int damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        checkDamagable(collision);
    }

    protected virtual void checkDamagable(Collider2D collision)
    {
        IDamagable hit = collision.GetComponent<IDamagable>();

        if (hit != null)
        {
            hit.takeDamage(damage);
        }
    }
}
