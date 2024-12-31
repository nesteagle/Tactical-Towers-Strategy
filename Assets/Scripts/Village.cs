using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class Village : MonoBehaviour
{
    public HexCell Cell;
    private SupplyManager _supplyManager;
    private Game _manager;
    private HashSet<Unit> _playerControl = new HashSet<Unit>();
    private HashSet<Unit> _enemyControl = new HashSet<Unit>();
    public float Control;
    private Unit _supplier;
    private Coroutine _supplyRoutine;
    public SpriteRenderer Renderer;
    // Start is called before the first frame update
    private void Start()
    {
        Cell = GetComponentInParent<HexCell>();
        GameObject control = GameObject.Find("Control");
        _supplyManager = control.GetComponent<SupplyManager>();
        _manager = control.GetComponent<Game>();
        _manager.TotalVillages.Add(this);
        StartCoroutine(UpdateVillage());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Unit")) return;
        Unit collidedUnit = collision.gameObject.GetComponent<Unit>();
        if (collidedUnit.Team == "Player")
        {
            if (!_playerControl.Contains(collidedUnit)) _playerControl.Add(collidedUnit);
        }
        else if (!_enemyControl.Contains(collidedUnit)) _enemyControl.Add(collidedUnit);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Unit")) return;
        Unit collidedUnit = collision.gameObject.GetComponent<Unit>();
        if (collidedUnit.Team == "Player")
        {
            _playerControl.Remove(collidedUnit);
        }
        else _enemyControl.Remove(collidedUnit);
    }
    private void FixedUpdate()
    {
        Control += (_playerControl.Count - _enemyControl.Count) * 0.125f * Time.fixedDeltaTime;
        if (Control > 1f)
        {
            Control = 1f;
        }
        if (Control < -1f)
        {
            Control = -1f;
        }
    }
    private IEnumerator UpdateVillage()
    {
        while (_manager)
        {//while game running
            yield return new WaitUntil(() => Control >= 1f || Control <= -1f);
            if (Control >= 1f)
            {
                Control = 1f;
                _manager.PlayerVillages.Add(Cell);
                if (_supplyRoutine == null) _supplyRoutine = StartCoroutine(SpawnSupply(_manager.PlayerSpawnCell, true));
                yield return new WaitUntil(() => Control < 0.5f);
                if (_manager.PlayerVillages.Contains(Cell)) _manager.PlayerVillages.Remove(Cell);
            }
            if (Control <= -1f)
            {
                Control = -1f;
                _manager.EnemyVillages.Add(Cell);
                if (_supplyRoutine == null) _supplyRoutine = StartCoroutine(SpawnSupply(_manager.EnemySpawnCell, false));
                yield return new WaitUntil(() => Control > -0.5f);
                if (_manager.EnemyVillages.Contains(Cell)) _manager.EnemyVillages.Remove(Cell);
            }
        }
    }
    private IEnumerator SpawnSupply(HexCell baseCell, bool onPlayerTeam)
    {
        yield return new WaitForSeconds(2.5f);
        _supplier = _supplyManager.SpawnSupplyUnit(baseCell, onPlayerTeam);
        _supplier.StartCoroutine(_supplyManager.SupplyVillage(_supplier, Cell, baseCell));
    }
}
