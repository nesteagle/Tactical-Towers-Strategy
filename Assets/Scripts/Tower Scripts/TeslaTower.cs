using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaTower : MonoBehaviour
{
    private Tower _tower;
    private LineRenderer _lineRenderer;
    public readonly int MaxChains=5;//can increase with upgrade?
    private void Awake()
    {
        _lineRenderer = GetComponentInChildren<LineRenderer>();
        _lineRenderer.positionCount = 0;
        _lineRenderer.enabled = false;
        _tower = GetComponentInParent<Tower>();
    }
    public IEnumerator LaunchBolt (Enemy target) {
        yield return new WaitForSeconds(0.15f);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(target.transform.position, 1.5f);
        List<Enemy> enemies=new();
        foreach (Collider2D c in colliders)
        {
            if (c.gameObject.CompareTag("Enemy"))
            {
                enemies.Add(c.GetComponent<Enemy>());
            }
        }
        int chainedTargets = Mathf.Min(enemies.Count, MaxChains);
        _lineRenderer.positionCount = chainedTargets + 1;
        _lineRenderer.SetPosition(0, new Vector3(transform.position.x,transform.position.y));

        for (int i = 0; i < chainedTargets; i++)
        {
            _lineRenderer.SetPosition(i + 1, new Vector3(enemies[i].transform.position.x, enemies[i].transform.position.y));
            if (enemies[i].Health > 0f) enemies[i].Health -= _tower.AttackPower;
        }
        _lineRenderer.enabled = true;
        StartCoroutine(LineFadeOut(_lineRenderer));
        // add method in here to damage enemies;
    }
    public IEnumerator LineFadeOut(LineRenderer line)
    {
        yield return new WaitForSeconds(0.4f);
        line.enabled = false;
    }
}
