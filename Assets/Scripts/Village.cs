using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Village : MonoBehaviour
{
    public HexCell Cell;
    private SupplyManager _supplyManager;
    private Game _manager;
    private List<Enemy> _playerControl = new List<Enemy>();
    private List<Enemy> _enemyControl = new List<Enemy>();
    public float Control;
    private Enemy _supplier;
    private Coroutine _supplyRoutine;
    public SpriteRenderer Renderer;
    // Start is called before the first frame update
    private void Start()
    {
        Cell = GetComponentInParent<HexCell>();
        GameObject control = GameObject.Find("Control");
        _supplyManager = control.GetComponent<SupplyManager>();
        _manager = control.GetComponent<Game>();
        _manager.TotalVillages.Add(Cell);
        StartCoroutine(UpdateVillage());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Enemy")) return;
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy.OnPlayerTeam)
        {
            if (!_playerControl.Contains(enemy)) _playerControl.Add(enemy);
        }
        else if (!_enemyControl.Contains(enemy)) _enemyControl.Add(enemy);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Enemy")) return;
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy.OnPlayerTeam)
        {
            if (_playerControl.Contains(enemy)) _playerControl.Remove(enemy);
        }
        else if (_enemyControl.Contains(enemy)) _enemyControl.Remove(enemy);
    }
    private void FixedUpdate()
    {
        Control += (_playerControl.Count-_enemyControl.Count) * 0.125f * Time.fixedDeltaTime;
    }
    private IEnumerator UpdateVillage()
    {
        while (true) {//while game running
            yield return new WaitUntil(() => Control > 1f || Control < 1f);
            if (Control > 1f)
            {
                Control = 1f;
                if (_supplyRoutine == null) _supplyRoutine = StartCoroutine(SpawnSupply(_manager.PlayerSpawnCell, true)); //eventually add this method to basecell and have queue system
            }
            if (Control < -1f)
            {
                Control = -1f;
                if (_supplyRoutine == null) _supplyRoutine = StartCoroutine(SpawnSupply(_manager.EnemySpawnCell, false));
            }
            if (-0.5f>Control && 0.5f < Control)
            {
                if (_manager.PlayerVillages.Contains(Cell)) _manager.PlayerVillages.Remove(Cell);
                else if (_manager.EnemyVillages.Contains(Cell))_manager.EnemyVillages.Remove(Cell);
            }
        }
    }
    private IEnumerator SpawnSupply(HexCell baseCell, bool onPlayerTeam)
    {
        if (onPlayerTeam) if (!_manager.PlayerVillages.Contains(Cell)) _manager.PlayerVillages.Add(Cell);
        else if (_manager.EnemyVillages.Contains(Cell)) _manager.EnemyVillages.Add(Cell);
        yield return new WaitForSeconds(2.5f);//can do anim here
        _supplier = _supplyManager.SpawnSupplyUnit(baseCell,onPlayerTeam);
        _supplier.StartCoroutine(_supplyManager.SupplyVillage(_supplier, Cell, baseCell));
    }
}
