using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathSelect : MonoBehaviour
{
    public bool SelectingPath;
    public List<HexCell> PathDrawn = new();
    private List<HexCell> _hexPath = new();
    private Enemy _selectedEnemy = null;
    private Coroutine _drawPathRoutine;
    private void OnMouseDown()
    {
        if (SelectingPath == true||_drawPathRoutine!=null) return;
        _selectedEnemy = GetComponent<Enemy>();
        if (_selectedEnemy.Attacking) return;
        SelectingPath = true;
        _drawPathRoutine=StartCoroutine(DrawPath());
    }
    private IEnumerator DrawPath()
    {
        Vector2Int? cellPos = null;
        while (SelectingPath == true)
        {
            if (!Input.GetMouseButton(0)&& cellPos.HasValue)
            {
                _selectedEnemy.FollowPath(cellPos.Value.x,cellPos.Value.y);
                ResetColors();
                if (_hexPath.Count > 0)
                {
                    //foreach (HexCell c in _hexPath)
                    //{
                    //    c.PathVisualizer.SetActive(true);
                    //}
                    yield return new WaitUntil(() => _selectedEnemy.Moving == false || _selectedEnemy.Attacking);
                    SelectingPath = false;
                    //foreach (HexCell c in _hexPath)
                    //{
                    //    c.PathVisualizer.SetActive(false);
                    //    //add check later for if overlap with other enemies' paths.
                    //}
                }
                _drawPathRoutine = null;
                yield break;
            }
            yield return new WaitForEndOfFrame();
            if (Input.GetMouseButton(0))
            {
                //possible select animation?
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider == null) continue;
                HexCell cell;
                if (hit.collider.gameObject.GetComponent<HexCell>())
                {
                    cell = hit.collider.gameObject.GetComponent<HexCell>();
                }
                else continue;
                if (_hexPath.Count > 0) ResetColors();
                if (new Vector2Int(_selectedEnemy.Position.x, _selectedEnemy.Position.y) != new Vector2Int(cell.Position.x,cell.Position.y))
                { 
                    Pathfind(new Vector2Int(_selectedEnemy.Position.x, _selectedEnemy.Position.y), cell.Position);
                    if (_hexPath.Count > 0) cellPos = new Vector2Int(cell.Position.x, cell.Position.y);
                }
            }
        }
    }
    private void Pathfind(Vector2Int position, Vector3Int clickedPos)
    {
        HexMap hexMap = GameObject.Find("HexGrid").GetComponent<HexMap>();
        //if (!hexMap.ReturnHex(clickedPos.x, clickedPos.y).Occupied)
        //{
        //    _hexPath = Pathfinding.FindPath(hexMap.ReturnHex(position.x, position.y), hexMap.ReturnHex(clickedPos.x, clickedPos.y));
        //}
        _hexPath = Pathfinding.FindPath(hexMap.ReturnHex(position.x, position.y), hexMap.ReturnHex(clickedPos.x, clickedPos.y));
        if (_hexPath.Count != 0)
        {
            List<Vector3> pathToFollow = new();
            foreach (HexCell c in _hexPath)
            {
                pathToFollow.Add(new Vector3(c.gameObject.transform.position.x, c.gameObject.transform.position.y, 1));
                c.GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
    }
    private void ResetColors()
    {
        //will be obsolete after icons are added.
        foreach (HexCell c in _hexPath)
        {
            switch (c.TerrainType)
            {
                case "Forest":
                    c.GetComponent<SpriteRenderer>().color = Color.green;
                    break;
                case "Mountain":
                    c.GetComponent<SpriteRenderer>().color = Color.cyan;
                    break;
                case "Spawn":
                    c.GetComponent<SpriteRenderer>().color = Color.blue;
                    break;
                case "Control":
                    c.GetComponent<SpriteRenderer>().color = Color.yellow;
                    break;
                default:
                    c.GetComponent<SpriteRenderer>().color = new Color(0.9f, 0.9f, 0.9f);
                    break;
            }
        }
    }
}
