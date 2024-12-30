using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDetection : MonoBehaviour
{
    private Unit _unit;
    private Unit _targetUnit;
    private Coroutine _attackRoutine;
    private void Awake()
    {
        _unit = GetComponentInParent<Unit>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_unit.State == "Attacking") return;
        if (!collision.gameObject.CompareTag("Enemy")) return;
        Unit target = collision.gameObject.GetComponent<Unit>();
        if (target)
        {
            if (_unit.Team != target.Team)
            {
                //_enemy.StopAllCoroutines(); //maybe add movement check first.
                _unit.State = "Attacking";
                _attackRoutine = StartCoroutine(AttackUnit(target));
                return;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Enemy")) return;
        Unit unit = collision.gameObject.GetComponent<Unit>();
        if (unit == _targetUnit)
        {
            if (_attackRoutine != null)
            {
                StopCoroutine(_attackRoutine);
                unit.State = "Rest";
                _attackRoutine = null;
            }
        }
    }
    private IEnumerator AttackUnit(Unit target)
    {
        yield return new WaitForSeconds(_unit.Cooldown * 1 / 3f);
        while (_unit && target.Health > 0)
        {
            target.Health -= _unit.AttackDamage;
            yield return new WaitForSeconds(_unit.Cooldown * 2 / 3f);//maybe will be attack cooldown.
        }
        if (_unit) _unit.State = "Rest";
    }
}
