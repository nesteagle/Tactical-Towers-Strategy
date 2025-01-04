using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SelectPath : MonoBehaviour
{
    // PURPOSE: On mouse down, if dragging on enemy, draw path from enemy to mouse position.
    // On mouse release, stop drawing path and attempt pathfinding to last position.

    // Since this script is attached to each Unit prefab, _unit = GetComponent<Unit>()

    private Unit _unit;
    private List<HexCell> _pathToClear = new();
    HexCell _targetCell;
    private bool _enabled = false;

    private void Awake()
    {
        StartCoroutine(WaitForUnitLoad());
    }

    private void OnMouseDrag()
    {
        if (_unit.State != "Rest" || !_enabled) return;
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider == null) return;

        if (hit.collider.gameObject.TryGetComponent(out HexCell hexCell))
        {
            _targetCell = hexCell;
        }
        else return;

        ClearPath(_pathToClear);
        _pathToClear = DrawPath(_unit.TilePosition, _targetCell.Position);
    }

    private List<HexCell> DrawPath(Vector2Int start, Vector3Int finish)
    {
        HexMap map = Game.Map;
        List<HexCell> path = Pathfinding.FindPath(map.ReturnHex(start.x, start.y), map.ReturnHex(finish.x, finish.y));
        if (path != null)
        {
            foreach (HexCell c in path)
            {
                c.GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
        return path;
    }

    private void OnMouseUp()
    {
        if (_targetCell == null || !_enabled) return;
        _unit.MoveTo(_targetCell);
        Game.EnemyBrain.UnitManagement.PlayerUnitMoved(_unit, _targetCell);
        StartCoroutine(ClearOnRest());
    }
    private void ClearPath(List<HexCell> path)
    {
        if (path != null)
        {
            foreach (HexCell c in path)
            {
                c.ResetColor();
            }
        }
    }
    private IEnumerator ClearOnRest()
    {
        yield return new WaitUntil(() => _unit.State == "Rest");
        ClearPath(_pathToClear);
    }

    private IEnumerator WaitForUnitLoad()
    {
        yield return new WaitUntil(() => TryGetComponent(out Unit unit));
        if (TryGetComponent(out Unit unit))
            _unit = unit;
        if (unit.Team == "Player")
        {
            _enabled = true;
        }
        else _enabled = false;
    }
}
