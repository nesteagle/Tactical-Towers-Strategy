using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectEnemy : MonoBehaviour
{
    private Tower _tower;
    private Coroutine _firing = null;
    private MonoBehaviour _launcher;
    private void Awake()
    {
        _tower = GetComponentInParent<Tower>();
        _launcher = GetTowerLauncher();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile")) return;
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (_firing == null && enemy)
        {
            _tower.TargetEnemy = enemy;
            _firing = StartCoroutine(FireAtEnemy());
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy == _tower.TargetEnemy)
        {
            _firing = null;
            StopAllCoroutines();
        }
    }
    private IEnumerator FireAtEnemy()
    {
        yield return new WaitForSeconds(0.3f);

        while (_firing != null && _tower.TargetEnemy)
        {
            AttackEnemy();
            yield return new WaitForSeconds(_tower.AttackCooldown);
        }
    }
    private void AttackEnemy()
    {
        if (_tower.TargetEnemy == null) return;
        switch (_tower.TowerType)
        {
            case "Cannon":
                CannonTower cannonLauncher = _launcher as CannonTower;
                StartCoroutine(cannonLauncher.LaunchCannonball(_tower.TargetEnemy));
                break;
            case "Fire":
                FireTower fireLauncher = _launcher as FireTower;
                StartCoroutine(fireLauncher.LaunchFire(_tower.TargetEnemy));
                break;
            case "Tesla":
                TeslaTower boltLauncher = _launcher as TeslaTower;
                StartCoroutine(boltLauncher.LaunchBolt(_tower.TargetEnemy));
                break;
        }
    }
    private MonoBehaviour GetTowerLauncher()
    {
        switch (_tower.TowerType)
        {
            case "Cannon":
                return _tower.GetComponentInChildren<CannonTower>();
            case "Fire":
                return _tower.GetComponentInChildren<FireTower>();
            case "Tesla":
                return _tower.GetComponentInParent<TeslaTower>();
            //case "" CAN ADD MORE TOWERS!
            default:
                return null;
        }
    }
}
