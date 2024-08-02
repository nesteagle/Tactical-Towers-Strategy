using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherTower : MonoBehaviour
{
    public GameObject ArrowPrefab;
    private Tower _tower;
    private MultiEnemyDetection _detection;
    public Sprite ArrowSprite;
    private void Awake()
    {
        _tower = GetComponentInParent<Tower>();
        _detection = GetComponentInChildren<MultiEnemyDetection>();
    }
    public IEnumerator LaunchArrow(Enemy enemy)
    {
        GameObject arrow = Instantiate(ArrowPrefab,_tower.transform);
        Rigidbody2D body = arrow.GetComponent<Rigidbody2D>();
        UpdateArrow(body, enemy);
        arrow.GetComponent<SpriteRenderer>().sprite = ArrowSprite;
        while (arrow && enemy)
        {
            UpdateArrow(body,enemy);
            yield return new WaitForEndOfFrame();
        }
        if (!enemy) yield break;
        if (enemy.Health<=0f)
        {
            for (int i = 0; i < _detection.Enemies.Length; i++)
            {
                if (_detection.Enemies[i].Item1 == enemy)
                {
                    yield return new WaitForSeconds(0.1f);
                    _detection.Enemies[i].Item1 = null;
                    break;
                }
            }
        }
    }
    void UpdateArrow(Rigidbody2D body,Enemy enemy)
    {
        Vector3 dir = enemy.transform.position - _tower.transform.position;
        dir.Normalize();
        Vector2 normalizedVel = new(dir.x, dir.y);
        Quaternion rotation = Quaternion.LookRotation(normalizedVel, Vector3.up);
        if (normalizedVel.x > 0)rotation *= new Quaternion(1, 1, Mathf.PI, 1);
        body.SetRotation(rotation);
        body.velocity = normalizedVel * 110f;//Velocity is magnitude*direction
    }
}