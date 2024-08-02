using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonTower : MonoBehaviour
{
    public GameObject BallPrefab;
    private Tower _tower;
    private void Awake()
    {
        _tower = GetComponentInParent<Tower>();
    }
    public IEnumerator LaunchCannonball(Enemy enemy)
    {
        GameObject ball = Instantiate(BallPrefab,_tower.transform);
        ball.transform.position -= new Vector3(0, 0, 2);
        Rigidbody2D body = ball.GetComponent<Rigidbody2D>();
        while (ball&&enemy)
        {
            Vector3 dir = enemy.transform.position - _tower.transform.position;
            dir.Normalize();
            Vector2 normalizedVel = new(dir.x, dir.y);
            body.velocity = normalizedVel * 60f;//Velocity is magnitude*direction
            yield return new WaitForEndOfFrame();
        }
        Destroy(ball,0.3f);
        if (!enemy) yield break;
        if (enemy.Health <= 0f)
        {
            yield return new WaitForSeconds(0.1f);
            _tower.TargetEnemy = null;
        }
    }
}
