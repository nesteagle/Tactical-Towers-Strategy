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
        yield return new WaitForSeconds(_enemy.Cooldown / 2f);
        while (_enemy && targetEnemy.Health > 0)
        {
            //if (targetEnemy.Type != "Knight"&&_isArcher==false)
            //{
            //    targetEnemy.Health--;
            //}
            //else
            targetEnemy.Health-=_enemy.Damage;
            yield return new WaitForSeconds(_enemy.Cooldown);//maybe will be attack cooldown.
        }
        if (_enemy) _enemy.Attacking = false;
    }
    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (_enemy.Attacking == true) return;
    //    if (collision.gameObject.CompareTag("Projectile") || collision.gameObject.CompareTag("Tile")) return; //Used for Arrows
    //    Enemy enemy;
    //    if (collision.gameObject.GetComponent<Enemy>())//collision.gameObject.GetComponent<Enemy>()
    //    {
    //        enemy = collision.gameObject.GetComponent<Enemy>();
    //        //GetIntersectingTile(enemy).Occupied=true;
    //        GetIntersectingTile(enemy).GetComponent<SpriteRenderer>().color = Color.yellow;
    //        _enemy.StopAllCoroutines();
    //        StartCoroutine(MoveToEnemy(enemy));
    //        _enemy.Attacking = true;
    //        //team check enemy and if same team return;
    //    }
    //    else return;
    //    //GetIntersectingTile(enemy);
    //    //replaced with AttackEnemy. Call GetIntersectingTile there.
    //    //need team sort mechanic
    //    //if (enemy.OnPlayerTeam != _enemy.OnPlayerTeam)
    //    //{
    //    //    StartCoroutine(MoveToEnemy(enemy));
    //    //    _enemy.Attacking = true;
    //    //}
    //}
    ////private void OnTriggerExit2D(Collider2D collision)
    ////{
    ////    Enemy enemy = collision.gameObject.GetComponent<Enemy>();
    ////}

    //private HexCell GetIntersectingTile(Enemy targetEnemy)
    //{
    //    Vector3 displacement;
    //    Vector3 pos1 = _enemy.transform.position;
    //    Vector3 pos2 = targetEnemy.transform.position;
    //    displacement = new Vector3((pos2.x - pos1.x) / 2 + pos1.x, (pos2.y - pos1.y) / 2 + pos1.y);
    //    Collider2D[] hits = Physics2D.OverlapCircleAll(displacement, 0.2f);
    //    foreach (Collider2D hit in hits)
    //    {
    //        HexCell cell = hit.gameObject.GetComponent<HexCell>();
    //        if (cell == null) continue;
    //        if (cell.Occupied)
    //        {
    //            foreach (HexCell adjacent in cell.AdjacentTiles)
    //            {
    //                if (!adjacent.Occupied) return adjacent;
    //            }
    //        }
    //        else return cell;
    //        //if (cell == null||cell.Occupied==true) continue;
    //        //return cell;

    //        //MIGHT WANT TO REPLACE WHOLE FUNCTION WITH ATTACKING SCRIPT
    //    }
    //    return null;
    //}
    //private IEnumerator MoveToEnemy(Enemy targetEnemy)
    //{
    //    HexCell intersecting = GetIntersectingTile(targetEnemy);
    //    _enemy.Moving = false;
    //    _enemy.FollowPath(intersecting.Position.x, intersecting.Position.y);
    //    //intersecting.Occupied = true;
    //    yield return new WaitUntil(() => _enemy.Moving == false);
    //}
    //private IEnumerator AttackEnemy(Enemy targetEnemy)
    //{
    //    while (_enemy.Health > 0 && targetEnemy.Health > 0)
    //    {
    //        targetEnemy.Health--;
    //        yield return new WaitForSeconds(1);//maybe will be attack cooldown.
    //    }
    //    if (_enemy) _enemy.Attacking = false;
    //}
}
