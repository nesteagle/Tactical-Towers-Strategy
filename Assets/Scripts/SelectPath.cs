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
    private List<HexCell> _pathToClear = new List<HexCell>();
    HexCell _targetCell;

    private void Awake()
    {
        _unit = GetComponent<Unit>();
    }

    private void OnMouseDrag()
    {
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
        Debug.Log(_targetCell);
        if (_targetCell == null) return;
        _unit.MoveTo(_targetCell.Position.x, _targetCell.Position.y);
        ClearPath(_pathToClear);
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
}
