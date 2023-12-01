using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    [SerializeField]
    private int damage;

    private bool hitted = false; // Bu variable Sorun olursa Silinebilir

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamagable hit = collision.GetComponent<IDamagable>();

        if (hit != null)
        {
            if (damage != 0 && collision.gameObject.tag.Equals("Player") && !hitted)
            {
                hit.takeDamage(damage);
                hitted = true;
            }
        }
    }

    public void setDamage(int damageIn)
    {
        this.damage = damageIn;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        hitted = false;
    }
}
