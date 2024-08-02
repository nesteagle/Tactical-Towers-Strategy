using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTower : MonoBehaviour
{
    public GameObject firePrefab;
    private Tower _tower;
    public GameObject DamageIndicatorPrefab;
    private void Awake()
    {
        _tower = GetComponentInParent<Tower>();
    }
    public IEnumerator LaunchFire(Enemy enemy)
    {
        if (!enemy) yield break;
        for (int i = 0; i < 8; i++)
        {
            GameObject fire = Instantiate(firePrefab, _tower.transform);
            fire.transform.position -= new Vector3(0, 0, 1);
            StartCoroutine(LaunchProjectile(fire, enemy));
            yield return new WaitForSeconds(0.1f);
        }
    }
    public IEnumerator LaunchProjectile(GameObject fire, Enemy enemy)
    {
        Rigidbody2D body = fire.GetComponent<Rigidbody2D>();
        float speed = 35f;
        Destroy(fire, 4f);
        while (fire && enemy)
        {
            Vector3 dir = enemy.transform.position - _tower.transform.position;
            dir.Normalize();
            Vector2 normalizedVel = new(dir.x, dir.y);
            Quaternion rotation = Quaternion.LookRotation(normalizedVel);
            body.SetRotation(rotation);
            if (speed > 0f)
            {
                speed -= Time.fixedDeltaTime * 2f;
            }
            else Destroy(fire);//will have fade out later
            body.velocity = normalizedVel * speed;//Velocity is magnitude*direction will UPDATE WITH FIRE CODE.
            yield return new WaitForEndOfFrame();
        }
        if (enemy) FireSplashDamage(enemy);
        Destroy(fire,0.2f);
    }
    void FireSplashDamage(Enemy target)
    {
        GameObject debugCircle = Instantiate(DamageIndicatorPrefab, target.transform.position, Quaternion.identity);
        Destroy(debugCircle, 0.5f); 
        Collider2D[] colliders = Physics2D.OverlapCircleAll(target.transform.position, 0.5f);
        foreach (Collider2D c in colliders)
        {
            if (c.gameObject.CompareTag("Enemy"))
            {
                Enemy enemy= c.GetComponent<Enemy>();
                if (enemy.Health >= 0f) enemy.Health -= _tower.AttackPower;
            }
        }
    }
}