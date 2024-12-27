using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDetection : MonoBehaviour
{
    private Enemy _enemy;
    private Enemy _targetEnemy;
    private Coroutine _attackRoutine;
    //private bool _isArcher;
    private void Awake()
    {
        _enemy = GetComponentInParent<Enemy>();
        //if (_enemy.Type == "Archer") _isArcher = true;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_enemy.Attacking == true) return;
        if (!collision.gameObject.CompareTag("Enemy")) return;
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy)//collision.gameObject.GetComponent<Enemy>()
        {
            _targetEnemy = enemy;
            if (_enemy.OnPlayerTeam!=_targetEnemy.OnPlayerTeam)
            {
                //_enemy.StopAllCoroutines(); //maybe add movement check first.
                _enemy.Attacking = true;
                _attackRoutine=StartCoroutine(AttackEnemy(_targetEnemy));
                return;
            }
        }
        }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Enemy")) return;
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy == _targetEnemy)
        {
            if (_attackRoutine != null)
            {
                StopCoroutine(_attackRoutine);
                _enemy.Attacking = false;
                _attackRoutine = null;
            }
        }
    }
    private IEnumerator AttackEnemy(Enemy targetEnemy)
    {
        yield return new WaitForSeconds(_enemy.Cooldown * 1/3f);
        while (_enemy && targetEnemy.Health > 0)
        {
            //if (targetEnemy.Type != "Knight"&&_isArcher==false)
            //{
            //    targetEnemy.Health--;
            //}
            //else
            targetEnemy.Health-=_enemy.Damage;
            yield return new WaitForSeconds(_enemy.Cooldown * 2/3f);//maybe will be attack cooldown.
        }
        if (_enemy) _enemy.Attacking = false;
    }
}
