using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiEnemyDetection : MonoBehaviour
{
    private Tower _tower;
    private MonoBehaviour _launcher;
    public (Enemy, Coroutine)[] Enemies;/* = new (Enemy,bool)[3] { (null, false), (null, false), (null, false) }*/
    private void Awake()
    {
        _tower = GetComponentInParent<Tower>();
        _launcher = GetTowerLauncher();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        Enemy enemy= collision.gameObject.GetComponent<Enemy>();

        if (collision.gameObject.CompareTag("Projectile")) return;
        if (!enemy) return;
        bool added = false;
        for (int i = 0; i < Enemies.Length; i++)
        {
            if (Enemies[i].Item1 == enemy)
            {
                added = true;
                break;
            }//If enemies contains this enemy already
        }
        if (added == false)
        {
            for (int i = 0; i < Enemies.Length; i++)
            {
                if (Enemies[i].Item1 == null && Enemies[i].Item2 == null)
                {
                    Enemies[i].Item1 = enemy;
                    Enemies[i].Item2 = StartCoroutine(FireAtEnemy(i));
                    break;
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Enemy e = collision.gameObject.GetComponent<Enemy>();
        for (int i = 0; i < Enemies.Length; i++)
        {
            if (Enemies[i].Item1 == e)
            {
                Enemies[i].Item1 = null;
                if (Enemies[i].Item2 != null)
                {
                    StopCoroutine(Enemies[i].Item2);
                    Enemies[i].Item2 = null;
                }
            }
        }
    }
    private MonoBehaviour GetTowerLauncher()
    {
        switch (_tower.TowerType)
        {
            case "Archer":
                {
                    Enemies = new (Enemy, Coroutine)[3] { (null, null), (null, null), (null, null) };
                    return _tower.GetComponentInChildren<ArcherTower>();
                }
        }
        return null;
    }
    private IEnumerator FireAtEnemy(int index)
    {
        yield return new WaitForSeconds(0.3f+index/5f);
        while (Enemies[index].Item1 != null)
        {
            AttackEnemy(index);
            yield return new WaitForSeconds(_tower.AttackCooldown);
        }
        Enemies[index].Item2 = null;
    }
    private void AttackEnemy(int index)
    {
        if (Enemies[index].Item1 == null) return; 
        switch (_tower.TowerType)
        {
            case "Archer":
                {
                    ArcherTower arrowLauncher = _launcher as ArcherTower;
                    StartCoroutine(arrowLauncher.LaunchArrow(Enemies[index].Item1));
                }
                break;
        }
    }
}
