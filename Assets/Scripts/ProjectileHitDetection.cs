using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHitDetection : MonoBehaviour
{
    Tower Tower;
    private void Awake()
    {
        Tower = GetComponentInParent<Tower>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy)
        {
            Destroy(gameObject);
            enemy.Health -= Tower.AttackPower;
        }
    }
}
